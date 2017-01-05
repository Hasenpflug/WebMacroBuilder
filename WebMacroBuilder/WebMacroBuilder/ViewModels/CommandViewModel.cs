using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMacroBuilder.Models;

namespace WebMacroBuilder.ViewModels
{
    public class CommandViewModel
    {
        public bool Enabled { get; set; }

        public string TaskBaseURL { get; set; }

        public ObjectId ID { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        public CommandType Type { get; set; }

        public string Selector { get; set; }

        public string WaitSelector { get; set; }

        public int WaitForSeconds { get; set; }

        public CommandViewModel()
        {

        }

        public CommandViewModel(ObjectId id, string baseURL, string name, int order, CommandType type, string selector, string waitSelector, int waitForSeconds)
        {
            ID = id;
            Name = name;
            Order = order;
            Type = type;
            Selector = selector;
            WaitSelector = waitSelector;
            WaitForSeconds = waitForSeconds;
        }        
    }
}
