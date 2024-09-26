namespace SCNS.EncryptionProviders
{
    public interface IStringEncryptionProvider
    {
        /// <summary>
        /// 加密字串
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="encryptionKey">加密金鑰</param>
        /// <returns>密文，為 Hex 字串</returns>
        string Encrypt(string plainText, IStringEncryptionKey? encryptionKey = null);

        /// <summary>
        /// 解密字串 
        /// </summary>
        /// <param name="cipherText">密文，僅支援 Hex 字串，不支援 Base 64 字串</param>
        /// <param name="encryptionKey">加密金鑰</param>
        /// <returns>明文</returns>
        string Decrypt(string cipherText, IStringEncryptionKey? encryptionKey = null);
    }

    public interface IStringEncryptionKey
    {
        /// <summary>
        /// 選項
        /// </summary>
        Dictionary<string, string> Properties { get; set; }
    }
}
