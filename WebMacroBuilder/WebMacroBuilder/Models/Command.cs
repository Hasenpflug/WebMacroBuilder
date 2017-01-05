﻿using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMacroBuilder.Models
{
    public enum CommandType
    {
        Click = 0,
        Type,
        Save,
        Check,
        Loop
    }

    public class Command
    {
        public Command() { }

        public Command(ObjectId taskID, string name, int order, CommandType type, bool enabled, string selector, string waitSelector, int waitForSeconds)
        {
            TaskID = taskID;
            Name = name;
            Type = type;
            Enabled = enabled;
            Order = order;
            Selector = selector;
            WaitSelector = waitSelector;
            WaitForSeconds = waitForSeconds;
        }        

        public bool Enabled { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        public string Selector { get; set; }

        public ObjectId TaskID { get; set; }

        public CommandType Type { get; set; }

        public string WaitSelector { get; set; }

        public int WaitForSeconds { get; set; }
    }
}