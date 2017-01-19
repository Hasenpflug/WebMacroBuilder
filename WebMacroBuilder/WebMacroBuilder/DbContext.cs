using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebMacroBuilder.Models;
using WebMacroBuilder.ViewModels;

namespace WebMacroBuilder
{
    public class DbContext
    {
        private IMongoClient client;
        private IMongoDatabase database;

        public DbContext()
        {
            client = new MongoClient("mongodb://localhost");
            database = client.GetDatabase("macros");

            if (client == null || database == null)
            {
                throw new Exception();
            }
        }

        public async Task<List<CommandViewModel>> GetTaskCommands(ObjectId taskID)
        {
            string taskBaseURL = "";
            var collection = database.GetCollection<BsonDocument>("commands").Aggregate().Lookup("tasks", "TaskID", "_id", "Tasks");
            var commandsFilter = Builders<BsonDocument>.Filter.Eq("TaskID", taskID);
            var taskFilter = Builders<BsonDocument>.Filter.Eq("_id", taskID);

            List<CommandViewModel> viewModels = new List<CommandViewModel>();

            using (var cursor = await collection.ToCursorAsync())
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (var document in batch)
                    {
                        CommandViewModel viewModel = new CommandViewModel
                        {
                            Enabled = document.GetValue("Enabled") != BsonNull.Value ? document.GetValue("Enabled").AsBoolean : false,
                            ID = document.GetValue("_id").AsObjectId,
                            TaskID = document.GetValue("TaskID").AsObjectId,
                            TaskBaseURL = document.GetValue("Tasks")[0].AsBsonDocument.GetValue("BaseUrl") != BsonNull.Value ? document.GetValue("Tasks")[0].AsBsonDocument.GetValue("BaseUrl").AsString : "",
                            Name = document.GetValue("Name") != BsonNull.Value ? document.GetValue("Name").AsString : null,
                            Order = document.GetValue("Order") != BsonNull.Value ? document.GetValue("Order").AsInt32 : 0,
                            Type = document.GetValue("Type") != BsonNull.Value ? (CommandType)document.GetValue("Type").AsInt32 : CommandType.Click,
                            Selector = document.GetValue("Selector") != BsonNull.Value ? document.GetValue("Selector").AsString : "",
                            WaitSelector = document.GetValue("WaitSelector") != BsonNull.Value ? document.GetValue("WaitSelector").AsString : "",
                            WaitForSeconds = document.GetValue("WaitForSeconds") != BsonNull.Value ? document.GetValue("WaitForSeconds").AsInt32 : 0
                        };

                        viewModels.Add(viewModel);
                    }
                }
            }

            return viewModels;
        }

        public async Task<MacroTaskViewModel> GetTask(ObjectId taskID, string name)
        {
            var collection = database.GetCollection<BsonDocument>("tasks");
            var filter = Builders<BsonDocument>.Filter.Empty;

            if (taskID != ObjectId.Empty)
            {
                filter = Builders<BsonDocument>.Filter.Eq("_id", taskID);
            }
            else
            {
                filter = Builders<BsonDocument>.Filter.Eq("Name", name);
            }

            BsonDocument document = await collection.Find(filter).FirstOrDefaultAsync();
            if (document != null)
            {
                MacroTaskViewModel viewModel = new MacroTaskViewModel
                {
                    ID = (ObjectId)document.GetValue("_id"),
                    Name = document.GetValue("Name") != BsonNull.Value ? document.GetValue("Name").ToString() : null,
                    BaseUrl = document.GetValue("BaseUrl") != BsonNull.Value ? document.GetValue("BaseUrl").ToString() : null,
                    Commands = await GetTaskCommands((ObjectId)document.GetValue("_id"))
                    //CommandCount = document.GetValue("Commands").AsBsonArray != BsonNull.Value ? document.GetValue("Commands").AsBsonArray.Count : 0
                };

                return viewModel;
            }
            else
            {
                throw new Exception("Could not find the specified Task.");
            }
        }

        public async Task<List<MacroTaskViewModel>> GetAllTasks()
        {
            try
            {
                List<MacroTaskViewModel> viewModels = new List<MacroTaskViewModel>();
                var taskCollection = database.GetCollection<BsonDocument>("tasks");
                var filter = new BsonDocument();

                using (var cursor = await taskCollection.FindAsync(filter))
                {
                    while (await cursor.MoveNextAsync())
                    {
                        var batch = cursor.Current;
                        foreach (var document in batch)
                        {
                            MacroTaskViewModel viewModel = new MacroTaskViewModel
                            {
                                ID = (ObjectId)document.GetValue("_id"),
                                Name = document.GetValue("Name") != BsonNull.Value ? document.GetValue("Name").ToString() : null,
                                BaseUrl = document.GetValue("BaseUrl") != BsonNull.Value ? document.GetValue("BaseUrl").ToString() : null,
                                Commands = new List<CommandViewModel>(),
                                //CommandCount = document.GetValue("Commands").AsBsonArray != BsonNull.Value ? document.GetValue("Commands").AsBsonArray.Count : 0
                            };

                            if (viewModel.CommandCount != 0)
                            {
                                foreach (BsonValue element in document.GetValue("Commands").AsBsonArray)
                                {
                                    viewModel.Commands.Add(DbContextTools.PopulateCommandViewModel(element));
                                }
                            }

                            viewModels.Add(viewModel);
                        }
                    }
                }

                return viewModels;
            }
            catch (Exception ex)
            {
                return new List<MacroTaskViewModel>();
            }
        }

        public async Task InsertCommand(Command command)
        {
            var collection = database.GetCollection<BsonDocument>("commands");
            var filter = Builders<BsonDocument>.Filter.Eq("TaskID", command.TaskID) & Builders<BsonDocument>.Filter.Gte("Order", command.Order) & !Builders<BsonDocument>.Filter.Eq("Name", command.Name);
            BsonDocument document = command.ToBsonDocument();
            document.Remove("ID");

            await collection.InsertOneAsync(document);
            await collection.UpdateManyAsync(filter, Builders<BsonDocument>.Update.Inc("Order", 1));
        }

        public async Task InsertTask(MacroTask task)
        {
            var collection = database.GetCollection<BsonDocument>("tasks");
            BsonDocument document = task.ToBsonDocument();

            await collection.InsertOneAsync(document);
        }

        public async Task DeleteTasks(List<ObjectId> taskIDs)
        {
            var collection = database.GetCollection<BsonDocument>("tasks");
            var filter = Builders<BsonDocument>.Filter.Eq("_id", taskIDs[0]);

            for (int i = 1; i < taskIDs.Count; i++)
            {
                filter = filter | Builders<BsonDocument>.Filter.Eq("_id", taskIDs[i]);
            }

            await collection.DeleteManyAsync(filter);

            for (int i = 0; i < taskIDs.Count; i++)
            {
                await DeleteTaskCommands(taskIDs[i]);
            }
        }

        public async Task DeleteTaskCommands(ObjectId taskID)
        {
            var collection = database.GetCollection<BsonDocument>("commands");
            var filter = Builders<BsonDocument>.Filter.Eq("TaskID", taskID);

            await collection.DeleteManyAsync(filter);
        }

        public async Task DeleteCommand(Command command)
        {
            var collection = database.GetCollection<BsonDocument>("commands");
            var deleteFilter = Builders<BsonDocument>.Filter.Eq("_id", command.ID);
            var updateFilter = Builders<BsonDocument>.Filter.Gte("Order", command.Order);

            await collection.DeleteOneAsync(deleteFilter);
            await collection.UpdateManyAsync(updateFilter, Builders<BsonDocument>.Update.Inc("Order", -1));
        }
    }
}
