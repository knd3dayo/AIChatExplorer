using System.Text.Json.Serialization;
using PythonAILib.Common;

namespace PythonAILib.Model.AutoGen {
    public class AutoGenTool {
        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.NewObjectId();

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("content")]
        public string Content { get; set; } = "";

        // ToDict
        public static Dictionary<string, object> ToDict(AutoGenTool data) {
            // Create a dictionary
            Dictionary<string, object> dict = new Dictionary<string, object> {
                { "name", data.Name },
                { "description", data.Description },
                { "content", data.Content },
            };
            return dict;
        }
        // ToDict
        public static List<Dictionary<string, object>> ToDict(List<AutoGenTool> data) {
            List<Dictionary<string, object>> dictList = new List<Dictionary<string, object>>();
            foreach (AutoGenTool item in data) {
                dictList.Add(ToDict(item));
            }
            return dictList;
        }

        public void Save() {
            var collection = PythonAILibManager.Instance.DataFactory.GetAutoGenToolCollection<AutoGenTool>();
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

        public static void Init() {
            // LiteDBにシステム定義のデータを保存する
            var collection = PythonAILibManager.Instance.DataFactory.GetAutoGenToolCollection<AutoGenTool>();
            // nameが一致するデータがあるか確認
            // vector_search
            var data = collection.FindOne(x => x.Name == "vector_search");
            if (data == null) {
                // データがない場合は新規作成
                data = new AutoGenTool {
                    Name = "vector_search",
                    Description = "This function performs a vector search on the specified text and returns the related documents.",
                    Content = "",
                };
                collection.Upsert(data);
            }
            // search_wikipedia
            data = collection.FindOne(x => x.Name == "search_wikipedia");
            if (data == null) {
                data = new AutoGenTool {
                    Name = "search_wikipedia",
                    Description = "This function searches Wikipedia with the specified keywords and returns the related articles.",
                    Content = "",
                };
                collection.Upsert(data);
            }
            // list_files
            data = collection.FindOne(x => x.Name == "list_files");
            if (data == null) {
                data = new AutoGenTool {
                    Name = "list_files",
                    Description = "This function returns a list of files in the specified directory.",
                    Content = "",
                };
                collection.Upsert(data);
            }
            // extract_text
            data = collection.FindOne(x => x.Name == "extract_text");
            if (data == null) {
                data = new AutoGenTool {
                    Name = "extract_text",
                    Description = "This function extracts text from the specified file.",
                    Content = "",
                };
                collection.Upsert(data);
            }
            // check file
            data = collection.FindOne(x => x.Name == "check_file");
            if (data == null) {
                data = new AutoGenTool {
                    Name = "check_file",
                    Description = "This function checks if the specified file exists.",
                    Content = "",
                };
                collection.Upsert(data);
            }
            // extract_webpage
            data = collection.FindOne(x => x.Name == "extract_webpage");
            if (data == null) {
                data = new AutoGenTool {
                    Name = "extract_webpage",
                    Description = "This function extracts text and links from the specified URL of a web page.",
                    Content = "",
                };
                collection.Upsert(data);
            }
            // search_duckduckgo
            data = collection.FindOne(x => x.Name == "search_duckduckgo");
            if (data == null) {
                data = new AutoGenTool {
                    Name = "search_duckduckgo",
                    Description = "This function searches DuckDuckGo with the specified keywords and returns related articles.",
                    Content = "",
                };
                collection.Upsert(data);
            }
            // save_text_file
            data = collection.FindOne(x => x.Name == "save_text_file");
            if (data == null) {
                data = new AutoGenTool {
                    Name = "save_text_file",
                    Description = "This function saves text data as a file.",
                    Content = "",
                };
                collection.Upsert(data);
            }
            // save_tools
            data = collection.FindOne(x => x.Name == "save_tools");
            if (data == null) {
                data = new AutoGenTool {
                    Name = "save_tools",
                    Description = "This function saves Python code as a JSON file for AutoGen tools.",
                    Content = "",
                };
                collection.Upsert(data);
            }

            // get_current_time
            data = collection.FindOne(x => x.Name == "get_current_time");
            if (data == null) {
                data = new AutoGenTool {
                    Name = "get_current_time",
                    Description = "This function returns the current time in the format yyyy/mm/dd (Day) hh:mm:ss.",
                    Content = "",
                };
                collection.Upsert(data);
            }
        }
    }
}
