using SCNS.EncryptionProviders;

namespace SCNS
{
    public interface IConnectionStringProtectionProvider
    {
        /// <summary>
        /// 加密連線字串
        /// </summary>
        /// <param name="connectionString">連線字串</param>
        /// <returns>密文，為 Hex 字串</returns>
        string Protect(string connectionString);

        /// <summary>
        /// 解密連線字串
        /// </summary>
        /// <param name="secureConnectionString">已加密的連線字串</param>
        /// <returns>連線字串</returns>
        string Unprotect(string secureConnectionString);
    }
}