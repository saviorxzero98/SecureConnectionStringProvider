using SecureConnectionString.Extensions;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace SecureConnectionString.Cryptos
{
    public class SymmetricCrypto
    {
        public SymmetricAlgorithmType Algorithm { get; set; }
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public CipherMode Mode { get; set; } = CipherMode.CBC;
        public PaddingMode Padding { get; set; } = PaddingMode.PKCS7;
        public EncryptedStringFormat StringFormat { get; set; } = EncryptedStringFormat.Base64;

        public SymmetricCrypto()
        {
            Algorithm = SymmetricAlgorithmType.AES;
            Padding = PaddingMode.PKCS7;
            Mode = CipherMode.CBC;
            Encoding = Encoding.UTF8;
            StringFormat = EncryptedStringFormat.Base64;
        }
        public SymmetricCrypto(SymmetricAlgorithmType algorithm)
        {
            Algorithm = algorithm;
            Padding = PaddingMode.PKCS7;
            Mode = CipherMode.CBC;
            Encoding = Encoding.UTF8;
            StringFormat = EncryptedStringFormat.Base64;
        }

        /// <summary>
        /// Generate Key
        /// </summary>
        /// <returns></returns>
        public SymmetricCryptoKey GenerateKey()
        {
            SymmetricAlgorithm provider = CreateEncryptAlgorithm(Algorithm, Mode, Padding);
            provider.GenerateKey();
            provider.GenerateIV();
            return new SymmetricCryptoKey(provider.Key, provider.IV);
        }

        #region Encrypt

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public string Encrypt(string plainText, byte[] key, byte[] iv)
        {
            return Encrypt(plainText.ToSecureString(), new SymmetricCryptoKey(key, iv));
        }
        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="securePlainText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public string Encrypt(SecureString securePlainText, byte[] key, byte[] iv)
        {
            return Encrypt(securePlainText, new SymmetricCryptoKey(key, iv));
        }
        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="base64Key"></param>
        /// <param name="base64Iv"></param>
        /// <returns></returns>
        public string Encrypt(string plainText, string base64Key, string base64Iv)
        {
            return Encrypt(plainText.ToSecureString(), new SymmetricCryptoKey(base64Key, base64Iv));
        }
        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="securePlainText"></param>
        /// <param name="base64Key"></param>
        /// <param name="base64Iv"></param>
        /// <returns></returns>
        public string Encrypt(SecureString securePlainText, string base64Key, string base64Iv)
        {
            return Encrypt(securePlainText, new SymmetricCryptoKey(base64Key, base64Iv));
        }
        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="securePlainText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Encrypt(string plainText, SymmetricCryptoKey key)
        {
            return Encrypt(plainText.ToSecureString(), key);
        }
        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="securePlainText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Encrypt(SecureString securePlainText, SymmetricCryptoKey key)
        {
            try
            {
                SymmetricAlgorithm provider = CreateEncryptAlgorithm(Algorithm, Mode, Padding);
                provider.Key = key.Key;
                provider.IV = key.IV;
                byte[] plain = Encoding.GetBytes(securePlainText.ToPlainString());

                ICryptoTransform desencrypt = provider.CreateEncryptor();
                byte[] bytes = desencrypt.TransformFinalBlock(plain, 0, plain.Length);

                return ToEncryptedString(bytes);
            }
            catch
            {
                return securePlainText.ToPlainString();
            }
        }

        #endregion


        #region Decrypt

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public SecureString Decrypt(string cipherText, byte[] key, byte[] iv)
        {
            return Decrypt(cipherText, new SymmetricCryptoKey(key, iv));
        }
        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="base64Key"></param>
        /// <param name="base64Iv"></param>
        /// <returns></returns>
        public SecureString Decrypt(string cipherText, string base64Key, string base64Iv)
        {
            return Decrypt(cipherText, new SymmetricCryptoKey(base64Key, base64Iv));
        }
        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public SecureString Decrypt(string cipherText, SymmetricCryptoKey key)
        {
            try
            {
                SymmetricAlgorithm provider = CreateEncryptAlgorithm(Algorithm, Mode, Padding);
                provider.Key = key.Key;
                provider.IV = key.IV;

                byte[] cipher = FromEncryptedString(cipherText);

                ICryptoTransform desencrypt = provider.CreateDecryptor();
                byte[] bytes = desencrypt.TransformFinalBlock(cipher, 0, cipher.Length);

                return Encoding.GetString(bytes).ToSecureString();
            }
            catch
            {
                return cipherText.ToSecureString();
            }
        }

        #endregion


        /// <summary>
        /// Create Crypto Algorithm
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="mode"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        protected SymmetricAlgorithm CreateEncryptAlgorithm(SymmetricAlgorithmType algorithm, CipherMode mode, PaddingMode padding)
        {
            switch (algorithm)
            {
                case SymmetricAlgorithmType.Rijndael:
                    var rijndaelAlg = Rijndael.Create();
                    rijndaelAlg.Mode = mode;
                    rijndaelAlg.Padding = padding;
                    return rijndaelAlg;

                case SymmetricAlgorithmType.RC2:
                    var rc2Alg = RC2.Create();
                    rc2Alg.Mode = mode;
                    rc2Alg.Padding = padding;
                    return rc2Alg;

                case SymmetricAlgorithmType.DES:
                    var desAlg = DES.Create();
                    desAlg.Mode = mode;
                    desAlg.Padding = padding;
                    return desAlg;

                case SymmetricAlgorithmType.TripleDES:
                    var tripleDesAlg = TripleDES.Create();
                    tripleDesAlg.Mode = mode;
                    tripleDesAlg.Padding = padding;
                    return tripleDesAlg;

                case SymmetricAlgorithmType.AES:
                default:
                    var aesAlg = Aes.Create();
                    aesAlg.Mode = mode;
                    aesAlg.Padding = padding;
                    return aesAlg;
            }
        }

        /// <summary>
        /// Bytes轉字串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        protected string ToEncryptedString(byte[] bytes)
        {
            switch (StringFormat)
            {
                case EncryptedStringFormat.Hex:
                    return HexEncoder.Instance.ToHexString(bytes);

                case EncryptedStringFormat.Base64:
                default:
                    return Base64Encoder.Instance.ToBase64String(bytes);
            }
        }

        /// <summary>
        /// 字串轉Bytes
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        protected byte[] FromEncryptedString(string cipherText)
        {
            switch (StringFormat)
            {
                case EncryptedStringFormat.Hex:
                    return HexEncoder.Instance.FromHexString(cipherText);

                case EncryptedStringFormat.Base64:
                default:
                    return Base64Encoder.Instance.FromBase64String(cipherText);
            }
        }
    }

    public class SymmetricCryptoKey
    {
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }

        public SymmetricCryptoKey()
        {

        }
        public SymmetricCryptoKey(byte[] key, byte[] iv)
        {
            Key = key;
            IV = iv;
        }
        public SymmetricCryptoKey(string base64Key, string base64Iv)
        {
            Key = Convert.FromBase64String(base64Key);
            IV = Convert.FromBase64String(base64Iv);
        }

        public string Base64Key
        {
            get
            {
                return Convert.ToBase64String(Key);
            }
        }

        public string Base64IV
        {
            get
            {
                return Convert.ToBase64String(IV);
            }
        }
    }
}
