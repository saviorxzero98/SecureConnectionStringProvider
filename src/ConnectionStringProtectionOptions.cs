using SCNS.EncryptionProviders;

namespace SCNS
{
    public enum ConnectionStringProtectionType
    {
        /// <summary>
        /// 不處理任何加密
        /// </summary>
        None = 0,

        /// <summary>
        /// 只加密連線字串密碼
        /// </summary>
        PasswordOnly = 1,

        /// <summary>
        /// 指定的連線字串 Session
        /// </summary>
        SpecifiedSessions = 2,

        /// <summary>
        /// 整個連線字串
        /// </summary>
        ConnectionString = 3
    }

    public class ConnectionStringProtectionOptions
    {
        /// <summary>
        /// 連線字串加密類型
        /// </summary>
        public ConnectionStringProtectionType ProtectionType { get; set; }

        /// <summary>
        /// 加密保護的 Session Name
        /// </summary>
        public List<string> ProtectionSessions { get; set; }

        /// <summary>
        /// 連線字串加密狀態 Session，僅在 Type = 3 時有效
        /// </summary>
        public string ProtectionStateSession { get; set; }

        /// <summary>
        /// 整個連線字串的 Session，僅在 Type = 4 時有效
        /// </summary>
        public string ProtectionCipherValueSession { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        public IStringEncryptionKey? StringEncryptionKey { get; set; }


        public ConnectionStringProtectionOptions()
        {
            ProtectionType = ConnectionStringProtectionType.PasswordOnly;
            ProtectionSessions = new List<string>()
            {
                ConnectionStringProtectionConst.DbPasswordSession,
                ConnectionStringProtectionConst.MySqlPasswordSession
            };
            ProtectionStateSession = ConnectionStringProtectionConst.DefaultProtectionPasswordStateSession;
            ProtectionCipherValueSession = ConnectionStringProtectionConst.DefaultProtectionCipherValueSession;
        }
        public ConnectionStringProtectionOptions(IStringEncryptionKey key)
        {
            ProtectionType = ConnectionStringProtectionType.PasswordOnly;
            ProtectionSessions = new List<string>()
            {
                ConnectionStringProtectionConst.DbPasswordSession,
                ConnectionStringProtectionConst.MySqlPasswordSession
            };
            ProtectionStateSession = ConnectionStringProtectionConst.DefaultProtectionPasswordStateSession;
            ProtectionCipherValueSession = ConnectionStringProtectionConst.DefaultProtectionCipherValueSession;
            StringEncryptionKey = key;
        }
    }
}
