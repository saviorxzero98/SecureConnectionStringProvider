namespace SCNS
{
    public static class ConnectionStringProtectionConst
    {
        /// <summary>
        /// SQL Server、PostgreSQL、Oracle 連線字串密碼的 Session
        /// </summary>
        public const string DbPasswordSession = "Password";

        /// <summary>
        /// MySQL、MariaDB 連線字串密碼的 Session
        /// </summary>
        public const string MySqlPasswordSession = "Pwd";

        /// <summary>
        /// 預設連線字串加密狀態 Session
        /// </summary>
        public const string DefaultProtectionPasswordStateSession = "Is Encrypted Password";

        /// <summary>
        /// 預設整個連線字串的 Session
        /// </summary>
        public const string DefaultProtectionCipherValueSession = "Cipher Value";

        /// <summary>
        /// appsettings.json 連線字串 Session
        /// </summary>
        internal const string ConnectionStringSession = "ConnectionStrings";
    }
}
