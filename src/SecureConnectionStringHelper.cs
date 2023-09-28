using SecureConnectionString.Extensions;
using SecureConnectionString.Providers;
using System.Data.Common;
using System.Security;

namespace SecureConnectionString
{
    public class SecureConnectionStringHelper
    {
        private readonly ISecureConnectionStringProvider Provider;
        private SecureConnectionStringOption Option { get; set; }
        public string TargetSession { get => Option.TargetSession; }
        public string StateSession { get => Option.StateSession; }

        public SecureConnectionStringHelper(SecureConnectionStringOption option = null)
        {
            Provider = new DefaultSecureConnectionStringProvider();

            if (option != null)
            {
                Option = option;
            }
            else
            {
                Option = new SecureConnectionStringOption();
            }
        }
        public SecureConnectionStringHelper(ISecureConnectionStringProvider provider,
                                            SecureConnectionStringOption option = null)
        {
            Provider = provider;

            if (option != null)
            {
                Option = option;
            }
            else
            {
                Option = new SecureConnectionStringOption();
            }
        }


        /// <summary>
        /// 解密連線字串
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public string DecryptConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }

            try
            {
                var builder = new DbConnectionStringBuilder()
                {
                    ConnectionString = connectionString
                };

                if (builder.ContainsKey(StateSession) &&
                    builder.ContainsKey(TargetSession))
                {
                    // 取出是否加密的連線字串項目，並移除掉
                    bool isEncrypt = Convert.ToBoolean(builder[StateSession]);
                    builder.Remove(StateSession);

                    // 取出已加密的密碼
                    string secretText = Convert.ToString(builder[TargetSession]);

                    // 密碼解密
                    if (isEncrypt && !string.IsNullOrEmpty(secretText))
                    {
                        builder[TargetSession] = Provider.Decrypt(secretText).ToPlainString();
                    }

                    string connectSring = builder.ConnectionString;
                    return connectSring;
                }
            }
            catch
            {

            }
            return connectionString;
        }

        /// <summary>
        /// 加密連線字串
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public string EncryptConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }

            try
            {
                var builder = new DbConnectionStringBuilder()
                {
                    ConnectionString = connectionString
                };

                bool isEncrypt = false;

                // 檢查是否已加密的連線字串項目
                if (builder.ContainsKey(StateSession))
                {
                    string isEncryptText = Convert.ToString(builder[StateSession]);
                    bool.TryParse(isEncryptText, out isEncrypt);
                }

                // 取出密碼
                SecureString password = Convert.ToString(builder[TargetSession]).ToSecureString();

                // 對密碼進行加密
                if (!isEncrypt && !password.IsNullOrEmpty())
                {
                    builder[TargetSession] = Provider.Encrypt(password);
                    builder.Add(StateSession, Convert.ToString(true));

                    string connect = builder.ConnectionString;
                    return connect;
                }
            }
            catch
            {

            }

            return connectionString;
        }
    }
}
