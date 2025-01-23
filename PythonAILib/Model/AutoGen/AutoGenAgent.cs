using System.Data.SQLite;
using System.Text.Json.Serialization;
using PythonAILib.Common;
using PythonAILib.Model.VectorDB;

namespace PythonAILib.Model.AutoGen {
    public class AutoGenAgent {

        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.NewObjectId();

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        // system_message
        [JsonPropertyName("system_message")]
        public string SystemMessage { get; set; } = "";

        // type
        [JsonPropertyName("type_value")]
        public string TypeValue { get; set; } = "";

        [JsonPropertyName("tool_names")]
        public List<string> ToolNames { get; set; } = new List<string>();


        // human_input_mode
        [JsonPropertyName("human_input_mode")]
        public string HumanInputMode { get; set; } = "";

        // termination_msg
        [JsonPropertyName("termination_msg")]
        public string TerminationMsg { get; set; } = "";

        // code_execution 
        [JsonPropertyName("code_execution")]
        public bool CodeExecution { get; set; } = false;

        // llm
        [JsonPropertyName("llm_config_name")]
        public string LLMConfigName { get; set; } = "";

        // List(VectorDBItem)
        [JsonPropertyName("vector_db_items")]
        public List<VectorDBItem> VectorDBItems { get; set; } = [];


        // ToDictList
        public static Dictionary<string, object> ToDict(AutoGenAgent data) {
            // Create a dictionary
            Dictionary<string, object> dict = new Dictionary<string, object> {
                { "name", data.Name },
                { "description", data.Description },
                { "system_message", data.SystemMessage },
                { "type_value", data.TypeValue },
                { "tool_names", data.ToolNames },
                { "human_input_mode", data.HumanInputMode },
                { "termination_msg", data.TerminationMsg },
                { "code_execution", data.CodeExecution },
                { "llm_config_name", data.LLMConfigName },
                { "vector_db_items", VectorDBItem.ToDictList(data.VectorDBItems) }

            };
            return dict;
        }
        // ToDictList
        public static List<Dictionary<string, object>> ToDictList(List<AutoGenAgent> data) {
            // Create a list of dictionaries
            List<Dictionary<string, object>> dictList = [];
            foreach (AutoGenAgent item in data) {
                dictList.Add(ToDict(item));
            }
            return dictList;
        }

        // Save
        public void Save(bool allow_override = true) {
            UpdateAutoGenAgent(Name, Description, SystemMessage, HumanInputMode, TerminationMsg, CodeExecution, LLMConfigName, ToolNames, allow_override);
        }
        // Delete
        public void Delete() {
            DeleteAutoGenAgent(this.Name);
        }

        // Update AutoGenAgent
        public static void UpdateAutoGenAgent(string name, string description, string system_message, string human_input_mode, string terminateMsg, bool code_execution, string llm_config_name, List<string> tool_names, bool overwrite) {
            IPythonAILibConfigParams ConfigPrams = PythonAILibManager.Instance.ConfigParams;
            // SQLITE3 DBに接続
            string autogenDBURL = ConfigPrams.GetAutoGenDBPath();

            var sqlConnStr = new SQLiteConnectionStringBuilder(
                $"Data Source={autogenDBURL};Version=3;"
                );
            using var sqlConn = new SQLiteConnection(sqlConnStr.ToString());
            // DBに接続
            sqlConn.Open();
            // DBにテーブルを作成

            // agentsテーブル： ツールの情報を格納
            // name: Agent名
            // description: Agentの説明
            // system_message: システムメッセージ
            // human_input_mode
            // termination_msg
            // code_execution
            // llm_config_name LLMConfig名
            //tooo_names ツール名
            // テーブルが存在しない場合のみ作成
            using var createCmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS agents (name TEXT PRIMARY KEY, description TEXT, system_message TEXT, human_input_mode TEXT, termination_msg TEXT, code_execution BOOLEAN, llm_config_name TEXT, tool_names TEXT)", sqlConn);
            createCmd.ExecuteNonQuery();
            // Agentの情報をDBに登録
            using var checkCmd = new SQLiteCommand("SELECT * FROM agents WHERE name = @name", sqlConn);
            checkCmd.Parameters.AddWithValue("@name", name);
            using var reader = checkCmd.ExecuteReader();
            if (reader.HasRows == false) {
                using var insertCmd2 = new SQLiteCommand("INSERT INTO agents (name, description, system_message, human_input_mode, termination_msg, code_execution, llm_config_name, tool_names) VALUES (@name, @description, @system_message, @human_input_mode, @termination_msg, @code_execution, @llm_config_name, @tool_names)", sqlConn);
                insertCmd2.Parameters.AddWithValue("@name", name);
                insertCmd2.Parameters.AddWithValue("@description", description);
                insertCmd2.Parameters.AddWithValue("@system_message", system_message);
                insertCmd2.Parameters.AddWithValue("@human_input_mode", human_input_mode);
                insertCmd2.Parameters.AddWithValue("@termination_msg", terminateMsg);
                insertCmd2.Parameters.AddWithValue("@code_execution", code_execution);
                insertCmd2.Parameters.AddWithValue("@llm_config_name", llm_config_name);
                insertCmd2.Parameters.AddWithValue("@tool_names", tool_names);
                insertCmd2.ExecuteNonQuery();
            } else if (overwrite){
                using var insertCmd2 = new SQLiteCommand("UPDATE agents SET description = @description, system_message = @system_message, human_input_mode = @human_input_mode, termination_msg = @termination_msg, code_execution = @code_execution, llm_config_name = @llm_config_name, tool_names = @tool_names WHERE name = @name", sqlConn);
                insertCmd2.Parameters.AddWithValue("@name", name);
                insertCmd2.Parameters.AddWithValue("@description", description);
                insertCmd2.Parameters.AddWithValue("@system_message", system_message);
                insertCmd2.Parameters.AddWithValue("@human_input_mode", human_input_mode);
                insertCmd2.Parameters.AddWithValue("@termination_msg",  terminateMsg);
                insertCmd2.Parameters.AddWithValue("@code_execution", code_execution);
                insertCmd2.Parameters.AddWithValue("@llm_config_name", llm_config_name);
                insertCmd2.Parameters.AddWithValue("@tool_names", tool_names);
                insertCmd2.ExecuteNonQuery();
            }
            // close
            sqlConn.Close();
        }

        // DeleteAutoGenAgent
        public static void DeleteAutoGenAgent(string name) {
            IPythonAILibConfigParams ConfigPrams = PythonAILibManager.Instance.ConfigParams;
            // SQLITE3 DBに接続
            string autogenDBURL = ConfigPrams.GetAutoGenDBPath();
            var sqlConnStr = new SQLiteConnectionStringBuilder(
                $"Data Source={autogenDBURL};Version=3;"
                );
            using var sqlConn = new SQLiteConnection(sqlConnStr.ToString());
            // DBに接続
            sqlConn.Open();
            // Agentの情報をDBから削除
            using var checkCmd = new SQLiteCommand("SELECT * FROM agents WHERE name = @name", sqlConn);
            checkCmd.Parameters.AddWithValue("@name", name);
            using var reader = checkCmd.ExecuteReader();
            if (reader.HasRows) {
                using var deleteCmd = new SQLiteCommand("DELETE FROM agents WHERE name = @name", sqlConn);
                deleteCmd.Parameters.AddWithValue("@name", name);
                deleteCmd.ExecuteNonQuery();
            }
            // close
            sqlConn.Close();
        }

        // GetAutoGenAgentList
        public static List<AutoGenAgent> GetAutoGenAgentList() {
            IPythonAILibConfigParams ConfigPrams = PythonAILibManager.Instance.ConfigParams;
            // SQLITE3 DBに接続
            string autogenDBURL = ConfigPrams.GetAutoGenDBPath();
            var sqlConnStr = new SQLiteConnectionStringBuilder(
                $"Data Source={autogenDBURL};Version=3;"
                );
            using var sqlConn = new SQLiteConnection(sqlConnStr.ToString());
            // DBに接続
            sqlConn.Open();
            // Agentの情報をDBから取得
            using var checkCmd = new SQLiteCommand("SELECT * FROM agents", sqlConn);
            using var reader = checkCmd.ExecuteReader();
            List<AutoGenAgent> agents = [];
            while (reader.Read()) {
                AutoGenAgent agent = new() {
                    Name = reader.GetString(0),
                    Description = reader.GetString(1),
                    SystemMessage = reader.GetString(2),
                    HumanInputMode = reader.GetString(3),
                    TerminationMsg = reader.GetString(4),
                    CodeExecution = reader.GetBoolean(5),
                    LLMConfigName = reader.GetString(6),
                    ToolNames = reader.GetString(7).Split(",").ToList(),
                };
                agents.Add(agent);
            }
            // close
            sqlConn.Close();
            return agents;
        }


    }
}
