using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMacroBuilder.Models
{
    public class MacroTask
    {
        public MacroTask()
        {

        }

        public MacroTask(string name, string baseUrl)
        {
            this.BaseUrl = baseUrl;
            this.Name = name;
        }

        public string ID { get; set; }

        public string Name { get; set; }

        public string BaseUrl { get; set; }
    }
}
