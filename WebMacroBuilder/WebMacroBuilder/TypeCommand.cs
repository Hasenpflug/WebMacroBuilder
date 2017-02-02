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
    public class TypeCommand : PanelItems, ICommandButton
    {
        public ButtonState State { get; set; }

        public string TaskBaseURL { get; set; }

        public bool Enabled { get; set; }

        public string Selector { get; set; }

        public string WaitSelector { get; set; }

        public int WaitForSeconds { get; set; }

        public string SendKeysText { get; set; }

        public TypeCommand(ObjectId commandID, ObjectId taskID, string taskBaseURL, int order, string name, bool enabled, string label, string target, string selector,
            string waitSelector, int waitForSeconds, string sendKeysText)
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
            SendKeysText = sendKeysText;
        }

        public void Run(IWebDriver driver)
        {
            IWebElement element = (driver as IJavaScriptExecutor).ExecuteScript("return document.querySelector('" + Selector + "')") as IWebElement;
            element.SendKeys(SendKeysText);
        }

        public void Stop(IWebDriver driver)
        {
            return;
        }
    }
}
