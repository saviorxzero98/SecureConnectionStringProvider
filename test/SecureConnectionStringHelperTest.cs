using Microsoft.Extensions.Configuration;
using SecureConnectionString.Extensions;
using SecureConnectionString.Providers;
using SecureConnectionString.Test.Providers;
using Xunit;

namespace SecureConnectionString.Test
{
    public class SecureConnectionStringHelperTest
    {
        protected IConfiguration Configuration { get; set; }
        protected ISecureConnectionStringProvider Provider { get; set; }

        public SecureConnectionStringHelperTest()
        {
            Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.secure.json",
                                                                   optional: true,
                                                                   reloadOnChange: true)
                                                      .Build();
            Provider = new CustomSecureConnectionStringProvider();
        }

        [Fact]
        public void TestGetConnectionString()
        {
            string sqlServerConnection = Configuration.GetSecureConnectionString("SqlServer", Provider);
            string postgreServerConnection = Configuration.GetSecureConnectionString("PostgreSql", Provider);

            string plainSqlServerConnection = Configuration.GetConnectionString("SqlServer");
            string plainPostgreServerConnection = Configuration.GetConnectionString("PostgreSql");

            Assert.Equal(plainSqlServerConnection, sqlServerConnection);
            Assert.Equal(plainPostgreServerConnection, postgreServerConnection);
        }

        [Fact]
        public void TestGetSecureConnectionString()
        {
            string sqlServerConnection = Configuration.GetSecureConnectionString("SqlServer-Encrypted", Provider);
            string postgreServerConnection = Configuration.GetSecureConnectionString("PostgreSql-Encrypted", Provider);

            Assert.Equal("data source=DemoDB;initial catalog=Demo;user id=sa;password=P@ssw0rd", sqlServerConnection);
            Assert.Equal("server=127.0.0.1;port=5432;database=DemoDB;user id=postgres;password=P@ssw0rd", postgreServerConnection);
        }

        [Fact]
        public void TestGetUnsecureConnectionString()
        {
            string sqlServerConnection = Configuration.GetSecureConnectionString("SqlServer-Unencrypted", Provider);
            string postgreServerConnection = Configuration.GetSecureConnectionString("PostgreSql-Unencrypted", Provider);

            Assert.Equal("data source=DemoDB;initial catalog=Demo;user id=sa;password=P@ssw0rd", sqlServerConnection);
            Assert.Equal("server=127.0.0.1;port=5432;database=DemoDB;user id=postgres;password=P@ssw0rd", postgreServerConnection);
        }
    }
}
