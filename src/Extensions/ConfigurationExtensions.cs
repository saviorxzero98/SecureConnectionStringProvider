using Microsoft.Extensions.Configuration;
using SCNS;
using SCNS.EncryptionProviders;

namespace SecureConnectionString.Extensions
{
    public static class ConfigurationExtensions
    {
        #region Connection String

        internal const string ConnectionStringSession = "ConnectionStrings";

        /// <summary>
        /// Get Connection String
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetConnectionStrings(this IConfiguration configuration)
        {
            var connections = configuration.GetSection(ConnectionStringSession).Get<Dictionary<string, string>>();
            return connections;
        }

        /// <summary>
        /// Get Connection String
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="name"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static string GetSecureConnectionString(this IConfiguration configuration, string name,
                                                       ConnectionStringProtectionOptions? option = null)
        {
            var encryptionProvider = new StringEncryptionProvider();
            var protectionProvider = new ConnectionStringProtectionProvider(encryptionProvider, option);
            string connectionString = configuration.GetConnectionString(name);
            return protectionProvider.Unprotect(connectionString);
        }

        /// <summary>
        /// Get Connection String
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="name"></param>
        /// <param name="encryptionProvider"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static string GetSecureConnectionString(this IConfiguration configuration, string name,
                                                       IStringEncryptionProvider encryptionProvider,
                                                       ConnectionStringProtectionOptions? option = null)
        {
            var protectionProvider = new ConnectionStringProtectionProvider(encryptionProvider, option);
            string connectionString = configuration.GetConnectionString(name);
            return protectionProvider.Unprotect(connectionString);
        }

        /// <summary>
        /// 檢查 Connection String 是否存在
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public static bool HasConnectionString(this IConfiguration configuration, string connectionName)
        {
            if (!string.IsNullOrWhiteSpace(connectionName))
            {
                string sessionName = GetConnectorStringSession(connectionName);
                return configuration.GetSection(sessionName).Exists();
            }
            return false;
        }

        /// <summary>
        /// 取得連線字串的 Session
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        private static string GetConnectorStringSession(string connectionName)
        {
            if (!string.IsNullOrWhiteSpace(connectionName))
            {
                return $"{ConnectionStringSession}:{connectionName}";
            }
            return string.Empty;
        }

        #endregion


        #region Settings

        /// <summary>
        /// 取得 Settings
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static string GetSetting(this IConfiguration configuration, string sectionName)
        {
            return configuration.GetSection(sectionName).Value ?? string.Empty;
        }

        /// <summary>
        /// 取得 Settings
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static T GetSetting<T>(this IConfiguration configuration, string sectionName)
        {
            return configuration.GetSection(sectionName).Get<T>();
        }

        /// <summary>
        /// 是否有指定的 Setting
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static bool HasSettingSection(this IConfiguration configuration, string sectionName)
        {
            return configuration.GetSection(sectionName).Exists();
        }

        #endregion
    }
}
