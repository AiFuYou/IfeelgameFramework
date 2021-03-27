using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IfeelgameFramework.Core.Logger;
using Newtonsoft.Json;

namespace IfeelgameFramework.Core.Storage
{
    /// <summary>
    /// 数据存储类
    /// </summary>
    public class LocalStorage : IDisposable
    {
        private Dictionary<string, object> _data;
        private string _dataStr;
        private readonly string _fileName;
        private readonly ReaderWriterLockSlim _rwLocks = new ReaderWriterLockSlim();
        private readonly object _saveLock = new object();
        private bool _isDirty;
        private IEncrypt _encryptor;
    
        public LocalStorage(string fileName, IEncrypt encryptor)
        {
            _fileName = fileName;
            _encryptor = encryptor;
            InitData();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitData()
        {
            if (_data == null)
            {
                byte[] dataBytes = null;
                if (File.Exists(_fileName))
                {
                    using var fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read);
                    try
                    {
                        var bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, (int)fs.Length);

                        dataBytes = Encoding.UTF8.GetBytes(_encryptor.Decode(Encoding.UTF8.GetString(bytes)));
                    }
                    catch (Exception e)
                    {
                        DebugEx.Error(e.Message);
                    }
                }

                if (dataBytes != null && dataBytes.Length > 0)
                {
                    _data = JsonConvert.DeserializeObject<Dictionary<string, object>>(Encoding.UTF8.GetString(dataBytes));
                }
                else
                {
                    _data = new Dictionary<string, object>();
                }
            }
        }

        /// <summary>
        /// 根据key和类型获取存储值，缺省为当前数据类型默认值
        /// </summary>
        /// <param name="k">key</param>
        /// <param name="defaultValue">如果未存储过该数据，则返回defaultValue</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>数据</returns>
        public T GetValue<T>(string k, T defaultValue = default)
        {
            _rwLocks.EnterReadLock();
            try
            {
                if (_data.TryGetValue(k, out var v))
                {
                    return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(v));
                }
                return defaultValue;
            }
            finally
            {
                _rwLocks.ExitReadLock();
            }
        }

        /// <summary>
        /// 根据key设置数据，并决定是否立即存储
        /// </summary>
        /// <param name="k">key</param>
        /// <param name="v">value</param>
        /// <param name="needSave">是否立即写入磁盘</param>
        /// <param name="isAsyncSave">是否异步存入磁盘</param>
        public void SetValue(string k, object v, bool needSave = false, bool isAsyncSave = false)
        {
            _rwLocks.EnterWriteLock();
            try
            {
                _data[k] = v;
                _isDirty = true;
            }
            finally
            {
                _rwLocks.ExitWriteLock();
            }
            
            if (needSave)
            {
                if (isAsyncSave)
                {
                    SaveAsync();
                }
                else
                {
                    Save();
                }
            }
        }

        /// <summary>
        /// 同步方法将数据写入磁盘
        /// </summary>
        public void Save()
        {
            if (!_isDirty) return;
            _isDirty = false;
            SaveInternal();
        }

        /// <summary>
        /// 异步线程将数据写入磁盘
        /// </summary>
        public void SaveAsync()
        {
            if (!_isDirty) return;
            _isDirty = false;
            Task.Run(SaveInternal);
        }

        private void SaveInternal()
        {
            lock (_saveLock)
            {
                using var fs = new FileStream(_fileName, FileMode.Create);
                try
                {
                    _rwLocks.EnterReadLock();
                    try
                    {
                        _dataStr = JsonConvert.SerializeObject(_data);
                    }
                    finally
                    {
                        _rwLocks.ExitReadLock();
                    }

                    var bytes = Encoding.UTF8.GetBytes(_encryptor.Encode(_dataStr));
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                }
                catch (Exception e)
                {
                    DebugEx.Error(e.Message);
                }
            }
        }
        
        ~LocalStorage()
        {
            Dispose(false);
        }

        private bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _data.Clear();
            }
            
            _rwLocks.Dispose();

            _disposed = true;
        }

        /// <summary>
        /// 当使用异步存储功能时，请谨慎使用
        /// </summary>
        public void ClearData()
        {
            _data.Clear();
            SaveInternal();
        }
    }
}
