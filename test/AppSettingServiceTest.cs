using SecureConnectionString.Services;
using Xunit;

namespace SecureConnectionString.Test
{
    public class AppSettingServiceTest
    {
        [Fact]
        public void TestAppSetting()
        {
            var service = new AppSettingService();

            #region Add

            // Add AppSettings
            bool addSettingResult = service.AddOrUpdateAppSetting("TestSetting", new { Test = "Value" });
            bool addSubSettingResult = service.AddOrUpdateAppSetting("TestSetting:Test2", "SubValue");
            string addSettingValue = service.GetSetting("TestSetting:Test");
            string addSubSettingValue = service.GetSetting("TestSetting:Test2");

            // Assert Add AppSettings
            Assert.True(addSettingResult);
            Assert.True(addSubSettingResult);
            Assert.Equal("Value", addSettingValue);
            Assert.Equal("SubValue", addSubSettingValue);

            #endregion


            #region Update

            // Update AppSettings
            bool updateSettingResult = service.AddOrUpdateAppSetting("TestSetting:Test", "Value2");
            bool updateSubSettingResult = service.AddOrUpdateAppSetting("TestSetting:Test2", "SubValue2");

            string updateSettingValue = service.GetSetting("TestSetting:Test");
            string updateSubSettingValue = service.GetSetting("TestSetting:Test2");

            // Assert Update AppSettings
            Assert.True(updateSettingResult);
            Assert.True(updateSubSettingResult);
            Assert.Equal("Value2", updateSettingValue);
            Assert.Equal("SubValue2", updateSubSettingValue);

            #endregion


            #region Delete

            // Delete AppSettings
            bool deleteSubSettingResult = service.DeleteAppSetting("TestSetting:Test2");
            bool deleteSettingResult = service.DeleteAppSetting("TestSetting");

            // Assert Delete AppSettings
            Assert.True(deleteSettingResult);
            Assert.True(deleteSubSettingResult);

            #endregion
        }

        [Fact]
        public void TestConnectionString()
        {
            var service = new AppSettingService();


            #region Add

            // Add Connection String
            bool addConnectionResult = service.AddOrUpdateConnectionString("TestDB", "Data Source=DemoDB;Initial Catalog=Demo;User Id=sa;Password=P@ssw0rd;");
            string addConnectionValue = service.GetConnectorString("TestDB");

            // Assert Add Connection String
            Assert.True(addConnectionResult);
            Assert.Equal("Data Source=DemoDB;Initial Catalog=Demo;User Id=sa;Password=P@ssw0rd;", addConnectionValue);

            #endregion


            #region Update

            // Update Connection String
            bool updateConnectionResult = service.AddOrUpdateConnectionString("TestDB", "Data Source=DemoDB;Initial Catalog=Demo;User Id=sa;Password=P@5sw0rd;");
            string updateConnectionValue = service.GetConnectorString("TestDB");

            // Assert Update Connection String
            Assert.True(updateConnectionResult);
            Assert.Equal("Data Source=DemoDB;Initial Catalog=Demo;User Id=sa;Password=P@5sw0rd;", updateConnectionValue);

            #endregion


            #region Delete

            // Delete Connection String
            bool deleteConnectionResult = service.DeleteConnectionString("TestDB");

            // Assert Delete Connection String
            Assert.True(deleteConnectionResult);

            #endregion
        }
    }
}
