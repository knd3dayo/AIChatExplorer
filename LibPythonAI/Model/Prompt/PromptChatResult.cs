using System.Data;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibPythonAI.Utils.Common;

namespace LibPythonAI.Model.Prompt {
    public class PromptChatResult() {

        public Dictionary<string, object> Results { get; set; } = [];

        public string GetTextContent(string promptName) {
            return Results.TryGetValue(promptName, out object? value) ? (string)value : "";
        }
        public void SetTextContent(string promptName, string content) {
            Results[promptName] = content;
        }

        public List<string> GetListContent(string promptName) {
            return Results.TryGetValue(promptName, out object? value) ? (List<string>)value : [];
        }
        public void SetListContent(string promptName, List<string> content) {
            Results[promptName] = content;
        }

        public DataTable GetTableContent(string promptName) {
            Results.TryGetValue(promptName, out object? values);
            if (values == null) {
                return DictionaryListToDataTable([]);
            }

            if (values is List<object> objectList) {
                List<string> stringList = [];
                foreach (var item in objectList) {
                    if (item.ToString() is string strValue) {
                        stringList.Add(strValue);
                    }
                }
                return ListToDataTable(stringList);
            }

            if (values is List<Dictionary<string, object>> list) {
                return DictionaryListToDataTable(list);
            }

            if (values is object[] list2) {
                var items =  list2.Select(x => (Dictionary<string, object>)x).ToList();
                return DictionaryListToDataTable(items);

            }

            return DictionaryListToDataTable([]);
        }
        public void SetTableContent(string promptName, DataTable dataTable) {
            Results[promptName] = DataTableToList(dataTable);
        }
        public void SetTableContent(string promptName, List<Dictionary<string, object>> dataTable) {
            Results[promptName] = dataTable;
        }

        // stringのListをDataTableに変換
        private static DataTable ListToDataTable(List<string> list) {
            DataTable dataTable = new();
            dataTable.Columns.Add("Value", typeof(string));
            foreach (var item in list) {
                DataRow dataRow = dataTable.NewRow();
                dataRow["Value"] = item;
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }

        // List<Dictionary<string, object>>からDataTableに変換
        private static DataTable DictionaryListToDataTable(List<Dictionary<string, object>> tableContent) {
            DataTable dataTable = new();
            if (tableContent.Count == 0) {
                return dataTable;
            }
            // tableContentの1番目の要素からキーを取得して、DataTableのカラムを作成
            var firstRow = tableContent[0];
            foreach (var key in firstRow.Keys) {
                dataTable.Columns.Add(key, typeof(string));
            }
            // tableContentの各要素をDataTableに追加
            foreach (var row in tableContent) {
                DataRow dataRow = dataTable.NewRow();
                foreach (var key in row.Keys) {
                    dataRow[key] = row[key];
                }
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }


        // DataTableからList<Dictionary<string, object>>に変換
        private List<Dictionary<string, object>> DataTableToList(DataTable dataTable) {
            List<Dictionary<string, object>> tableContent = [];
            foreach (DataRow row in dataTable.Rows) {
                Dictionary<string, object> newRow = new();
                foreach (DataColumn column in dataTable.Columns) {
                    ((IDictionary<string, object>)newRow)[column.ColumnName] = row[column];
                }
                tableContent.Add(newRow);
            }
            return tableContent;
        }


        public string ToJson() {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            return JsonSerializer.Serialize(this, options);
        }

        public static PromptChatResult? FromDict(Dictionary<string, dynamic?> dict) {
            PromptChatResult? result = new();
            // key = Results があるかどうか
            if (!dict.TryGetValue("Results", out object? value)) {
                return result;
            }
            // Resultsの中身を取り出す
            if (value is Dictionary<string, object> results) {
                result.Results = results;
            }
            return result;
        }

        public static PromptChatResult? FromJson(string json) {
            Dictionary<string, dynamic?> dict = JsonUtil.ParseJson(json);
            return FromDict(dict);
        }
    }
}
