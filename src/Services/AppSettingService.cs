using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecureConnectionString.Extensions;
using SecureConnectionString.Providers;

namespace SecureConnectionString.Services
{
    public class AppSettingService
    {
        public string BaseDirectory { get; set; }
        public string AppSettingFile { get; set; }

        public const string ConnectionStringSession = "ConnectionStrings";

        public AppSettingService(string environment = "")
        {
            BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (string.IsNullOrWhiteSpace(environment))
            {
                AppSettingFile = "appsettings.json";
            }
            else
            {
                AppSettingFile = $"appsettings.{environment}.json";
            }
        }



        #region Get

        /// <summary>
        /// 取出所有連線字串
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetConnectorStrings()
        {
            var connections = new Dictionary<string, string>();
            IConfiguration config = CreateConfiguration();

            if (config != null)
            {
                connections = config.GetConnectionStrings() ?? new Dictionary<string, string>();
            }
            return connections;
        }

        /// <summary>
        /// 取出連線字串
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public string GetConnectorString(string connectionName)
        {
            string connectorString = string.Empty;
            IConfiguration config = CreateConfiguration();

            if (config != null)
            {
                connectorString = config.GetConnectionString(connectionName) ?? string.Empty;
            }
            return connectorString;
        }

        /// <summary>
        /// 取出連線字串 (已加密)
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public string GetSecureConnectionString(string connectionName, SecureConnectionStringOption option = null)
        {
            string connectorString = string.Empty;
            IConfiguration config = CreateConfiguration();

            if (config != null)
            {
                connectorString = config.GetSecureConnectionString(connectionName, option) ?? string.Empty;
            }
            return connectorString;
        }

        /// <summary>
        /// 取出連線字串 (已加密)
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="provider"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public string GetSecureConnectionString(string connectionName, ISecureConnectionStringProvider provider, SecureConnectionStringOption option = null)
        {
            string connectorString = string.Empty;
            IConfiguration config = CreateConfiguration();

            if (config != null)
            {
                connectorString = config.GetSecureConnectionString(connectionName, provider, option) ?? string.Empty;
            }
            return connectorString;
        }

        /// <summary>
        /// 取出相關設定
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public string GetSetting(string sectionName)
        {
            IConfiguration config = CreateConfiguration();

            if (config != null)
            {
                return config.GetSection(sectionName).Value ?? string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// 取出相關設定
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public T GetSetting<T>(string sectionName)
        {
            IConfiguration config = CreateConfiguration();

            if (config != null)
            {
                return config.GetSection(sectionName).Get<T>();
            }

            return default(T);
        }

        /// <summary>
        /// 是否存在連線字串
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasConnectorString(string connectionName)
        {
            IConfiguration config = CreateConfiguration();

            if (config != null && !string.IsNullOrWhiteSpace(connectionName))
            {
                return config.HasConnectionString(connectionName);
            }
            return false;
        }

        /// <summary>
        /// 是否存在相關設定
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasSetting(string sectionName)
        {
            IConfiguration config = CreateConfiguration();

            if (config != null)
            {
                return config.HasSettingSection(sectionName);
            }
            return false;
        }

        /// <summary>
        /// 是否為 Null Configuration
        /// </summary>
        /// <returns></returns>
        public bool IsNullConfiguration()
        {
            IConfiguration config = CreateConfiguration();

            return (config == null);
        }

        /// <summary>
        /// 建立 Configuration
        /// </summary>
        /// <returns></returns>
        protected IConfiguration CreateConfiguration()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile(AppSettingFile,
                                                                       optional: true,
                                                                       reloadOnChange: true)
                                                          .Build();
            return configuration;
        }

        #endregion


        #region Add Or Update

        /// <summary>
        /// 新增/更新 AppSettings 值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sectionPathKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateAppSetting<T>(string sectionPathKey, T value)
        {
            if (string.IsNullOrEmpty(sectionPathKey))
            {
                return false;
            }

            try
            {
                // 讀取 appsettings
                JToken settings = ReadAppSettings();

                if (settings != null)
                {
                    // 設定 appsettings
                    SetValueRecursively(sectionPathKey, settings, value);

                    // 寫入 appsettings
                    return WriteAppSettings(settings);
                }
            }
            catch
            {

            }
            return false;
        }

        /// <summary>
        /// 新增/更新 Connection String
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateConnectionString(string connectionName, string value)
        {
            if (!string.IsNullOrEmpty(connectionName))
            {
                string sectionPathKey = GetConnectorStringSession(connectionName);
                return AddOrUpdateAppSetting(sectionPathKey, value);
            }
            return false;
        }

        /// <summary>
        /// 新增/更新設定 (巢狀處理)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sectionPathKey"></param>
        /// <param name="jToken"></param>
        /// <param name="value"></param>
        protected void SetValueRecursively<T>(string sectionPathKey, JToken jToken, T value)
        {
            string[] remainingSections = sectionPathKey.Split(":", 2);

            string currentSection = remainingSections[0];
            if (remainingSections.Length > 1)
            {
                var nextSection = remainingSections[1];
                SetValueRecursively(nextSection, jToken[currentSection], value);
            }
            else
            {
                jToken[currentSection] = JToken.FromObject(value);
            }
        }

        #endregion


        #region Update

        /// <summary>
        /// 更新 AppSettings 值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sectionPathKey"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool UpdateAppSetting<T>(string sectionPathKey, Func<T, T> predicate)
        {
            if (!string.IsNullOrEmpty(sectionPathKey) &&
                HasSetting(sectionPathKey) &&
                predicate != null)
            {
                // 取得設定值
                T value = GetSetting<T>(sectionPathKey);
                T newValue = predicate(value);

                // 更新設定值
                return AddOrUpdateAppSetting(sectionPathKey, newValue);
            }
            return false;
        }

        /// <summary>
        /// 更新 Connection String
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool UpdateConnectionString(string connectionName, Func<string, string> predicate)
        {
            if (!string.IsNullOrEmpty(connectionName) &&
                HasConnectorString(connectionName) &&
                predicate != null)
            {
                // 取得設定值
                string connectionString = GetConnectorString(connectionName);
                string newConnectionString = predicate(connectionString);

                // 更新設定值
                return AddOrUpdateConnectionString(connectionName, newConnectionString);
            }
            return false;
        }

        /// <summary>
        /// 更新 Connection Strings
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool UpdateConnectionStrings(Func<string, string, string> predicate)
        {
            if (predicate == null)
            {
                return false;
            }

            // 取得所有連線字串
            Dictionary<string, string> connections = GetConnectorStrings();

            if (connections != null && connections.Any())
            {
                return UpdateConnectionStrings(connections, predicate);
            }

            return false;
        }

        /// <summary>
        /// 更新 Connection Strings
        /// </summary>
        /// <param name="connectionNames"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool UpdateConnectionStrings(IEnumerable<string> connectionNames, Func<string, string, string> predicate)
        {
            if (predicate == null)
            {
                return false;
            }

            // 取得所有連線字串
            Dictionary<string, string> connections = GetConnectorStrings();
            var selectConnections = new Dictionary<string, string>();

            foreach (var name in connectionNames)
            {
                if (connections.ContainsKey(name))
                {
                    selectConnections.Add(name, connections[name]);
                }
            }

            if (selectConnections.Any())
            {
                return UpdateConnectionStrings(selectConnections, predicate);
            }

            return false;
        }

        /// <summary>
        /// 更新 Connection Strings
        /// </summary>
        /// <param name="connections"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        protected bool UpdateConnectionStrings(Dictionary<string, string> connections, Func<string, string, string> predicate)
        {
            if (predicate == null || connections == null || !connections.Any())
            {
                return false;
            }

            try
            {
                // 讀取 appsettings
                JToken settings = ReadAppSettings();

                if (settings != null)
                {
                    foreach (var connection in connections)
                    {
                        string connectionName = connection.Key;
                        string connectionString = connection.Value;

                        // 取得連線字串
                        string newConnectionString = predicate(connectionName, connectionString);

                        // 設定 appsettings
                        string sessionName = GetConnectorStringSession(connectionName);
                        SetValueRecursively(sessionName, settings, newConnectionString);
                    }

                    // 寫入 appsettings
                    return WriteAppSettings(settings);
                }
            }
            catch
            {

            }
            return false;
        }

        #endregion


        #region Delete


        /// <summary>
        /// 刪除 AppSettings 值
        /// </summary>
        /// <param name="sectionPathKey"></param>
        /// <returns></returns>
        public bool DeleteAppSetting(string sectionPathKey)
        {
            if (string.IsNullOrEmpty(sectionPathKey))
            {
                return false;
            }

            try
            {
                // 讀取 appsettings
                JToken settings = ReadAppSettings();

                if (settings != null)
                {
                    // 刪除設定
                    DeleteRecursively(sectionPathKey, settings);

                    // 寫入 appsettings
                    return WriteAppSettings(settings);
                }
            }
            catch
            {

            }
            return false;
        }

        /// <summary>
        /// 刪除 Connection String
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public bool DeleteConnectionString(string connectionName)
        {
            if (!string.IsNullOrEmpty(connectionName))
            {
                string sectionPathKey = GetConnectorStringSession(connectionName);
                return DeleteAppSetting(sectionPathKey);
            }
            return false;
        }

        /// <summary>
        /// 刪除設定 (巢狀處理)
        /// </summary>
        /// <param name="sectionPathKey"></param>
        /// <param name="jToken"></param>
        protected void DeleteRecursively(string sectionPathKey, JToken jToken)
        {
            string[] remainingSections = sectionPathKey.Split(":", 2);

            string currentSection = remainingSections[0];
            if (remainingSections.Length > 1)
            {
                var nextSection = remainingSections[1];
                DeleteRecursively(nextSection, jToken[currentSection]);
            }
            else
            {
                if (jToken.Type == JTokenType.Object)
                {
                    var jObj = (JObject)jToken;
                    jObj.Remove(currentSection);
                }
            }
        }

        #endregion


        #region Private Method

        /// <summary>
        /// 取得連線字串的 Session
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        protected string GetConnectorStringSession(string connectionName)
        {
            if (!string.IsNullOrWhiteSpace(connectionName))
            {
                return $"{ConnectionStringSession}:{connectionName}";
            }
            return string.Empty;
        }

        /// <summary>
        /// 讀取 appsettings
        /// </summary>
        /// <returns></returns>
        protected JToken ReadAppSettings()
        {
            string filePath = Path.Combine(BaseDirectory, AppSettingFile);

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                JToken jToken = JToken.Parse(json);
                return jToken;
            }
            return default(JToken);
        }

        /// <summary>
        /// 寫入 appsettings
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        protected bool WriteAppSettings(JToken settings)
        {
            try
            {
                string filePath = Path.Combine(BaseDirectory, AppSettingFile);
                string output = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(filePath, output);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
