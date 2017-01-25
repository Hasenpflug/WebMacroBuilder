using MongoDB.Driver;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MongoDB.Bson;
using WebMacroBuilder.Models;
using WebMacroBuilder.ViewModels;
using Application = System.Windows.Application;

namespace WebMacroBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IWebDriver driver;

        public DbContext Context { get; set; }

        public TaskRunner _taskRunner;

        public TaskRunner TaskRunner
        {
            get
            {
                if (_taskRunner == null)
                {
                    _taskRunner = new TaskRunner(pnlCommandRunner.Children.OfType<ICommandButton>().ToList());
                }

                return _taskRunner;
            }
        }
        
        public MainWindow()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            try
            {
                Context = new DbContext();
            }
            catch(Exception ex)
            {
                this.Close();
            }

            ShowTaskIndex();            
        }

        private void btnTaskCreate_Click(object sender, RoutedEventArgs e)
        {
            TaskCreate taskCreateWindow = new TaskCreate();
            taskCreateWindow.Show();
        }

        public async Task ShowTaskDetails(ObjectId taskID)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;

            window.Nodes.Visibility = Visibility.Visible;
            window.Tasks.Visibility = Visibility.Collapsed;
            window.Actions.Visibility = Visibility.Collapsed;
            await PopulateNodeGrid(taskID);
        }

        public async Task ShowTaskRunner(ObjectId taskID)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;

            window.Actions.Visibility = Visibility.Visible;
            window.Nodes.Visibility = Visibility.Collapsed;
            window.Tasks.Visibility = Visibility.Collapsed;
            await PopulateActionsGrid(taskID);
            btnPauseTask.IsEnabled = false;
            btnStopTask.IsEnabled = false;
        }

        public async Task PopulateNodeGrid(ObjectId taskID)
        {
            string taskBaseURL = (await Context.GetTask(taskID, null)).BaseUrl;
            List<CommandViewModel> commands = (await Context.GetTaskCommands(taskID)).OrderBy(m => m.Order).ToList();
            MainWindow window = (MainWindow)Application.Current.MainWindow;

            window.pnlCommandViewer.Children.Clear();
            window.pnlCommandViewer.Children.Add(new StartNode());
            CommandCreator beginningCreator = new CommandCreator(0, taskBaseURL, taskID);
            beginningCreator.Click += btnCommandCreator_Click;
            window.pnlCommandViewer.Children.Add(beginningCreator);
            for (int i = 0; i < commands.Count; i++)
            {
                switch (commands[i].Type)
                {
                    case CommandType.Click:
                        ClickCommand clickCommand = new ClickCommand(commands[i].ID, commands[i].TaskID, commands[i].TaskBaseURL, commands[i].Order, "btn" + commands[i].Name, commands[i].Enabled, commands[i].Name, 
                            commands[i].TaskBaseURL, commands[i].Selector, commands[i].WaitSelector, commands[i].WaitForSeconds);
                        clickCommand.Click += btnCommandCreator_Click;
                        window.pnlCommandViewer.Children.Add(clickCommand);
                        break;
                    case CommandType.Type:
                        TypeCommand typeCommand = new TypeCommand(commands[i].ID, commands[i].TaskID, commands[i].TaskBaseURL, commands[i].Order, "btn" + commands[i].Name, commands[i].Enabled, commands[i].Name, 
                            commands[i].TaskBaseURL, commands[i].Selector, commands[i].WaitSelector, commands[i].WaitForSeconds, commands[i].SendKeysText);
                        typeCommand.Click += btnCommandCreator_Click;
                        window.pnlCommandViewer.Children.Add(typeCommand);
                        break;
                }

                CommandCreator endingCreator = new CommandCreator(i + 1, taskBaseURL, taskID);
                endingCreator.Click += btnCommandCreator_Click;
                window.pnlCommandViewer.Children.Add(endingCreator);
            }

            window.pnlCommandViewer.Children.Add(new EndNode());
        }

        public async Task PopulateActionsGrid(ObjectId taskID)
        {
            string taskBaseURL = (await Context.GetTask(taskID, null)).BaseUrl;
            List<CommandViewModel> commands = (await Context.GetTaskCommands(taskID)).OrderBy(m => m.Order).ToList();
            MainWindow window = (MainWindow)Application.Current.MainWindow;

            window.pnlCommandRunner.Children.Clear();
            window.pnlCommandRunner.Children.Add(new StartNode());
            for (int i = 0; i < commands.Count; i++)
            {
                switch (commands[i].Type)
                {
                    case CommandType.Click:
                        ClickCommand clickCommand = new ClickCommand(commands[i].ID, commands[i].TaskID, commands[i].TaskBaseURL, commands[i].Order, "btn" + commands[i].Name, commands[i].Enabled, commands[i].Name,
                            commands[i].TaskBaseURL, commands[i].Selector, commands[i].WaitSelector, commands[i].WaitForSeconds);
                        window.pnlCommandRunner.Children.Add(clickCommand);
                        break;
                    case CommandType.Type:
                        TypeCommand typeCommand = new TypeCommand(commands[i].ID, commands[i].TaskID, commands[i].TaskBaseURL, commands[i].Order, "btn" + commands[i].Name, commands[i].Enabled, commands[i].Name,
                            commands[i].TaskBaseURL, commands[i].Selector, commands[i].WaitSelector, commands[i].WaitForSeconds, commands[i].SendKeysText);
                        window.pnlCommandRunner.Children.Add(typeCommand);
                        break;
                }
            }

            window.pnlCommandRunner.Children.Add(new EndNode());
        }

        public void btnCommandCreator_Click(object sender, RoutedEventArgs e)
        {
            if (sender.GetType() == typeof(CommandCreator))
            {
                ShowCommandCreate((CommandCreator)sender);
            }
            else if (sender.GetType() == typeof(ClickCommand))
            {
                ShowClickCommandUpdate((ClickCommand)sender);
            }
            else if (sender.GetType() == typeof(TypeCommand))
            {
                ShowTypeCommandUpdate((TypeCommand)sender);
            }
        }

        public void ShowCommandCreate(CommandCreator creator)
        {
            CommandCreate commandCreateWindow = new CommandCreate(creator);
            commandCreateWindow.Show();
        }

        public void ShowClickCommandUpdate(ClickCommand command)
        {
            CommandCreate commandCreateWindow = new CommandCreate(command);
            commandCreateWindow.Show();
        }

        public void ShowTypeCommandUpdate(TypeCommand command)
        {
            CommandCreate commandCreateWindow = new CommandCreate(command);
            commandCreateWindow.Show();
        }

        public async Task ShowTaskIndex()
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            
            window.dtgTasks.ItemsSource = await Context.GetAllTasks();
            window.Tasks.Visibility = Visibility.Visible;
            window.Nodes.Visibility = Visibility.Collapsed;
            window.Actions.Visibility = Visibility.Collapsed;
        }

        private async Task DeleteTask(List<ObjectId> IDs)
        {
            await Context.DeleteTasks(IDs);
            await ShowTaskIndex();
        }

        private async void btnTaskDelete_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            List<ObjectId> tasksToDelete = new List<ObjectId>();

            if (window.dtgTasks.SelectedIndex != -1)
            {
                foreach (MacroTaskViewModel viewModel in window.dtgTasks.SelectedItems)
                {
                    tasksToDelete.Add(viewModel.ID);
                };

                await DeleteTask(tasksToDelete);
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await ShowTaskIndex();
        }

        private async void btnTaskEdit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            ObjectId taskToView;

            try
            {
                taskToView = ((MacroTaskViewModel)window.dtgTasks.SelectedItem).ID;
                await ShowTaskDetails(taskToView);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Please select a Task to edit. ");
            }
        }

        private async void btnTaskRun_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            ObjectId taskToView;

            try
            {
                taskToView = ((MacroTaskViewModel)window.dtgTasks.SelectedItem).ID;
                await ShowTaskRunner(taskToView);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Please select a Task to run. ");
            }
        }

        private void btnStartTask_Click(object sender, RoutedEventArgs e)
        {
            btnPauseTask.IsEnabled = true;
            btnStopTask.IsEnabled = true;
            btnStartTask.IsEnabled = false;
            TaskRunner.StartDriver();
        }

        private void btnStopTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskRunner != null && TaskRunner.RunningState == RunnerState.Running)
            {
                TaskRunner.QuitDriver();
                btnPauseTask.IsEnabled = false;
                btnStopTask.IsEnabled = false;
                btnStartTask.IsEnabled = true;
            }
        }

        private void btnPauseTask_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
