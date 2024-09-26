using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using SCNS.EncryptionProviders;
using SecureConnectionString.Test.Providers;
using Xunit;

namespace SCNS.Test
{
    public class StringEncryptionProviderTest
    {
        public StringEncryptionProviderTest()
        {
        }

        [Fact]
        public void TestDefaultProvider()
        {
            // Arrange
            var provider = new StringEncryptionProvider(key: "b1047f05-c921-4564-85e2-44221602df82",
                                                        initVector: "cd8846c6-22b9-4e6f-9d59-f05ff27567d7");
            var text = "P@ssw0rd";

            // Act
            var secretText = provider.Encrypt(text);
            var plainText = provider.Decrypt(secretText);

            // Assert
            var exceptionSecretText = "b3f12a22ee4bf5fe04a90c1fb500837b";
            Assert.Equal(exceptionSecretText, secretText);
            Assert.Equal(text, plainText);
        }

        [Fact]
        public void TestCustomProvider()
        {
            // Arrange
            var provider = new CustomStringEncryptionProvider();
            var text = "P@ssw0rd";

            // Act
            var secretText = provider.Encrypt(text);
            var plainText = provider.Decrypt(secretText);

            // Assert
            var exceptionSecretText = "b3f12a22ee4bf5fe04a90c1fb500837b";
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
            var provider = new DataProtectionStringEncryptionProvider(dataProtector);
            var text = "P@ssw0rd";

            // Act
            var secretText = provider.Encrypt(text);
            var plainText = provider.Decrypt(secretText);

            // Assert
            Assert.Equal(text, plainText);
        }
    }
}
