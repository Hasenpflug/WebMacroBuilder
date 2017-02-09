using MongoDB.Bson;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMacroBuilder
{
    public class SaveCommand : PanelItems, ICommandButton
    {
        public ButtonState State { get; set; }

        public string TaskBaseURL { get; set; }

        public bool Enabled { get; set; }

        public string Selector { get; set; }

        public string WaitSelector { get; set; }

        public int WaitForSeconds { get; set; }

        public string AttributeValue { get; set; }

        public string Collection { get; set; }

        public SaveCommand(ObjectId commandID, ObjectId taskID, string taskBaseURL, int order, string name, bool enabled, string label, 
            string target, string selector, string waitSelector, int waitForSeconds, string attributeValue, string collection)
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
            AttributeValue = attributeValue;
            Collection = collection;
        }

        public void Run(IWebDriver driver)
        {
            (new WebDriverWait(driver, TimeSpan.FromSeconds(5))).Until(m => (m as IJavaScriptExecutor).ExecuteScript("return document.querySelector('" + WaitSelector + "')") as IWebElement);
            IWebElement element = (driver as IJavaScriptExecutor).ExecuteScript("return document.querySelector('" + Selector + "')") as IWebElement;
        }

        public void Stop(IWebDriver driver)
        {
            return;
        }
    }
}
