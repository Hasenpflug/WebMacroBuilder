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
        public ObjectId TaskID { get; set; }

        public bool Enabled { get; set; }

        public string TaskBaseURL { get; set; }

        public ObjectId ID { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        public CommandType Type { get; set; }

        public string Selector { get; set; }

        public string WaitSelector { get; set; }

        public int WaitForSeconds { get; set; }

        public string SendKeysText { get; set; }

        public string Database { get; set; }

        public string Collection { get; set; }

        public CommandViewModel()
        {

        }
    }
}
