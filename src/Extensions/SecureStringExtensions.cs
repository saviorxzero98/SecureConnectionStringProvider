using System.Runtime.InteropServices;
using System.Security;

namespace SecureConnectionString.Extensions
{
    public static class SecureStringExtensions
    {
        /// <summary>
        /// 轉成一般字串
        /// </summary>
        /// <param name="secureString"></param>
        /// <returns></returns>
        public static string ToPlainString(this SecureString secureString)
        {
            if (secureString == null)
            {
                return string.Empty;
            }

            var ptr = Marshal.SecureStringToBSTR(secureString);

            try
            {
                return Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                Marshal.ZeroFreeBSTR(ptr);
            }

        }

        /// <summary>
        /// 是否為空字串
        /// </summary>
        /// <param name="secureString"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this SecureString secureString)
        {
            if (secureString == null || secureString.Length == 0)
            {
                return true;
            }
            return string.IsNullOrEmpty(secureString.ToPlainString());
        }
    }
}
