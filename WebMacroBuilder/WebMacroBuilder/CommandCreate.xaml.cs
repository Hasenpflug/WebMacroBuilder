using MongoDB.Bson;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WebMacroBuilder.Models;
using ComboBox = System.Windows.Controls.ComboBox;

namespace WebMacroBuilder
{
    /// <summary>
    /// Interaction logic for CommandCreate.xaml
    /// </summary>
    public partial class CommandCreate : Window
    {
        private bool isInitializing = true;

        private IWebDriver Driver { get; set; }

        private Command Command { get; set; }

        private string CommandURL { get; set; }

        public CommandCreate()
        {
            isInitializing = true;
            InitializeComponent();
            isInitializing = false;
            MainWindow window = (MainWindow)System.Windows.Application.Current.MainWindow;            
            cboSaveCollection.ItemsSource = window.Context.GetCollections();
        }

        public CommandCreate(CommandCreator creator)
            : this()
        {
            CommandURL = creator.CommandURL;
            Command = new Command(ObjectId.Empty, creator.TaskID, "", creator.Order, CommandType.Click, true, "", "", 0);            
        }

        public CommandCreate(ClickCommand creator)
            : this()
        {
            CommandURL = creator.TaskBaseURL;
            Command = new Command(creator.ID, creator.TaskID, creator.Name, creator.Order, CommandType.Click, creator.Enabled, creator.Selector, creator.WaitSelector, creator.WaitForSeconds);
            this.lblCommandCreate.Content = "Command Update";
            this.txtName.Text = Command.Name;
            this.cboType.SelectedIndex = (int)Command.Type;
            this.chkEnabled.IsChecked = Command.Enabled;
            this.txtSelector.Text = Command.Selector;
            this.txtWaitSelector.Text = Command.WaitSelector;
            this.txtWaitFor.Text = Command.WaitForSeconds.ToString();
        }

        public CommandCreate(TypeCommand creator)
            : this()
        {
            CommandURL = creator.TaskBaseURL;
            Command = new Command(creator.ID, creator.TaskID, creator.Name, creator.Order, CommandType.Click, creator.Enabled, creator.Selector, creator.WaitSelector, creator.WaitForSeconds);
            this.lblCommandCreate.Content = "Command Update";
            this.txtName.Text = Command.Name;
            this.cboType.SelectedIndex = (int)Command.Type;
            this.chkEnabled.IsChecked = Command.Enabled;
            this.txtSelector.Text = Command.Selector;
            this.txtWaitSelector.Text = Command.WaitSelector;
            this.txtWaitFor.Text = Command.WaitForSeconds.ToString();
            this.txtSendKeysText.Text = Command.SendKeysText;
        }

        public CommandCreate(SaveCommand creator)
            : this()
        {
            MainWindow window = (MainWindow)System.Windows.Application.Current.MainWindow;
            CommandURL = creator.TaskBaseURL;
            Command = new Command(creator.ID, creator.TaskID, creator.Name, creator.Order, CommandType.Click, creator.Enabled, creator.Selector, creator.WaitSelector, creator.WaitForSeconds);
            this.lblCommandCreate.Content = "Command Update";
            this.txtName.Text = Command.Name;
            this.cboType.SelectedIndex = (int)Command.Type;
            this.chkEnabled.IsChecked = Command.Enabled;
            this.txtSelector.Text = Command.Selector;
            this.txtWaitSelector.Text = Command.WaitSelector;
            this.txtWaitFor.Text = Command.WaitForSeconds.ToString();
        }

        private void cboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitializing)
            {
                return;
            }

            string type = (e.AddedItems[0] as ListBoxItem).Content.ToString();

            switch (type)
            {
                case "Click":
                    this.dtgClickForm.Visibility = Visibility.Visible;
                    this.dtgSendKeysForm.Visibility = Visibility.Collapsed;
                    this.dtgSaveForm.Visibility = Visibility.Collapsed;
                    break;
                case "Type":
                    this.dtgSendKeysForm.Visibility = Visibility.Visible;
                    this.dtgClickForm.Visibility = Visibility.Collapsed;
                    this.dtgSaveForm.Visibility = Visibility.Collapsed;
                    break;
                case "Save":
                    this.dtgSaveForm.Visibility = Visibility.Visible;
                    this.dtgSendKeysForm.Visibility = Visibility.Collapsed;
                    this.dtgClickForm.Visibility = Visibility.Collapsed;                    
                    break;
            }
        }

        private async void btnClick_Submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Command commandToCreate = BuildNewCommand();

                commandToCreate.SendKeysText = this.txtSendKeysText.Text;
                MainWindow window = (MainWindow)System.Windows.Application.Current.MainWindow;
                await window.Context.InsertCommand(commandToCreate);
                await window.PopulateNodeGrid(Command.TaskID);
                this.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Something went wrong with the conversion probably: " + ex.Message);
            }
        }

        private void btnClick_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_GetSelector_Click(object sender, RoutedEventArgs e)
        {
            if (Driver is IJavaScriptExecutor)
            {
                string selector = ((IJavaScriptExecutor)Driver).ExecuteScript("return window.selector;").ToString();
                this.txtSelector.Text = selector;
            }
        }

        private async void btnAddSelector_Click(object sender, RoutedEventArgs e)
        {
            if (Driver != null)
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript(File.ReadAllText(@"..\..\InitScripts\getSelector.js"));
                return;
            }

            this.btnClickSubmit.IsEnabled = false;
            this.btnClickDelete.IsEnabled = false;
            this.btnClickCancel.IsEnabled = false;
            await Task.Run(() => StartDriver());
            this.btnGetSelector.IsEnabled = true;
            this.btnWaitGetSelector.IsEnabled = true;
            this.btnQuitDriver.IsEnabled = true;
        }

        private void StartDriver()
        {
            Driver = new ChromeDriver();
            Driver.Navigate().GoToUrl(CommandURL);

            if (Driver is IJavaScriptExecutor)
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript(File.ReadAllText(@"..\..\InitScripts\getSelector.js"));
            }
        }

        private async void btnQuitDriver_Click(object sender, RoutedEventArgs e)
        {
            this.btnAddSelector.IsEnabled = false;
            this.btnGetSelector.IsEnabled = false;
            this.btnWaitGetSelector.IsEnabled = false;
            this.btnQuitDriver.IsEnabled = false;
            await Task.Run(() => QuitDriver());
            this.btnAddSelector.IsEnabled = true;
            this.btnClickSubmit.IsEnabled = true;
            this.btnClickDelete.IsEnabled = true;
            this.btnClickCancel.IsEnabled = true;
        }

        private void QuitDriver()
        {
            Driver.Quit();
            Driver.Dispose();
            Driver = null;
        }

        private void btnWaitGetSelector_Click(object sender, RoutedEventArgs e)
        {
            if (Driver is IJavaScriptExecutor)
            {
                string selector = ((IJavaScriptExecutor)Driver).ExecuteScript("return window.selector;").ToString();
                this.txtWaitSelector.Text = selector;
            }
        }

        private async void btnDeleteCommand_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)System.Windows.Application.Current.MainWindow;
            await window.Context.DeleteCommand(Command);
            await window.PopulateNodeGrid(Command.TaskID);
            this.Close();
        }

        private Command BuildNewCommand()
        {
            Command commandToCreate = new Command()
            {
                Enabled = this.chkEnabled.IsChecked.Value,
                Name = this.txtName.Text,
                Order = Command.Order,
                Selector = this.txtSelector.Text,
                TaskID = Command.TaskID,
                Type = (CommandType)this.cboType.SelectedIndex,
                WaitForSeconds = !String.IsNullOrEmpty(this.txtWaitFor.Text) ? Convert.ToInt16(this.txtWaitFor.Text) : 0,
                WaitSelector = this.txtWaitSelector.Text
            };

            return commandToCreate;
        }

        private async void btnSendKeys_Submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Command commandToCreate = BuildNewCommand();

                commandToCreate.SendKeysText = txtSendKeysText.Text;
                MainWindow window = (MainWindow)System.Windows.Application.Current.MainWindow;
                await window.Context.InsertCommand(commandToCreate);
                await window.PopulateNodeGrid(Command.TaskID);
                this.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Something went wrong with the conversion probably: " + ex.Message);
            }
        }

        private async void btnSaveSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Command commandToCreate = BuildNewCommand();
                bool isCustomCollection = !String.IsNullOrEmpty(txtSaveCustomCollection.Text);
                commandToCreate.Collection = !isCustomCollection ? cboSaveCollection.Text : txtSaveCustomCollection.Text;
                commandToCreate.AttributeValue = cboSaveAttribute.Text;
                MainWindow window = (MainWindow)System.Windows.Application.Current.MainWindow;
                await window.Context.InsertCommand(commandToCreate);
                await window.PopulateNodeGrid(Command.TaskID);
                                
                this.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Something went wrong with the conversion probably: " + ex.Message);
            }
        }
    }
}
