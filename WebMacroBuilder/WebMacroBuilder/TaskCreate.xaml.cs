using System;
using System.Collections.Generic;
using System.Linq;
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
using MongoDB.Bson;
using WebMacroBuilder.Models;

namespace WebMacroBuilder
{
    /// <summary>
    /// Interaction logic for TaskCreate.xaml
    /// </summary>
    public partial class TaskCreate : Window
    {
        public TaskCreate()
        {
            InitializeComponent();
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string taskName = this.txtName.Text;
                string taskBaseUrl = "http://" + this.txtBaseUrl.Text;
                MacroTask taskToCreate = new MacroTask(taskName, taskBaseUrl);
                MainWindow window = (MainWindow)System.Windows.Application.Current.MainWindow;

                await window.Context.InsertTask(taskToCreate);
                ObjectId taskID = (await window.Context.GetTask(ObjectId.Empty, taskName)).ID;
                await window.ShowTaskDetails(taskID);
                this.Close();
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
    }
}
