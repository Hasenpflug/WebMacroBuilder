using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WebMacroBuilder
{
    public abstract class PanelItems : Button
    {
        const int DEFAULT_WIDTH = 60;
        const int DEFAULT_HEIGHT = 60;
        const int PADDING = 20;

        public new string Name { get; set; }

        public int Order { get; set; }

        public string Target { get; set; }

        public ObjectId TaskID { get; set; }

        public PanelItems(string name = null)
        {
            Name = name;
            Padding = new System.Windows.Thickness(PADDING);
        }

        public PanelItems(string name, string label)
        {
            Name = name;
            Content = label;
        }
    }
}
