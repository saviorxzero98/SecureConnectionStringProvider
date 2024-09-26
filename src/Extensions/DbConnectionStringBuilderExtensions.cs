using System.Data.Common;

namespace SCNS.Extensions
{
    public static class DbConnectionStringBuilderExtensions
    {
        /// <summary>
        /// 檢查連線字串是否含有指定 Session Name
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="sessionName"></param>
        /// <returns></returns>
        public static bool HasSessionName(this DbConnectionStringBuilder builder, string sessionName)
        {
            var keys = new List<string>(builder.Keys.Cast<string>().ToList());
            return keys.Any(key => key.ToLower() == sessionName.ToLower());
        }

        /// <summary>
        /// 從連線字串取得 Session Boolean 值
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="sessionName"></param>
        /// <returns></returns>
        public static bool GetBooleanValue(this DbConnectionStringBuilder builder, string sessionName)
        {
            if (builder.HasSessionName(sessionName))
            {
                var sessionValue = Convert.ToString(builder[sessionName]);

                if (bool.TryParse(sessionValue, out var isEncryptedPassword))
                {
                    return isEncryptedPassword;
                }
            }
            return false;
        }

        /// <summary>
        /// 從連線字串取得 Session String 值
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="sessionName"></param>
        /// <returns></returns>
        public static string GetStringValue(this DbConnectionStringBuilder builder, string sessionName)
        {
            if (builder.HasSessionName(sessionName))
            {
                var sessionValue = Convert.ToString(builder[sessionName]);
                if (!string.IsNullOrEmpty(sessionValue))
                {
                    return sessionValue;
                }
            }
            return string.Empty;
        }
    }
}
