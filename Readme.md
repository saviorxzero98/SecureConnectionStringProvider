# 資料庫連線字串密碼加密處理

提供資料庫連線字串密碼加密處理

## ■ 使用說明

### (1) Using

```c#
using SecureConnectionString.Services;
```



### (2) Add Connection String

> * 加入一個 Connection String 到指定的 `appsettings.json`

```c#
var service = new AppSettingService();

service.AddOrUpdateConnectionString("TestDB", "Data Source=DemoDB;Initial Catalog=Demo;User Id=sa;Password=<Your Password>;");
```



### (3) Encrypt Connection String

> * 對連線字串密碼加密

```c#
// Provider & Helper
ISecureConnectionStringProvider provider = new DefaultSecureConnectionStringProvider();
SecureConnectionStringHelper helper = new SecureConnectionStringHelper(provider);

// Update
service.UpdateConnectionString("TestDB", (value) => helper.EncryptConnectionString(value));

// cs: "Data Source=DemoDB;Initial Catalog=Demo;User Id=sa;Password=<Encrypted Password>;"
string cs = service.GetConnectorString("TestDB");
```



### (4) Get Secure Connection String

> * 取得密碼已加密的連線字串

```c#
// cs: Data Source=DemoDB;Initial Catalog=Demo;User Id=sa;Password=<Your Password>;"
string cs = service.GetSecureConnectionString("TestDB", provider);
```



### (5) Delete Connection String

```c#
service.DeleteConnectionString("TestDB");
```



## ■ 判斷資料庫連線是否加密的邏輯

* 判斷連線字串是否有 "Is Encrypted Password" (預設值) 的 Session
    * 無這個 Session 或是值為 `false`，程式會判斷為密碼未加密
    * 值為 `true`，程式會判斷為密碼已加密
* 透過這個值來判斷是否有加密
* 資料庫使用時，Runtime 會移除這個 Session，不會影響資料庫的連線



### 修改判斷連線字串的 Session 名稱

```c#
// Provider & Helper
ISecureConnectionStringProvider provider = new DefaultSecureConnectionStringProvider();

var options = new SecureConnectionStringOption() 
{
    // Password Session
    TargetSession = "Password",
    
    // Encrypted State Session
    StateSession = "Is Encrypted Password"
};
var helper = new SecureConnectionStringHelper(provider, options);
```



---

## ■ 客製 Secure Connection String Provider

* **繼承 `ISecureConnectionStringProvider`**
    * 並實作 `Encrypt` 和 `Decrypt` 函式即可
* **Example**

```c#
public class MyCustomSecureConnectionStringProvider : ISecureConnectionStringProvider
{
    public string Encrypt(SecureString securePlainText) 
    {
        // TODO...
    }
    
    public SecureString Decrypt(string secretText)
    {
        // TODO...
    }
}
```



