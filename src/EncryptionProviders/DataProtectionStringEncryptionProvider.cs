using Microsoft.AspNetCore.DataProtection;
using SecureConnectionString.Cryptos;
using System.Text;

namespace SCNS.EncryptionProviders
{
    public class DataProtectionStringEncryptionProvider : IStringEncryptionProvider
    {
        private readonly IDataProtector _dataProtector;
        
        public Encoding Encoding { get; set; }

        public DataProtectionStringEncryptionProvider(IDataProtector dataProtector)
        {
            _dataProtector = dataProtector;
            Encoding = Encoding.UTF8;
        }
        public DataProtectionStringEncryptionProvider(IDataProtector dataProtector, Encoding encoding)
        {
            _dataProtector = dataProtector;
            Encoding = encoding;
        }


        /// <summary>
        /// 加密字串
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="encryptionKey">加密金鑰</param>
        /// <returns>密文，為 Hex 字串</returns>
        public string Encrypt(string plainText, IStringEncryptionKey? encryptionKey = null)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return string.Empty;
            }

            var textBytes = Encoding.GetBytes(plainText);
            var secretBytes = _dataProtector.Protect(textBytes);
            return HexConvert.Create().ToHexString(secretBytes);
        }

        /// <summary>
        /// 解密字串 
        /// </summary>
        /// <param name="cipherText">密文，僅支援 Hex 字串，不支援 Base 64 字串</param>
        /// <param name="encryptionKey">加密金鑰</param>
        /// <returns>明文</returns>
        public string Decrypt(string cipherText, IStringEncryptionKey? encryptionKey = null)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return string.Empty;
            }

            try
            {
                var secretBytes = HexConvert.Create().FromHexString(cipherText);
                var plainBytes = _dataProtector.Unprotect(secretBytes);
                return Encoding.GetString(plainBytes);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
