using System.Security;

namespace SecureConnectionString.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 轉成加密字串
        /// </summary>
        /// <param name="plainString"></param>
        /// <returns></returns>
        public static SecureString ToSecureString(this string plainString)
        {
            if (plainString == null)
            {
                throw new ArgumentNullException(nameof(plainString));
            }

            var securePassword = new SecureString();

            foreach (char c in plainString)
            {
                securePassword.AppendChar(c);
            }

            securePassword.MakeReadOnly();
            return securePassword;
        }
    }
}
