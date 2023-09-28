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
            var service = new AppSettingService("integration");
            var provider = new CustomSecureConnectionStringProvider();


            #region Add

            // Add Connection String
            bool addConnectionResult = service.AddOrUpdateConnectionString("TestDB", "Data Source=DemoDB;Initial Catalog=Demo;User Id=sa;Password=P@ssw0rd;");
            bool addConnectionResult2 = service.AddOrUpdateConnectionString("TestDB2", "Server=127.0.0.1;Port=5432;Database=DemoDB;User Id=postgres;Password=P@ssw0rd;");

            // Assert Add Connection String
            Assert.True(addConnectionResult);
            Assert.True(addConnectionResult2);

            #endregion


            #region Encrypt

            // Encrypt Connection String
            var connections = new List<string>() { "TestDB", "TestDB2" };
            var helper = new SecureConnectionStringHelper(provider);

            service.UpdateConnectionStrings(connections, (name, value) => helper.EncryptConnectionString(value));
            //service.EncryptConnectionStrings(helper, connections);

            string secureConnectionValue = service.GetConnectorString("TestDB");
            string secureConnectionValue2 = service.GetConnectorString("TestDB2");

            // Assert Encrypt Connection String
            Assert.Equal("data source=DemoDB;initial catalog=Demo;user id=sa;password=847c65514ece7fe02e2e0e13e0ab0378;Is Encrypted Password=True", secureConnectionValue);
            Assert.Equal("server=127.0.0.1;port=5432;database=DemoDB;user id=postgres;password=847c65514ece7fe02e2e0e13e0ab0378;Is Encrypted Password=True", secureConnectionValue2);

            #endregion


            #region Get Connection String

            string plainServerConnection = service.GetSecureConnectionString("TestDB", provider);
            string plainServerConnection2 = service.GetSecureConnectionString("TestDB2", provider);

            Assert.Equal("data source=DemoDB;initial catalog=Demo;user id=sa;password=P@ssw0rd", plainServerConnection);
            Assert.Equal("server=127.0.0.1;port=5432;database=DemoDB;user id=postgres;password=P@ssw0rd", plainServerConnection2);

            #endregion


            #region Delete

            // Delete Connection String
            bool deleteConnectionResult = service.DeleteConnectionString("TestDB");
            bool deleteConnectionResult2 = service.DeleteConnectionString("TestDB2");

            // Assert Delete Connection String
            Assert.True(deleteConnectionResult);
            Assert.True(deleteConnectionResult2);

            #endregion
        }
    }
}
