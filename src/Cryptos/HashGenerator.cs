using System.Security.Cryptography;
using System.Text;

namespace SecureConnectionString.Cryptos
{
    public class HashGenerator
    {
        public enum AlgorithmType { MD5, SHA1, SHA256, SHA384, SHA512 }

        public AlgorithmType Algorithm { get; set; } = AlgorithmType.SHA256;
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public bool UrlEncodeFlag { get; set; } = false;

        protected Base64Convert Base64
        {
            get
            {
                return new Base64Convert()
                {
                    UrlEncodeFlag = UrlEncodeFlag,
                    Encoding = Encoding
                };
            }
        }

        public HashGenerator(AlgorithmType algorithm)
        {
            Algorithm = algorithm;
        }

        /// <summary>
        /// Create Hash
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public byte[] Create(string plainText)
        {
            if (plainText == null)
            {
                throw new ArgumentNullException(nameof(plainText));
            }

            // Hash
            HashAlgorithm algorithm = GetHashAlgorithm(Algorithm);
            byte[] textBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] hashBytes = algorithm.ComputeHash(textBytes);

            return hashBytes;
        }

        /// <summary>
        /// Create Hash + Salt
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="saltText"></param>
        /// <returns></returns>
        public byte[] Create(string plainText, string saltText)
        {
            if (plainText == null)
            {
                throw new ArgumentNullException(nameof(plainText));
            }

            if (string.IsNullOrEmpty(saltText))
            {
                return Create(plainText);
            }

            // Hash
            using (var algorithm = GetHashAlgorithm(Algorithm))
            {
                byte[] textBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] saltBytes = Encoding.UTF8.GetBytes(saltText);
                byte[] saltedTextBytes = new byte[textBytes.Length + saltText.Length];

                // Concatenate password and salt
                Buffer.BlockCopy(textBytes, 0, saltedTextBytes, 0, textBytes.Length);
                Buffer.BlockCopy(saltBytes, 0, saltedTextBytes, textBytes.Length, saltBytes.Length);

                // Hash the concatenated password and salt
                byte[] hashedBytes = algorithm.ComputeHash(saltedTextBytes);

                // Concatenate the salt and hashed password for storage
                byte[] hashedBytesWithSalt = new byte[hashedBytes.Length + saltBytes.Length];
                Buffer.BlockCopy(saltBytes, 0, hashedBytesWithSalt, 0, saltBytes.Length);
                Buffer.BlockCopy(hashedBytes, 0, hashedBytesWithSalt, saltBytes.Length, hashedBytes.Length);

                return hashedBytesWithSalt;
            }
        }

        /// <summary>
        /// Create Hash
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public string CreateToBase64(string plainText)
        {
            byte[] hashBytes = Create(plainText);
            return Base64.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Create Hash + Salt
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="saltText"></param>
        /// <returns></returns>
        public string CreateToBase64String(string plainText, string saltText)
        {
            var hashBytes = Create(plainText, saltText);
            return Base64Convert.Create().ToBase64String(hashBytes);
        }

        /// <summary>
        /// Create Hash
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public string CreateToHexString(string plainText)
        {
            byte[] hashBytes = Create(plainText);
            return HexConvert.Create().ToHexString(hashBytes);
        }

        /// <summary>
        /// Create Hash + Salt
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="saltText"></param>
        /// <returns></returns>
        public string CreateToHexString(string plainText, string saltText)
        {
            var hashBytes = Create(plainText, saltText);
            return HexConvert.Create().ToHexString(hashBytes);
        }

        /// <summary>
        /// Get Hash Algorithm
        /// </summary>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        protected HashAlgorithm GetHashAlgorithm(AlgorithmType algorithm)
        {
            switch (algorithm)
            {
                case AlgorithmType.SHA1:
                    return SHA1.Create();
                case AlgorithmType.SHA256:
                    return SHA256.Create();
                case AlgorithmType.SHA384:
                    return SHA384.Create();
                case AlgorithmType.SHA512:
                    return SHA512.Create();
                case AlgorithmType.MD5:
                default:
                    return MD5.Create();
            }
        }
    }
}
