using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        
        public ClickCommand(string taskBaseURL, int order, string name, bool enabled, string label, string target, string selector, string waitSelector, int waitForSeconds)
            : base(name)
        {
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
            return;
        }

        public void Stop(IWebDriver driver)
        {
            return;
        }
    }
}
