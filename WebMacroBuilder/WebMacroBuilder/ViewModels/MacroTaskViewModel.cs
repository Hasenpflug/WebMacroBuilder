using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMacroBuilder.Models;

namespace WebMacroBuilder.ViewModels
{
    public class MacroTaskViewModel
    {
        public MacroTaskViewModel()
        {

        }

        public MacroTaskViewModel(string name, string baseURL)
        {

        }

        public ObjectId ID { get; set; }

        public string Name { get; set; }

        public string BaseUrl { get; set; }

        public List<CommandViewModel> Commands { get; set; }

        public int CommandCount { get; set; }
    }
}
