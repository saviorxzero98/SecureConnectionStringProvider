using SecureConnectionString.Cryptos;

namespace SCNS.EncryptionProviders
{
    public class StringEncryptionProvider : IStringEncryptionProvider
    {
        /// <summary>
        /// Key
        /// </summary>
        protected string Key { get; set; }

        /// <summary>
        /// IV
        /// </summary>
        protected string InitVector { get; set; }

        public StringEncryptionProvider()
        {
            Key = Environment.MachineName;
            InitVector = GetType().Assembly.GetName().Name;
        }
        public StringEncryptionProvider(string key, string? initVector)
        {
            Key = key ?? Environment.MachineName;
            InitVector = initVector ?? GetType().Assembly.GetName().Name;
        }

        /// <summary>
        /// 加密字串
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="encryptionKey">加密金鑰</param>
        /// <returns>密文，為 Hex 字串</returns>
        public virtual string Encrypt(string plainText, IStringEncryptionKey? encryptionKey = null)
        {
            (byte[] key, byte[] iv) = GetKeyAndIvBytes(encryptionKey);
            var helper = new SymmetricEncryptionHelper(isBase64Cipher: false);
            return helper.Encrypt(plainText, key, iv);
        }

        /// <summary>
        /// 解密字串 
        /// </summary>
        /// <param name="cipherText">密文，僅支援 Hex 字串，不支援 Base 64 字串</param>
        /// <param name="encryptionKey">加密金鑰</param>
        /// <returns>明文</returns>
        public virtual string Decrypt(string cipherText, IStringEncryptionKey? encryptionKey = null)
        {
            (byte[] key, byte[] iv) = GetKeyAndIvBytes(encryptionKey);
            var helper = new SymmetricEncryptionHelper(isBase64Cipher: false);
            return helper.Decrypt(cipherText, key, iv);
        }

        /// <summary>
        /// 取得 Key 和 IV值
        /// </summary>
        /// <param name="encryptionKey"></param>
        /// <returns></returns>
        protected (byte[] key, byte[] iv) GetKeyAndIvBytes(IStringEncryptionKey? encryptionKey = null)
        {
            if (encryptionKey != null && encryptionKey.GetType() == typeof(StringEncryptionKey))
            {
                Key = ((StringEncryptionKey)encryptionKey).CryptoKey;
            }

            var sha256 = new HashGenerator(HashGenerator.AlgorithmType.SHA256);
            var md5 = new HashGenerator(HashGenerator.AlgorithmType.MD5);
            byte[] keyBytes = sha256.Create(Key);
            byte[] ivBytes = md5.Create(InitVector);

            return (key: keyBytes, iv: ivBytes);
        }
    }

    public class StringEncryptionKey : IStringEncryptionKey
    {
        public Dictionary<string, string> Properties { get; set; }

        public string CryptoKey
        {
            get
            {
                if (Properties != null && Properties.ContainsKey(nameof(CryptoKey)))
                {
                    return Properties[nameof(CryptoKey)];
                }
                return string.Empty;
            }
            set
            {
                if (Properties == null)
                {
                    Properties = new Dictionary<string, string>();
                }

                if (!Properties.TryAdd(nameof(CryptoKey), value))
                {
                    Properties[nameof(CryptoKey)] = value;
                }
            }
        }


        public StringEncryptionKey() 
        {
            Properties = new Dictionary<string, string>();
        }
        public StringEncryptionKey(string cryptoKey)
        {
            Properties = new Dictionary<string, string>();
            CryptoKey = cryptoKey;
        }
    }
}
