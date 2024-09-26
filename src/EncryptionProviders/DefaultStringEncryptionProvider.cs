using SecureConnectionString.Cryptos;
using SecureConnectionString.Extensions;
using System.Security;
using static SecureConnectionString.Cryptos.HashGenerator;

namespace SCNS.EncryptionProviders
{
    public class DefaultStringEncryptionProvider : IStringEncryptionProvider
    {
        private string PrivateSecret
        {
            get
            {
                return Environment.MachineName;
            }
        }
        private string PublicSecret
        {
            get
            {
                return GetType().Assembly.GetName().Name;
            }
        }

        private readonly SecureString Key;
        private readonly SecureString InitVector;


        public DefaultStringEncryptionProvider()
        {
            Key = PrivateSecret.ToSecureString();
            InitVector = PublicSecret.ToSecureString();
        }
        public DefaultStringEncryptionProvider(string key, string iv)
        {
            Key = key.ToSecureString();
            InitVector = iv.ToSecureString();
        }
        public DefaultStringEncryptionProvider(SecureString key, SecureString iv)
        {
            Key = key;
            InitVector = iv;
        }


        /// <summary>
        /// 加密字串
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="encryptionKey">加密金鑰</param>
        /// <returns>密文，為 Hex 字串</returns>
        public string Encrypt(string plainText, IStringEncryptionKey? encryptionKey = null)
        {
            var helper = new SymmetricEncryptionHelper();

            var sha256 = new HashGenerator(AlgorithmType.SHA256);
            var md5 = new HashGenerator(AlgorithmType.MD5);

            byte[] key = sha256.Create(Key.ToPlainString());
            byte[] iv = md5.Create(InitVector.ToPlainString());
            return helper.Encrypt(plainText, key, iv);
        }

        /// <summary>
        /// 解密字串 
        /// </summary>
        /// <param name="cipherText">密文，僅支援 Hex 字串，不支援 Base 64 字串</param>
        /// <param name="encryptionKey">加密金鑰</param>
        /// <returns>明文</returns>
        public string Decrypt(string cipherText, IStringEncryptionKey? encryptionKey = null)
        {
            var helper = new SymmetricEncryptionHelper();

            var sha256 = new HashGenerator(AlgorithmType.SHA256);
            var md5 = new HashGenerator(AlgorithmType.MD5);

            byte[] key = sha256.Create(Key.ToPlainString());
            byte[] iv = md5.Create(InitVector.ToPlainString());
            return helper.Decrypt(cipherText, key, iv);
        }
    }
}
