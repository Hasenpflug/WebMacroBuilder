using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMacroBuilder
{
    public enum ButtonState
    {
        Idle, 
        Running, 
        Error,
        Stopped
    }

    interface ICommandButton
    {
        string TaskBaseURL { get; set; }

        string Name { get; set; }

        string Target { get; set; }

        ButtonState State { get; set;}

        void Run(IWebDriver driver);

        void Stop(IWebDriver driver);
    }
}
