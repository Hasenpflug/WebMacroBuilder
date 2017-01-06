using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WebMacroBuilder
{
    public class CommandCreator : PanelItems
    {
        public ObjectId CommandID { get; set; }

        public string CommandURL { get; set; }

        public CommandCreator(string name, string label)
        {
            Name = "btnCommandCreator";
            Content = ">>>";

        }

        public CommandCreator(int order, string baseURL, ObjectId taskID)
        {
            Content = ">>>";
            CommandURL = baseURL;
            Name = "btnCommandCreator";            
            Order = order;
            TaskID = taskID;
        }        
    }
}
