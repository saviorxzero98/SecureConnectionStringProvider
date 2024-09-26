using SCNS.EncryptionProviders;
using SCNS.Extensions;
using System.Data.Common;

namespace SCNS
{
    public class ConnectionStringProtectionProvider : IConnectionStringProtectionProvider
    {
        protected readonly IStringEncryptionProvider ProtectionProvider;
        protected ConnectionStringProtectionOptions Options { get; set; }

        public static List<string> PasswordSessionNames
        {
            get
            {
                return new List<string>()
                {
                    ConnectionStringProtectionConst.DbPasswordSession,
                    ConnectionStringProtectionConst.MySqlPasswordSession
                };
            }
        }


        public ConnectionStringProtectionProvider(IStringEncryptionProvider provider,
                                                  ConnectionStringProtectionOptions? options = null)
        {
            ProtectionProvider = provider;
            Options = options ?? new ConnectionStringProtectionOptions();
        }

        /// <summary>
        /// 加密連線字串
        /// </summary>
        /// <param name="connectionString">連線字串</param>
        /// <returns>密文，為 Hex 字串</returns>
        public virtual string Protect(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }

            var builder = new DbConnectionStringBuilder()
            {
                ConnectionString = connectionString
            };

            switch (Options.ProtectionType)
            {
                case ConnectionStringProtectionType.ConnectionString:
                    return EncryptConnectionString(builder, Options);

                case ConnectionStringProtectionType.PasswordOnly:
                    Options.ProtectionSessions = PasswordSessionNames;
                    return EncryptSessionValues(builder, Options);

                case ConnectionStringProtectionType.SpecifiedSessions:
                    return EncryptSessionValues(builder, Options);

                case ConnectionStringProtectionType.None:
                default:
                    return connectionString;
            }
        }

        /// <summary>
        /// 解密連線字串
        /// </summary>
        /// <param name="secureConnectionString">已加密的連線字串</param>
        /// <returns>連線字串</returns>
        public virtual string Unprotect(string secureConnectionString)
        {
            if (string.IsNullOrEmpty(secureConnectionString))
            {
                return secureConnectionString;
            }

            var builder = new DbConnectionStringBuilder()
            {
                ConnectionString = secureConnectionString
            };

            switch (Options.ProtectionType)
            {
                case ConnectionStringProtectionType.ConnectionString:
                    return DecryptConnectionString(builder, Options);

                case ConnectionStringProtectionType.PasswordOnly:
                    Options.ProtectionSessions = PasswordSessionNames;
                    return DecryptSessionValues(builder, Options);

                case ConnectionStringProtectionType.SpecifiedSessions:
                    return DecryptSessionValues(builder, Options);


                case ConnectionStringProtectionType.None:
                default:
                    return secureConnectionString;
            }
        }


        #region 指定 Session 內容

        /// <summary>
        /// 加密指定 Sessions
        /// </summary>
        /// <param name="builder"></param>
        protected virtual string EncryptSessionValues(DbConnectionStringBuilder builder,
                                                      ConnectionStringProtectionOptions options)
        {
            // 檢查連線字串是否已經加密
            if (builder.HasSessionName(options.ProtectionCipherValueSession))
            {
                return builder.ConnectionString;
            }

            // 檢查連線字串是否已經部分加密
            if (builder.HasSessionName(options.ProtectionStateSession) &&
                builder.GetBooleanValue(options.ProtectionStateSession) == true)
            {
                return builder.ConnectionString;
            }

            // 加密連線字串
            bool isEncrypted = false;
            foreach (var name in options.ProtectionSessions)
            {
                if (!builder.HasSessionName(name))
                {
                    continue;
                }

                var value = builder.GetStringValue(name);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var cipherValue = ProtectionProvider.Encrypt(value, options.StringEncryptionKey);
                    builder[name] = cipherValue;
                    isEncrypted = true;
                }
            }

            // 設定加密狀態
            if (isEncrypted)
            {
                builder.Add(options.ProtectionStateSession, isEncrypted);
            }

            return builder.ConnectionString;
        }

        /// <summary>
        /// 解密指定 Sessions
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected virtual string DecryptSessionValues(DbConnectionStringBuilder builder,
                                                      ConnectionStringProtectionOptions options)
        {
            // 檢查連線字串是否已經部分加密
            if (!builder.HasSessionName(options.ProtectionStateSession) )
            {
                return builder.ConnectionString;
            }
            else if (builder.GetBooleanValue(options.ProtectionStateSession) == false)
            {
                // 移除加密狀態
                builder.Remove(options.ProtectionStateSession);
                return builder.ConnectionString;

            }

            // 檢查是否整個連線字串加密
            if (builder.HasSessionName(options.ProtectionCipherValueSession))
            {
                return DecryptConnectionString(builder, options);
            }

            // 解密連線字串
            foreach (var name in Options.ProtectionSessions)
            {
                if (!builder.HasSessionName(name))
                {
                    continue;
                }

                var cipherValue = builder.GetStringValue(name);
                if (!string.IsNullOrWhiteSpace(cipherValue))
                {
                    var plainValue = ProtectionProvider.Decrypt(cipherValue, options.StringEncryptionKey);
                    builder[name] = plainValue;
                }
            }

            // 移除加密狀態
            builder.Remove(options.ProtectionStateSession);

            return builder.ConnectionString;
        }

        #endregion


        #region 連線字串

        /// <summary>
        /// 加密整個連線字串
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected virtual string EncryptConnectionString(DbConnectionStringBuilder builder,
                                                         ConnectionStringProtectionOptions options)
        {
            // 檢查連線字串是否已經加密
            if (builder.HasSessionName(options.ProtectionCipherValueSession))
            {
                return builder.ConnectionString;
            }

            // 檢查連線字串是否已經部分加密
            if (builder.HasSessionName(options.ProtectionStateSession) &&
                builder.GetBooleanValue(options.ProtectionStateSession) == true)
            {
                return builder.ConnectionString;
            }

            // 加密連線字串
            var cipherValue = ProtectionProvider.Encrypt(builder.ConnectionString, options.StringEncryptionKey);

            // 調整已經加密的連線字串
            builder.Clear();
            builder.Add(options.ProtectionCipherValueSession, cipherValue);
            builder.Add(options.ProtectionStateSession, true);
            return builder.ConnectionString;
        }

        /// <summary>
        /// 解密整個連線字串
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected virtual string DecryptConnectionString(DbConnectionStringBuilder builder,
                                                         ConnectionStringProtectionOptions options)
        {
            // 檢查連線字串是否已經加密
            if (!builder.HasSessionName(options.ProtectionCipherValueSession))
            {
                return builder.ConnectionString;
            }

            // 解密連線字串
            var cipherValue = builder.GetStringValue(options.ProtectionCipherValueSession);
            var plainValue = ProtectionProvider.Decrypt(cipherValue, options.StringEncryptionKey);

            // 調整已經加密的連線字串
            builder.Clear();
            builder.ConnectionString = plainValue;
            return builder.ConnectionString;
        }

        #endregion
    }
}
