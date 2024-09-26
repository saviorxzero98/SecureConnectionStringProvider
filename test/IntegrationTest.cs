using SCNS;
using SecureConnectionString.Services;
using SecureConnectionString.Test.Providers;
using Xunit;

namespace SecureConnectionString.Test
{
    public class IntegrationTest
    {

        [Fact]
        public void TestEncryptConnectionString()
        {
            var manager = new AppSettingManager("integration");
            var provider = new CustomStringEncryptionProvider();


            #region Add

            // Add Connection String
            bool addConnectionResult = manager.AddOrUpdateConnectionString("TestDB", "Data Source=DemoDB;Initial Catalog=Demo;User Id=sa;Password=P@ssw0rd;");
            bool addConnectionResult2 = manager.AddOrUpdateConnectionString("TestDB2", "Server=127.0.0.1;Port=5432;Database=DemoDB;User Id=postgres;Password=P@ssw0rd;");

            // Assert Add Connection String
            Assert.True(addConnectionResult);
            Assert.True(addConnectionResult2);

            #endregion


            #region Encrypt

            // Encrypt Connection String
            var connections = new List<string>() { "TestDB", "TestDB2" };
            var protectionProvider = new ConnectionStringProtectionProvider(provider);

            manager.UpdateConnectionStrings(connections, (name, value) => protectionProvider.Protect(value));

            string secureConnectionValue = manager.GetConnectorString("TestDB");
            string secureConnectionValue2 = manager.GetConnectorString("TestDB2");

            // Assert Encrypt Connection String
            Assert.Equal("data source=DemoDB;initial catalog=Demo;user id=sa;password=b3f12a22ee4bf5fe04a90c1fb500837b;Is Encrypted Password=True", secureConnectionValue);
            Assert.Equal("server=127.0.0.1;port=5432;database=DemoDB;user id=postgres;password=b3f12a22ee4bf5fe04a90c1fb500837b;Is Encrypted Password=True", secureConnectionValue2);

            #endregion


            #region Get Connection String

            string plainServerConnection = manager.GetSecureConnectionString("TestDB", provider);
            string plainServerConnection2 = manager.GetSecureConnectionString("TestDB2", provider);

            Assert.Equal("data source=DemoDB;initial catalog=Demo;user id=sa;password=P@ssw0rd", plainServerConnection);
            Assert.Equal("server=127.0.0.1;port=5432;database=DemoDB;user id=postgres;password=P@ssw0rd", plainServerConnection2);

            #endregion


            #region Delete

            // Delete Connection String
            bool deleteConnectionResult = manager.DeleteConnectionString("TestDB");
            bool deleteConnectionResult2 = manager.DeleteConnectionString("TestDB2");

            // Assert Delete Connection String
            Assert.True(deleteConnectionResult);
            Assert.True(deleteConnectionResult2);

            #endregion
        }
    }
}
