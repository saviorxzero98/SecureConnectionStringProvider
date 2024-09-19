using Microsoft.AspNetCore.DataProtection;
using SecureConnectionString.Cryptos;
using SecureConnectionString.Extensions;
using SecureConnectionString.Providers;
using System.Security;
using System.Text;

namespace SCNS.Providers
{
    public class DataProtectionConnectionStringProvider : ISecureConnectionStringProvider
    {
        private readonly IDataProtector _dataProtector;


        public DataProtectionConnectionStringProvider(IDataProtector dataProtector)
        {
            _dataProtector = dataProtector;
        }

        public string Encrypt(SecureString securePlainText)
        {
            if (securePlainText.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var textBytes = Encoding.UTF8.GetBytes(securePlainText.ToPlainString());
            var secretBytes = _dataProtector.Protect(textBytes);
            return HexEncoder.Instance.ToHexString(secretBytes);
        }

        public SecureString Decrypt(string secretText)
        {
            if (string.IsNullOrEmpty(secretText))
            {
                return string.Empty.ToSecureString();
            }

            try
            {
                var secretBytes = HexEncoder.Instance.FromHexString(secretText);
                var plainBytes = _dataProtector.Unprotect(secretBytes);
                return Encoding.UTF8.GetString(plainBytes).ToSecureString();
            }
            catch
            {
                return string.Empty.ToSecureString();
            }
        }
    }
}
