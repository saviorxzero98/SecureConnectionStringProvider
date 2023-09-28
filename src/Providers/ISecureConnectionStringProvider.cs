using System.Security;

namespace SecureConnectionString.Providers
{
    public interface ISecureConnectionStringProvider
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="securePlainText"></param>
        /// <returns></returns>
        string Encrypt(SecureString securePlainText);

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="secretText"></param>
        /// <returns></returns>
        SecureString Decrypt(string secretText);
    }
}
