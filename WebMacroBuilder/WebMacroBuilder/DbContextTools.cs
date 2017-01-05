using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMacroBuilder.Models;
using WebMacroBuilder.ViewModels;

namespace WebMacroBuilder
{
    public static class DbContextTools
    {
        public static CommandViewModel PopulateCommandViewModel(BsonValue element)
        {
            CommandType type = (CommandType)element["Type"].AsInt32;

            switch (type)
            {
                case CommandType.Click:
                    return new CommandViewModel
                        {
                            Name = element["Name"].AsString,
                            Order = element["Order"].AsInt32,
                            Selector = element["Selector"].AsString,
                            Type = CommandType.Click
                        };
                case CommandType.Type:
                    return new CommandViewModel();
                default:
                    throw new Exception("Command type could not be found");
            }
            
        }
    }
}
