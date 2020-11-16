﻿using System;
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
    public class LocalStorage
    {
        private Dictionary<string, object> _data;
        private string _dataStr;
        private readonly string _fileName;
        private readonly ReaderWriterLockSlim _rwLocks = new ReaderWriterLockSlim();
        private readonly object _saveLock = new object();
        private bool _isDirty;
    
        public LocalStorage(string fileName)
        {
            _fileName = fileName;
            InitGameData();
        }
    
        ~LocalStorage()
        {
            _rwLocks.Dispose();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitGameData()
        {
            if (_data == null)
            {
                byte[] dataBytes = null;
                if (File.Exists(_fileName))
                {
                    using (var fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read))
                    {
                        try
                        {
                            dataBytes = new byte[fs.Length];
                            fs.Read(dataBytes, 0, (int)fs.Length);
                        }
                        catch (Exception e)
                        {
                            DebugEx.Error(e.Message);
                        }
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
        /// <typeparam name="T">类型</typeparam>
        /// <returns>数据</returns>
        public T GetValue<T>(string k)
        {
            _rwLocks.EnterReadLock();
            try
            {
                if (_data.TryGetValue(k, out var v))
                {
                    return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(v));
                }
                return default;
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
            SaveGameDataInternal();
        }

        /// <summary>
        /// 异步线程将数据写入磁盘
        /// </summary>
        public void SaveAsync()
        {
            if (!_isDirty) return;
            _isDirty = false;
            Task.Run(SaveGameDataInternal);
        }

        private void SaveGameDataInternal()
        {
            lock (_saveLock)
            {
                using (var fs = new FileStream(_fileName, FileMode.Create))
                {
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

                        var bytes = Encoding.UTF8.GetBytes(_dataStr);
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Flush();
                    }
                    catch (Exception e)
                    {
                        DebugEx.Error(e.Message);
                    }
                }
            }
        }
    }
}
