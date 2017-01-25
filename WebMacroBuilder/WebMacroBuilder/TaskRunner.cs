using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMacroBuilder
{
    public enum RunnerState
    {
        Running,
        Paused,
        Stopped
    }

    public class TaskRunner
    {
        public IWebDriver Driver { get; set; }

        public RunnerState RunningState { get; set; }

        public List<ICommandButton> Commands { get; set; }

        public TaskRunner()
        {

        }

        public TaskRunner(List<ICommandButton> commands)
        {
            Commands = commands;
        }

        public void StartDriver()
        {
            Driver = new ChromeDriver();
            Driver.Manage().Window.Maximize();
            Driver.Navigate().GoToUrl(Commands[0].Target);
            RunCommands();
        }

        public void RunCommands()
        {
            foreach (ICommandButton command in Commands)
            {
                command.Run(Driver);
            }
        }

        public void QuitDriver()
        {
            Driver.Quit();
        }
    }
}
