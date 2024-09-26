using System.Security.Cryptography;
using System.Text;

namespace SecureConnectionString.Cryptos
{
    public class SymmetricEncryptionHelper
    {
        /// <summary>
        /// Encoding
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Mode
        /// </summary>
        public CipherMode Mode { get; set; } = CipherMode.CBC;

        /// <summary>
        /// Padding
        /// </summary>
        public PaddingMode Padding { get; set; } = PaddingMode.PKCS7;
        
        /// <summary>
        /// 密文是否為 Base64 字串
        /// </summary>
        public bool IsBase64Cipher { get; set; } = false;

        public SymmetricEncryptionHelper(CipherMode mode = CipherMode.CBC,
                                         PaddingMode padding = PaddingMode.PKCS7,
                                         bool isBase64Cipher = false)
        {
            Encoding = Encoding.UTF8;
            Mode = mode;
            Padding = padding;
            IsBase64Cipher = isBase64Cipher;
        }

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public string Encrypt(string plainText, byte[] key, byte[] iv)
        {
            try
            {
                using (var aesAlg = Aes.Create())
                {
                    aesAlg.Mode = Mode;
                    aesAlg.Padding = Padding;
                    byte[] plain = Encoding.GetBytes(plainText);

                    using (MemoryStream memoryStream = new MemoryStream())
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                                        aesAlg.CreateEncryptor(key, iv),
                                                                        CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plain, 0, plain.Length);
                        cryptoStream.FlushFinalBlock();
                        var cipherBytes = memoryStream.ToArray();

                        if (IsBase64Cipher)
                        {
                            return Base64Convert.Create().ToBase64String(cipherBytes);
                        }
                        else
                        {
                            return HexConvert.Create().ToHexString(cipherBytes);
                        }
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public string Decrypt(string cipherText, byte[] key, byte[] iv)
        {
            try
            {
                using (var aesAlg = Aes.Create())
                {
                    aesAlg.Mode = Mode;
                    aesAlg.Padding = Padding;

                    byte[] cipherBytes;
                    if (IsBase64Cipher)
                    {
                        cipherBytes = Base64Convert.Create().FromBase64String(cipherText);
                    }
                    else
                    {
                        cipherBytes = HexConvert.Create().FromHexString(cipherText);
                    }

                    using (MemoryStream memoryStream = new MemoryStream())
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                                        aesAlg.CreateDecryptor(key, iv),
                                                                        CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                        cryptoStream.FlushFinalBlock();

                        return Encoding.GetString(memoryStream.ToArray());

                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
