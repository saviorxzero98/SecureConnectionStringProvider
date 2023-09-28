using SecureConnectionString.Cryptos;
using SecureConnectionString.Extensions;
using System.Security;

namespace SecureConnectionString.Providers
{
    public class DefaultSecureConnectionStringProvider : ISecureConnectionStringProvider
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
        private readonly SecureString Iv;


        public DefaultSecureConnectionStringProvider()
        {
            Key = PrivateSecret.ToSecureString();
            Iv = PublicSecret.ToSecureString();
        }
        public DefaultSecureConnectionStringProvider(string key, string iv)
        {
            Key = key.ToSecureString();
            Iv = iv.ToSecureString();
        }
        public DefaultSecureConnectionStringProvider(SecureString key, SecureString iv)
        {
            Key = key;
            Iv = iv;
        }


        public string Encrypt(SecureString securePlainText)
        {
            var crypto = new SymmetricCrypto(SymmetricAlgorithmType.AES)
            {
                StringFormat = EncryptedStringFormat.Hex
            };
            string key = GetHash(Key.ToPlainString());
            string iv = GetHash(Iv.ToPlainString());
            return crypto.Encrypt(securePlainText, key, iv);
        }

        public SecureString Decrypt(string secretText)
        {
            var crypto = new SymmetricCrypto(SymmetricAlgorithmType.AES)
            {
                StringFormat = EncryptedStringFormat.Hex
            };
            string key = GetHash(Key.ToPlainString());
            string iv = GetHash(Iv.ToPlainString());
            return crypto.Decrypt(secretText, key, iv);
        }

        private string GetHash(string plainText)
        {
            var crypto = new HashCrypto(HashAlgorithmType.MD5);

            return crypto.Encode(plainText);
        }
    }
}
