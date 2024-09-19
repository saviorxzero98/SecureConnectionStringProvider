namespace SecureConnectionString
{
    public class SecureConnectionStringOption
    {
        public const string DefaultTargetSession = "Password";
        public const string MySqlTargetSession = "Pwd";
        public const string DefaultStateSession = "Is Encrypted Password";

        public string TargetSession { get; set; } = DefaultTargetSession;
        public string StateSession { get; set; } = DefaultStateSession;

        public SecureConnectionStringOption()
        {
            TargetSession = DefaultTargetSession;
            StateSession = DefaultStateSession;
        }
        public SecureConnectionStringOption(string targetSession, string stateSession)
        {
            TargetSession = (string.IsNullOrWhiteSpace(targetSession)) ? DefaultTargetSession : targetSession;
            StateSession = (string.IsNullOrWhiteSpace(stateSession)) ? DefaultStateSession : stateSession;
        }

        public static SecureConnectionStringOption SqlServerOption 
        { 
            get => new SecureConnectionStringOption(DefaultTargetSession, DefaultStateSession);
        }
        public static SecureConnectionStringOption PostgreSqlOptions
        {
            get => new SecureConnectionStringOption(DefaultTargetSession, DefaultStateSession);
        }
        public static SecureConnectionStringOption MySqlOptions
        {
            get => new SecureConnectionStringOption(MySqlTargetSession, DefaultStateSession);
        }
    }
}
