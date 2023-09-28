using SecureConnectionString.Cryptos;
using SecureConnectionString.Extensions;
using SecureConnectionString.Providers;
using System.Security;

namespace SecureConnectionString.Test.Providers
{
    public class CustomSecureConnectionStringProvider : ISecureConnectionStringProvider
    {
        private const string PrivateSecret = "b1047f05-c921-4564-85e2-44221602df82";
        private const string PublicSecret = "cd8846c6-22b9-4e6f-9d59-f05ff27567d7";

        private readonly SecureString Key;
        private readonly SecureString Iv;


        public CustomSecureConnectionStringProvider()
        {
            Key = PrivateSecret.ToSecureString();
            Iv = PublicSecret.ToSecureString();
        }
        public CustomSecureConnectionStringProvider(string key, string iv)
        {
            Key = key.ToSecureString();
            Iv = iv.ToSecureString();
        }
        public CustomSecureConnectionStringProvider(SecureString key, SecureString iv)
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
