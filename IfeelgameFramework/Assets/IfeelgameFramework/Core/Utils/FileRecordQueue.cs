using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace IfeelgameFramework.Core.Utils
{
    /// <summary>
    /// 文件记录；FIFO队列
    /// </summary>
    public class FileRecordQueue
    {
        class QueuedJob
        {
            bool _isDone = false;

            struct Job
            {
                public string record;
                public Action<FileRecordQueue> action;
            }

            FileRecordQueue queue;
            System.Threading.Thread thread = null;
            List<Job> jobs = new List<Job>();

            public bool isDone
            {
                get
                {
                    bool tmp;
                    lock (jobs)
                    {
                        tmp = jobs.Count == 0 || _isDone;
                    }
                    return tmp;
                }
                set
                {
                    lock (jobs)
                    {
                        _isDone = value;
                    }
                }
            }

            public QueuedJob(FileRecordQueue queue)
            {
                this.queue = queue;
            }

            void AddAction(Job job)
            {
                _isDone = false;
                jobs.Add(job);
                if (thread == null)
                {
                    thread = new System.Threading.Thread(DoJobs);
                    thread.Start();
                }
            }

            public void AddRecord(string record)
            {
                lock (jobs)
                {
                    AddAction(new Job() { record = record });
                }
            }

            public void DoAction(Action<FileRecordQueue> action)
            {
                lock (jobs)
                {
                    AddAction(new Job() { action = action });
                }
            }

            public void DoJobs()
            {
                Job job;
                while (true)
                {
                    lock (jobs)
                    {
                        if (jobs.Count > 0)
                        {
                            job = jobs[0];
                            jobs.RemoveAt(0);
                        }
                        else
                        {
                            isDone = true;
                            thread = null;
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(job.record))
                        queue.Add(job.record);
                    else if (job.action != null)
                        job.action(queue);
                }
            }
        }


        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        struct RecHeader
        {
            public short sign;
            public short recSize;
            public short recMax;
            public short recCount;
            public short recFirst;
            public short recLast;

            public short reserved1;
            public short reserved2;
        }

        const int HEADER_SIZE = 16;

        public static byte[] GetBytes<T>(T str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];
            GCHandle h = default(GCHandle);

            try
            {
                h = GCHandle.Alloc(arr, GCHandleType.Pinned);
                Marshal.StructureToPtr<T>(str, h.AddrOfPinnedObject(), false);
            }
            finally
            {
                if (h.IsAllocated)
                    h.Free();
            }

            return arr;
        }

        public static T FromBytes<T>(byte[] arr) where T : struct
        {
            T str = default(T);
            GCHandle h = default(GCHandle);

            try
            {
                h = GCHandle.Alloc(arr, GCHandleType.Pinned);
                str = Marshal.PtrToStructure<T>(h.AddrOfPinnedObject());
            }
            finally
            {
                if (h.IsAllocated)
                    h.Free();
            }

            return str;
        }

        RecHeader header;
        QueuedJob thread;
        string fileName;
        FileStream fileStream;
        byte[] stringBuf;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">文件名；推荐将此文件保存在Unity的Application.persistentDataPath下</param>
        /// <param name="recordSize">每个记录的大小（字节数）；请保证大多数记录小于此数字</param>
        /// <param name="maxRecords">最多保留的记录数目</param>
        public FileRecordQueue(string fileName, int recordSize = 512, int maxRecords = 1000)
        {
            header.sign = 1;
            header.recSize = (short)recordSize;
            header.recMax = (short)maxRecords;
            header.recCount = 0;
            header.recFirst = 0;
            header.recLast = 0;

            stringBuf = new byte[recordSize];

            this.fileName = fileName;
            fileStream = new FileStream(fileName, FileMode.OpenOrCreate,
                                        FileAccess.ReadWrite, FileShare.None);

            if (fileStream == null)
                throw new Exception("Cannot open file: " + fileName);

            thread = new QueuedJob(this);
            //thread.DoAction(InitRecords);
            InitFile();
        }

        void RebuildFile()
        {
            //
            ClearAll();
        }

        void InitFile()
        {
            fileStream.Position = 0;
            if (fileStream.Length >= HEADER_SIZE)
            {
                byte[] buf = new byte[HEADER_SIZE];
                fileStream.Read(buf, 0, HEADER_SIZE);
                RecHeader tmpHeader = FromBytes<RecHeader>(buf);

                if (tmpHeader.recSize != header.recSize || tmpHeader.recMax != header.recMax)
                    RebuildFile();
                else
                    header = tmpHeader;
            }
            else
            {
                fileStream.Write(GetBytes<RecHeader>(header), 0, HEADER_SIZE);
            }
        }

        //static void InitRecords(FileRecordQueue queue)
        //{
        //    queue.InitFile();
        //}

        void RemoveOne()
        {
            if (header.recCount > 0)
            {
                bool loop = true;
                while (loop)
                {
                    fileStream.Position = HEADER_SIZE + header.recFirst * header.recSize;
                    fileStream.Read(stringBuf, 0, header.recSize);
                    if (stringBuf[header.recSize - 1] == 0)
                        loop = false;

                    header.recFirst++;
                    if (header.recFirst >= header.recMax)
                        header.recFirst = 0;
                }

                header.recCount--;
            }
        }

        void RemoveOldRecords(int count)
        {
            int n = header.recLast;
            while (count > 0)
            {
                if (n == header.recFirst && header.recCount > 0)
                    RemoveOne();
                n++;
                if (n >= header.recMax)
                    n = 0;
                count--;
            }
        }

        internal void Add(string record)
        {
            lock (thread)
            {
                int len = Encoding.UTF8.GetByteCount(record) + 1;
                int n = len / header.recSize;
                if (len % header.recSize > 0)
                    n++;

                RemoveOldRecords(n);

                byte[] buf;
                if (n == 1)
                {
                    Array.Clear(stringBuf, 0, header.recSize);
                    buf = stringBuf;
                }
                else
                    buf = new byte[n * header.recSize];

                Encoding.UTF8.GetBytes(record, 0, record.Length, buf, 0);


                for (int i = 0; i < n; i++)
                {
                    fileStream.Position = HEADER_SIZE + header.recSize * header.recLast;
                    fileStream.Write(buf, i * header.recSize, header.recSize);

                    // write content
                    header.recLast++;
                    if (header.recLast >= header.recMax)
                        header.recLast = 0;
                }

                header.recCount++;
                SyncHeader();
            }
        }

        internal void SyncHeader()
        {
            // write header
            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.Write(GetBytes<RecHeader>(header), 0, HEADER_SIZE);
            fileStream.Flush();
        }

        /// <summary>
        /// 写入记录
        /// </summary>
        /// <param name="record"></param>
        public void WriteRecord(string record)
        {
            if (fileStream == null)
                throw new Exception("FileRecordQueue::WriteRecord: file not open");

            int len = System.Text.Encoding.UTF8.GetByteCount(record) + 1;
            int n = len / header.recSize;
            if (len % header.recSize > 0)
                n++;
            if (n > header.recMax)
                throw new Exception("FileRecordQueue::WriteRecord: record too long");

            thread.AddRecord(record);
        }

        /// <summary>
        /// 取回count个记录
        /// </summary>
        /// <param name="count">待取回的记录数</param>
        /// <param name="remove">是否同时移除记录</param>
        /// <returns>按顺序排列的记录列表，返回的记录数可能少于count</returns>
        public List<string> FetchRecord(int count, bool remove = false)
        {
            if (fileStream == null)
                throw new Exception("FileRecordQueue::FetchRecord: file not open");

            if (header.recCount == 0)
                return null;

            if (count <= 0)
                return null;

            List<string> ret = new List<string>();
            lock (thread)
            {
                int recfirst = header.recFirst;
                int reccount = header.recCount;
                int maxn = 0;
                int savedCount = count;
                byte[] buf = new byte[header.recSize];
                while (reccount > 0 && count > 0)
                {
                    bool loop = true;
                    int n = 0;
                    Array.Clear(buf, 0, (maxn + 1) * header.recSize);

                    while (loop)
                    {
                        fileStream.Position = HEADER_SIZE + recfirst * header.recSize;
                        fileStream.Read(buf, n * header.recSize, header.recSize);

                        if (buf[(n + 1) * header.recSize - 1] == 0)
                        {
                            int i = n * header.recSize;
                            while (buf[i] != 0)
                                i++;

                            ret.Add(Encoding.UTF8.GetString(buf, 0, i));
                            loop = false;
                        }
                        else
                        {
                            n++;
                            if (maxn < n)
                            {
                                maxn = n;
                                Array.Resize(ref buf, header.recSize * (n + 1));
                            }
                        }

                        recfirst++;
                        if (recfirst >= header.recMax)
                            recfirst = 0;

                        if (loop && recfirst == header.recLast)
                            throw new Exception("FileRecordQueue::FetchRecord: loop record");
                    }

                    reccount--;
                    count--;
                }

                if (remove)
                {
                    header.recFirst = (short)recfirst;
                    header.recCount = (short)reccount;
                    if (header.recCount == 0)
                        header.recFirst = header.recLast = 0;
                    SyncHeader();
                }
            }
            return ret;
        }

        /// <summary>
        /// 删除最早的count个记录
        /// </summary>
        /// <param name="count"></param>
        public void RemoveRecord(int count)
        {
            if (fileStream == null)
                throw new Exception("FileRecordQueue::RemoveRecord: file not open");

            lock (thread)
            {
                for (int i = 0; i < count; i++)
                    RemoveOne();
                if (header.recCount == 0)
                    header.recFirst = header.recLast = 0;
                SyncHeader();
            }
        }

        /// <summary>
        /// 取回文件中当前有效的记录数
        /// </summary>
        /// <returns></returns>
        public int RecordCount()
        {
            return header.recCount;
        }

        /// <summary>
        /// 取回文件的当前记录容量，用于判断文件是否超过特定的大小
        /// </summary>
        /// <returns></returns>
        public int Capacity()
        {
            lock (thread)
            {
                return ((int)fileStream.Length - HEADER_SIZE) / (int)header.recSize;
            }
        }


        /// <summary>
        /// 删除所有记录，重置文件大小
        /// </summary>
        public void ClearAll()
        {
            if (fileStream == null)
                throw new Exception("FileRecordQueue::ClearAll: file not open");

            lock (thread)
            {
                header.recCount = 0;
                header.recFirst = 0;
                header.recLast = 0;
                SyncHeader();

                fileStream.SetLength(HEADER_SIZE);
            }
        }

        /// <summary>
        /// 删除记录文件
        /// </summary>
        public void RemoveFile()
        {
            if (fileStream != null)
            {
                lock (thread)
                {
                    fileStream.Close();
                    fileStream = null;
                    File.Delete(fileName);
                }
            }
        }
    }
}
