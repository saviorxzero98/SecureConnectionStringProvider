using SCNS.EncryptionProviders;
using SecureConnectionString.Cryptos;
using SecureConnectionString.Extensions;
using System.Security;

namespace SecureConnectionString.Test.Providers
{
    public class CustomStringEncryptionProvider : IStringEncryptionProvider
    {
        private const string PrivateSecret = "b1047f05-c921-4564-85e2-44221602df82";
        private const string PublicSecret = "cd8846c6-22b9-4e6f-9d59-f05ff27567d7";

        private readonly SecureString Key;
        private readonly SecureString Iv;


        public CustomStringEncryptionProvider()
        {
            Key = PrivateSecret.ToSecureString();
            Iv = PublicSecret.ToSecureString();
        }
        public CustomStringEncryptionProvider(string key, string iv)
        {
            Key = key.ToSecureString();
            Iv = iv.ToSecureString();
        }
        public CustomStringEncryptionProvider(SecureString key, SecureString iv)
        {
            Key = key;
            Iv = iv;
        }


        /// <summary>
        /// 加密字串
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="encryptionKey">加密金鑰</param>
        /// <returns>密文，為 Hex 字串</returns>
        public string Encrypt(string plainText, IStringEncryptionKey? encryptionKey = null)
        {
            var crypto = new SymmetricEncryptionHelper();

            var sha256 = new HashGenerator(HashGenerator.AlgorithmType.SHA256);
            var md5 = new HashGenerator(HashGenerator.AlgorithmType.MD5);
            byte[] keyBytes = sha256.Create(Key.ToPlainString());
            byte[] ivBytes = md5.Create(Iv.ToPlainString());

            return crypto.Encrypt(plainText, keyBytes, ivBytes);
        }

        /// <summary>
        /// 解密字串 
        /// </summary>
        /// <param name="cipherText">密文，僅支援 Hex 字串，不支援 Base 64 字串</param>
        /// <param name="encryptionKey">加密金鑰</param>
        /// <returns>明文</returns>
        public string Decrypt(string cipherText, IStringEncryptionKey? encryptionKey = null)
        {
            var crypto = new SymmetricEncryptionHelper();
            var sha256 = new HashGenerator(HashGenerator.AlgorithmType.SHA256);
            var md5 = new HashGenerator(HashGenerator.AlgorithmType.MD5);
            byte[] keyBytes = sha256.Create(Key.ToPlainString());
            byte[] ivBytes = md5.Create(Iv.ToPlainString());

            return crypto.Decrypt(cipherText, keyBytes, ivBytes); ;
        }
    }
}
