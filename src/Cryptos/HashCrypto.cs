using System.Security.Cryptography;
using System.Text;

namespace SecureConnectionString.Cryptos
{
    public class HashCrypto
    {
        public HashAlgorithmType Algorithm { get; set; } = HashAlgorithmType.SHA256;
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public bool UrlEncodeFlag { get; set; } = false;

        protected Base64Encoder Base64
        {
            get
            {
                return new Base64Encoder()
                {
                    UrlEncodeFlag = UrlEncodeFlag,
                    Encoding = Encoding
                };
            }
        }

        public HashCrypto(HashAlgorithmType algorithm)
        {
            Algorithm = algorithm;
        }

        /// <summary>
        ///  Hash Encode
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public string Encode(string plainText)
        {
            if (plainText == null)
            {
                throw new ArgumentNullException(nameof(plainText));
            }

            // Hash
            HashAlgorithm algorithm = GetHashAlgorithm(Algorithm);
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            byte[] crypto = algorithm.ComputeHash(bytes);

            // To Base64
            return Base64.ToBase64String(crypto);
        }

        /// <summary>
        ///  Hash Encode
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public string EncodeToHexString(string plainText)
        {
            if (plainText == null)
            {
                throw new ArgumentNullException(nameof(plainText));
            }

            // Hash
            HashAlgorithm algorithm = GetHashAlgorithm(Algorithm);
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            byte[] crypto = algorithm.ComputeHash(bytes);

            // To Base64
            return HexEncoder.Instance.ToHexString(crypto);
        }

        /// <summary>
        /// Vaildate Hash
        /// </summary>
        /// <param name="vaildateText"></param>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public bool Vaildate(string vaildateText, string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentNullException(nameof(plainText));
            }
            if (string.IsNullOrEmpty(vaildateText))
            {
                throw new ArgumentNullException(nameof(vaildateText));
            }

            string cipherText = Encode(plainText);
            return cipherText.Equals(vaildateText);
        }

        /// <summary>
        /// Get Hash Algorithm
        /// </summary>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        protected HashAlgorithm GetHashAlgorithm(HashAlgorithmType algorithm)
        {
            switch (algorithm)
            {
                case HashAlgorithmType.SHA1:
                    return SHA1.Create();
                case HashAlgorithmType.SHA256:
                    return SHA256.Create();
                case HashAlgorithmType.SHA384:
                    return SHA384.Create();
                case HashAlgorithmType.SHA512:
                    return SHA512.Create();
                case HashAlgorithmType.MD5:
                default:
                    return MD5.Create();
            }
        }
    }
}
