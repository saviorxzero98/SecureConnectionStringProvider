namespace SecureConnectionString
{
    public class SecureConnectionStringOption
    {
        public const string DefaultTargetSession = "Password";
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
    }
}
