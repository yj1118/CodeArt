using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;
using CodeArt.TestTools;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace CodeArt.Selenium
{
    public class WebRunner : IWebRunner
    {
        protected IWebDriver Driver
        {
            get;
            private set;
        }

        public WebRunner()
        {
            var config = SeleniumConfiguration.Current.DriverConfig;
            switch (config.Type)
            {
                case "chrome":
                    this.Driver = new ChromeDriver(config.Directory);
                    break;
                case "firefox":
                    this.Driver = new FirefoxDriver(config.Directory);
                    break;
                case "ie":
                    this.Driver = new InternetExplorerDriver(config.Directory);
                    break;
                default:
                    throw new TestException("浏览器的类型配置不正确");
            }
        }

        public void AddCookie()
        {
            
        }

        public void ClearCookie()
        {
            //this.Driver.Manage().Cookies.DeleteAllCookies();
        }

        public void DeleteCookie(string name)
        {

        }

        public void GetCookie(string name)
        {
      
        }
    }
}
