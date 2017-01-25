using MongoDB.Bson;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;

namespace WebMacroBuilder
{
    public class ClickCommand : PanelItems, ICommandButton
    {
        public ButtonState State { get; set; }

        public string TaskBaseURL { get; set; }

        public bool Enabled { get; set; }

        public string Selector { get; set; }

        public string WaitSelector { get; set; }

        public int WaitForSeconds { get; set; }
        
        public ClickCommand(ObjectId commandID, ObjectId taskID, string taskBaseURL, int order, string name, bool enabled, string label, string target, string selector, string waitSelector, int waitForSeconds)
            : base(name)
        {
            ID = commandID;
            TaskID = taskID;
            TaskBaseURL = taskBaseURL;
            Order = order;
            Content = label;
            Name = name;
            Enabled = enabled;
            Target = target;
            Selector = selector;
            WaitSelector = waitSelector;
            WaitForSeconds = waitForSeconds;
        }

        public void Run(IWebDriver driver)
        {
            try
            {
                for (int i = 0; i < WaitForSeconds * 1000; i++)
			    {
			        if (i % 500 == 0)
                    {
                        IWebElement waitElement = (driver as IJavaScriptExecutor).ExecuteScript("return document.querySelector('" + WaitSelector + "')") as IWebElement;
                        if (waitElement != null)
                        {
                            IWebElement element = (driver as IJavaScriptExecutor).ExecuteScript("return document.querySelector('" + Selector + "')") as IWebElement;
                            element.Click();
                            break;
                        }
                    }
			    }
            }
            catch(WebDriverTimeoutException ex)
            {

            }
            catch(Exception ex)
            {

            }

            return;
        }

        public void Stop(IWebDriver driver)
        {
            return;
        }
    }
}
