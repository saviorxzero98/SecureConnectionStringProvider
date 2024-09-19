using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using SCNS.Providers;
using SecureConnectionString.Extensions;
using SecureConnectionString.Test.Providers;
using Xunit;

namespace SCNS.Test
{
    public class SecureConnectionStringProviderTest
    {
        public SecureConnectionStringProviderTest()
        {
        }

        [Fact]
        public void TestCustomProvider()
        {
            // Arrange
            var provider = new CustomSecureConnectionStringProvider();
            var text = "P@ssw0rd";

            // Act
            var secretText = provider.Encrypt(text.ToSecureString());
            var plainText = provider.Decrypt(secretText).ToPlainString();

            // Assert
            var exceptionSecretText = "847c65514ece7fe02e2e0e13e0ab0378";
            Assert.Equal(exceptionSecretText, secretText);
            Assert.Equal(text, plainText);
        }

        [Fact]
        public void TestDataProtectionProvider()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo("Keys"));
            var dataProtector = services.BuildServiceProvider()
                                        .GetDataProtectionProvider()
                                        .CreateProtector("Sample");
            var provider = new DataProtectionConnectionStringProvider(dataProtector);
            var text = "P@ssw0rd";

            // Act
            var secretText = provider.Encrypt(text.ToSecureString());
            var plainText = provider.Decrypt(secretText).ToPlainString();

            // Assert
            Assert.Equal(text, plainText);
        }
    }
}
