using System.Data.SQLite;
using System.Text.Json;
using System.Text.Json.Serialization;
using PythonAILib.Common;

namespace LibPythonAI.Model.AutoGen {
    public class AutoGenGroupChat {

        private static readonly JsonSerializerOptions jsonSerializerOptions = new() {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
            WriteIndented = true
        };

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("llm_config_name")]
        public string LLMConfigName { get; set; } = "";


        [JsonPropertyName("agent_names")]
        public List<string> AgentNames { get; set; } = [];

        // CreateEntriesDictList
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "name", Name },
                { "description", Description },
                { "llm_config_name", LLMConfigName },
                { "agent_names", AgentNames },
            };
            return dict;
        }

        // Save
        public void Save(bool allow_override = true) {
            UpdateAutoGenGroupChat(Name, Description, LLMConfigName, AgentNames, allow_override);
        }

        // DeleteAsync
        public void Delete() {
            DeleteAutoGenGroupChat(Name);
        }

        // Equals
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            AutoGenGroupChat other = (AutoGenGroupChat)obj;
            return Name == other.Name;
        }

        public override int GetHashCode() {
            return Name.GetHashCode();
        }

        public static void UpdateAutoGenGroupChat(string name, string description, string llm_config_name, List<string> agent_names, bool overwrite) {
            IPythonAILibConfigParams ConfigPrams = PythonAILibManager.Instance.ConfigParams;
            // SQLITE3 DBに接続
            string autogenDBURL = ConfigPrams.GetMainDBPath();
            var sqlConnStr = new SQLiteConnectionStringBuilder(
                $"Data Source={autogenDBURL};Version=3;"
                );
            using var sqlConn = new SQLiteConnection(sqlConnStr.ToString());
            // DBに接続
            sqlConn.Open();
            // DBにテーブルを作成

            // name:  名前
            // description: 説明
            // llm_config_name: LLMの設定名
            // agent_names: エージェント名のリスト
            using var insertCmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS autogen_group_chats (name TEXT, description TEXT, llm_config_name TEXT, agent_names TEXT)", sqlConn);
            insertCmd.ExecuteNonQuery();
            // チャット定義の情報をDBに登録
            using var checkCmd = new SQLiteCommand("SELECT * FROM autogen_group_chats WHERE name = @name", sqlConn);
            checkCmd.Parameters.AddWithValue("@name", name);
            using var reader = checkCmd.ExecuteReader();
            if (reader.HasRows == false) {
                using var insertCmd2 = new SQLiteCommand("INSERT INTO autogen_group_chats (name, description, llm_config_name, agent_names) VALUES (@name, @description, @llm_config_name, @agent_names)", sqlConn);
                insertCmd2.Parameters.AddWithValue("@name", name);
                insertCmd2.Parameters.AddWithValue("@description", description);
                insertCmd2.Parameters.AddWithValue("@llm_config_name", llm_config_name);
                insertCmd2.Parameters.AddWithValue("@agent_names", string.Join(",", agent_names));
                insertCmd2.ExecuteNonQuery();
            } else if (overwrite) {
                using var insertCmd2 = new SQLiteCommand("UPDATE autogen_group_chats SET description = @description, llm_config_name = @llm_config_name, agent_names = @agent_names WHERE name = @name", sqlConn);
                insertCmd2.Parameters.AddWithValue("@name", name);
                insertCmd2.Parameters.AddWithValue("@description", description);
                insertCmd2.Parameters.AddWithValue("@llm_config_name", llm_config_name);
                insertCmd2.Parameters.AddWithValue("@agent_names", string.Join(",", agent_names));
                insertCmd2.ExecuteNonQuery();
            }

            // close
            sqlConn.Close();

        }
        // DeleteAutoGenGroupChat
        public static void DeleteAutoGenGroupChat(string name) {
            IPythonAILibConfigParams ConfigPrams = PythonAILibManager.Instance.ConfigParams;
            // SQLITE3 DBに接続
            string autogenDBURL = ConfigPrams.GetMainDBPath();
            var sqlConnStr = new SQLiteConnectionStringBuilder(
                $"Data Source={autogenDBURL};Version=3;"
                );
            using var sqlConn = new SQLiteConnection(sqlConnStr.ToString());
            // DBに接続
            sqlConn.Open();
            // チャット定義の情報をDBから削除
            using var checkCmd = new SQLiteCommand("SELECT * FROM autogen_group_chats WHERE name = @name", sqlConn);
            checkCmd.Parameters.AddWithValue("@name", name);
            using var reader = checkCmd.ExecuteReader();
            if (reader.HasRows == true) {
                using var insertCmd = new SQLiteCommand("DELETE FROM autogen_group_chats WHERE name = @name", sqlConn);
                insertCmd.Parameters.AddWithValue("@name", name);
                insertCmd.ExecuteNonQuery();
            }
            // close
            sqlConn.Close();
        }

        // GetAutoGenChatList
        public static List<AutoGenGroupChat> GetAutoGenChatList() {
            IPythonAILibConfigParams ConfigPrams = PythonAILibManager.Instance.ConfigParams;
            // SQLITE3 DBに接続
            string autogenDBURL = ConfigPrams.GetMainDBPath();
            var sqlConnStr = new SQLiteConnectionStringBuilder(
                $"Data Source={autogenDBURL};Version=3;"
                );
            using var sqlConn = new SQLiteConnection(sqlConnStr.ToString());
            // DBに接続
            sqlConn.Open();
            // Agentの情報をDBから取得
            using var checkCmd = new SQLiteCommand("SELECT * FROM autogen_group_chats", sqlConn);
            using var reader = checkCmd.ExecuteReader();
            List<AutoGenGroupChat> chats = [];
            while (reader.Read()) {
                AutoGenGroupChat chat = new() {
                    Name = reader.GetString(0),
                    Description = reader.GetString(1),
                    LLMConfigName = reader.GetString(2),
                    AgentNames = reader.GetString(3).Split(",").ToList(),
                };
                chats.Add(chat);
            }
            // close
            sqlConn.Close();
            return chats;
        }

        // ToJson
        public string ToJson() {
            // Serialize the object to JSON
            string jsonString = JsonSerializer.Serialize(this, jsonSerializerOptions);
            return jsonString;
        }

        // FromDict
        public static AutoGenGroupChat FromDict(Dictionary<string, object> dict) {
            AutoGenGroupChat chat = new() {
                Name = dict["name"].ToString() ?? "",
                Description = dict["description"].ToString() ?? "",
                LLMConfigName = dict["llm_config_name"].ToString() ?? "",
                AgentNames = ((List<string?>)dict["agent_names"]).Select(x => x.ToString()).ToList() ?? [],
            };
            return chat;
        }
    }


}
