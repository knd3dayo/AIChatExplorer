using System.Text.Json.Serialization;
using PythonAILib.Common;

namespace PythonAILib.Model.AutoGen {
    public class AutoGenTool {
        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.NewObjectId();

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("source_path")]
        public string SourcePath { get; set; } = "";

        // ToDictList
        public static Dictionary<string, object> ToDict(AutoGenTool data) {
            // Create a dictionary
            Dictionary<string, object> dict = new Dictionary<string, object> {
                { "name", data.Name },
                { "description", data.Description },
                { "source_path", data.SourcePath },
            };
            return dict;
        }
        // ToDictList
        public static List<Dictionary<string, object>> ToDictList(List<AutoGenTool> data) {
            List<Dictionary<string, object>> dictList = new List<Dictionary<string, object>>();
            foreach (AutoGenTool item in data) {
                dictList.Add(ToDict(item));
            }
            return dictList;
        }

        public void Save() {
            var collection = PythonAILibManager.Instance.DataFactory.GetAutoGenToolCollection<AutoGenTool>();
            var items = collection.Find(x => x.Name == Name);
            foreach (var item in items) {
                collection.Delete(item.Id);
            }
            collection.Upsert(this);
        }

        public void Delete() {
            var collection = PythonAILibManager.Instance.DataFactory.GetAutoGenToolCollection<AutoGenTool>();
            collection.Delete(this.Id);
        }

        public static List<AutoGenTool> FindAll() {
            var collection = PythonAILibManager.Instance.DataFactory.GetAutoGenToolCollection<AutoGenTool>();
            return collection.FindAll().ToList();
        }

    }
}
