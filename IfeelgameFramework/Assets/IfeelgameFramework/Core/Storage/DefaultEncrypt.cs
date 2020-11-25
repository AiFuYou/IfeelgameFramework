using System;
using System.Security.Cryptography;
using System.Text;

namespace IfeelgameFramework.Core.Storage
{
    public class DefaultEncrypt : IDisposable, IEncrypt
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;
        
        private RijndaelManaged _cipher;
        
        public DefaultEncrypt(byte[] key = null, byte[] iv = null)
        {
            _key = key ?? Encoding.ASCII.GetBytes("Ifeelgame1234567");
            _iv = iv ?? Encoding.ASCII.GetBytes("1234567Ifeelgame");

            CheckKey(_key);
            CheckIV(_iv);
            
            _cipher = new RijndaelManaged()
            {
                Mode = CipherMode.CBC,//use CBC
                Padding = PaddingMode.PKCS7,//default PKCS7
                KeySize = 128,//default 256
                BlockSize = 128,//default 128
                FeedbackSize = 128//default 128
            };
        }
        
        public string Encode(string s)
        {
            var en = _cipher.CreateEncryptor(_key, _iv);

            var eBytes = Encoding.UTF8.GetBytes(s);
            var enBytes = en.TransformFinalBlock(eBytes, 0, eBytes.Length);
            
            en.Dispose();
            
            return Convert.ToBase64String(enBytes);
        }

        public string Decode(string s)
        {
            var de = _cipher.CreateDecryptor(_key, _iv);

            var dBytes = Convert.FromBase64String(s);
            var deBytes = de.TransformFinalBlock(dBytes, 0, dBytes.Length);
            
            de.Dispose();
            
            return Encoding.UTF8.GetString(deBytes);
        }

        private void CheckKey(byte[] key)
        {
            if (key == null || (key.Length != 16 && key.Length != 24 && key.Length != 32))
                throw new ArgumentException("The 'Key' must be 16byte 24byte or 32byte!");
        }

        private void CheckIV(byte[] iv)
        {
            if (iv == null || iv.Length != 16)
                throw new ArgumentException("The 'IV' must be 16byte!");
        }

        private bool _disposed;
        ~DefaultEncrypt()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                Array.Clear(_key, 0, _key.Length);
                Array.Clear(_iv, 0, _iv.Length);
            }
            
            _cipher.Dispose();

            _disposed = true;
        }
    }
}
