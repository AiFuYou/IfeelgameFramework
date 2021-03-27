using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IfeelgameFramework.Core.Storage
{
    /// <summary>
    /// 本地数据存储
    /// </summary>
    public class LocalStorageManager
    {
        private static readonly string FileRootPath = Path.Combine(Application.persistentDataPath, "localstorage");
        private Dictionary<string, LocalStorage> _localStorages = null;
        private LocalStorage _defaultLocalStorage = null;
        private readonly string _defaultFileName = "gameDefault.dat";
        private readonly object _dLock = new object();
        private readonly DefaultEncrypt _encryptor = new DefaultEncrypt();
        private LocalStorage DefaultLocalStorage
        {
            get
            {
                lock (_dLock)
                {
                    if (_defaultLocalStorage == null)
                    {
                        _defaultLocalStorage = new LocalStorage(Path.Combine(FileRootPath, _defaultFileName), _encryptor);
                    }
                }
                return _defaultLocalStorage;
            }
        }

        private static LocalStorageManager _instance;
        private static readonly object _insLock = new object();
        /// <summary>
        /// 实例化LocalStorageManager单例，并创建存储文件夹
        /// </summary>
        public static LocalStorageManager Instance
        {
            get
            {
                lock (_insLock)
                {
                    if (_instance == null)
                    {
                        if (!Directory.Exists(FileRootPath))
                        {
                            Directory.CreateDirectory(FileRootPath);
                        }
                        _instance = new LocalStorageManager();
                    }
                }

                return _instance;
            }
        }
    
        private LocalStorageManager()
        {
            if (_localStorages == null)
            {
                _localStorages = new Dictionary<string, LocalStorage>();
            }
        }

        /// <summary>
        /// 获取存储对象
        /// </summary>
        /// <param name="fileName">存储文件名称</param>
        /// <param name="cache">是否把存储对象放入缓存，建议操作频率较高的数据存储对象缓存</param>
        /// <returns>存储对象</returns>
        public LocalStorage GetLocalStorage(string fileName, bool cache = false)
        {
            var ls = new LocalStorage(Path.Combine(FileRootPath, fileName), _encryptor);
            if (cache && !_localStorages.ContainsKey(fileName))
            {
                _localStorages[fileName] = ls;
            }
            return ls;
        }

        /// <summary>
        /// 移除存储对象
        /// </summary>
        /// <param name="fileName"></param>
        public void ReleaseLocalStorage(string fileName)
        {
            if (_localStorages.ContainsKey(fileName))
            {
                _localStorages.Remove(fileName);
            }
        }

        /// <summary>
        /// 移除所有存储对象
        /// </summary>
        public void ReleaseAllLocalStorages()
        {
            _localStorages.Clear();
        }

        /// <summary>
        /// 根据key存储value，存入默认存储文件
        /// </summary>
        /// <param name="k">key</param>
        /// <param name="v">value</param>
        /// <param name="needSave">是否立即写入磁盘</param>
        /// <param name="isAsyncSave">是否异步</param>
        public void SetValue(string k, object v, bool needSave = false, bool isAsyncSave = false)
        {
            DefaultLocalStorage.SetValue(k, v, needSave, isAsyncSave);
        }

        /// <summary>
        /// 根据key和类型获取存储值，从默认存储文件中获取
        /// </summary>
        /// <param name="k">key</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public T GetValue<T>(string k)
        {
            return DefaultLocalStorage.GetValue<T>(k);
        }

        /// <summary>
        /// 写入磁盘
        /// </summary>
        /// <param name="saveAll">是否所有存储写入磁盘</param>
        public void Save(bool saveAll = false)
        {
            _defaultLocalStorage?.Save();

            if (saveAll)
            {
                foreach (var item in _localStorages)
                {
                    item.Value.Save();
                }
            }
        }

        /// <summary>
        /// 异步线程写入磁盘
        /// </summary>
        /// <param name="saveAll">是否所有存储写入磁盘</param>
        public void SaveAsync(bool saveAll = false)
        {
            _defaultLocalStorage?.SaveAsync();
            
            if (saveAll)
            {
                foreach (var item in _localStorages)
                {
                    item.Value.SaveAsync();
                }
            }
        }

        /// <summary>
        /// 删除所有存储数据，并清除所有的存储对象，慎重使用
        /// </summary>
        public void ClearAllData()
        {
            if (!Directory.Exists(FileRootPath)) return;
            
            Directory.Delete(FileRootPath, true);
            Directory.CreateDirectory(FileRootPath);
            
            ReleaseAllLocalStorages();
            _defaultLocalStorage = null;
        }

        /// <summary>
        /// 删除指定存储数据，如果要删除存储对象，请确保不会再使用
        /// </summary>
        /// <param name="fileName">存储文件名</param>
        /// <param name="deleteLocalStorageObject">是否删除存储对象</param>
        public void ClearData(string fileName, bool deleteLocalStorageObject = false)
        {
            GetLocalStorage(fileName).ClearData();
            if (deleteLocalStorageObject)
            {
                ReleaseLocalStorage(fileName);
            }
        }

        /// <summary>
        /// 仅删除默认存储数据
        /// </summary>
        public void ClearData()
        {
            DefaultLocalStorage.ClearData();
        }
    }
}