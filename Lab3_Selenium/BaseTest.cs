using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
 
namespace Lab3_Selenium
{
    public abstract class BaseTest : IDisposable
    {
        protected const string BaseUrl = "https://the-internet.herokuapp.com";
        protected readonly IWebDriver Driver;
        protected readonly WebDriverWait Wait;
 
        protected BaseTest()
        {
            var options = new ChromeOptions();
            options.AddArgument("--window-size=1400,900");
 
            Driver = new ChromeDriver(options);
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        }
        
        protected void Open(string path) => Driver.Navigate().GoToUrl(BaseUrl + path);
 
        public void Dispose()
        {
            Driver.Quit();
            GC.SuppressFinalize(this);
        }
    }
}