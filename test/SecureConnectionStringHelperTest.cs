using Microsoft.Extensions.Configuration;
using SecureConnectionString.Extensions;
using SecureConnectionString.Test.Providers;
using Xunit;

namespace SecureConnectionString.Test
{
    public class SecureConnectionStringHelperTest
    {
        protected IConfiguration Configuration { get; set; }

        public SecureConnectionStringHelperTest()
        {
            Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.secure.json",
                                                                   optional: true,
                                                                   reloadOnChange: true)
                                                      .Build();
        }

        [Fact]
        public void TestGetConnectionStringWithCustom()
        {
            // Arrange
            var provider = new CustomSecureConnectionStringProvider();

            // Act
            string sqlServerConnection = Configuration.GetSecureConnectionString("SqlServer", provider);
            string postgreServerConnection = Configuration.GetSecureConnectionString("PostgreSql", provider);
            string plainSqlServerConnection = Configuration.GetConnectionString("SqlServer");
            string plainPostgreServerConnection = Configuration.GetConnectionString("PostgreSql");

            // Assert
            Assert.Equal(plainSqlServerConnection, sqlServerConnection);
            Assert.Equal(plainPostgreServerConnection, postgreServerConnection);
        }

        [Fact]
        public void TestGetSecureConnectionStringWithCustom()
        {
            // Arrange
            var provider = new CustomSecureConnectionStringProvider();

            // Act
            string sqlServerConnection = Configuration.GetSecureConnectionString("SqlServer-Encrypted", provider);
            string postgreServerConnection = Configuration.GetSecureConnectionString("PostgreSql-Encrypted", provider);

            // Assert
            Assert.Equal("data source=DemoDB;initial catalog=Demo;user id=sa;password=P@ssw0rd", sqlServerConnection);
            Assert.Equal("server=127.0.0.1;port=5432;database=DemoDB;user id=postgres;password=P@ssw0rd", postgreServerConnection);
        }

        [Fact]
        public void TestGetUnsecureConnectionStringWithCustom()
        {
            // Arrange
            var provider = new CustomSecureConnectionStringProvider();

            // Act
            string sqlServerConnection = Configuration.GetSecureConnectionString("SqlServer-Unencrypted", provider);
            string postgreServerConnection = Configuration.GetSecureConnectionString("PostgreSql-Unencrypted", provider);

            // Assert
            Assert.Equal("data source=DemoDB;initial catalog=Demo;user id=sa;password=P@ssw0rd", sqlServerConnection);
            Assert.Equal("server=127.0.0.1;port=5432;database=DemoDB;user id=postgres;password=P@ssw0rd", postgreServerConnection);
        }
    }
}
