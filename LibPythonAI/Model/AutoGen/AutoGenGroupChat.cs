using System.Data.SQLite;
using System.Text.Json.Serialization;
using PythonAILib.Common;

namespace PythonAILib.Model.AutoGen {
    public class AutoGenGroupChat {

        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.NewObjectId();

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("llm_config_name")]
        public string LLMConfigName { get; set; } = "";


        [JsonPropertyName("agent_names")]
        public List<string> AgentNames { get; set; } = [];

        // CreateRequestDict
        public Dictionary<string, object> CreateRequestDict() {
            Dictionary<string, object> dict = new() {
                { "name", Name },
            };
            return dict;
        }

        // ToDictList
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
            UpdateAutoGenGroupChat(this.Name, this.Description, this.LLMConfigName, this.AgentNames, allow_override);
        }

        // Delete
        public void Delete() {
            DeleteAutoGenGroupChat(this.Name);
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

        public static void UpdateAutoGenGroupChat(string name, string description, string llm_config_name, List<string> agent_names  ,bool overwrite) {
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

            // name:  名前
            // description: 説明
            // llm_config_name: LLMの設定名
            // agent_names: エージェント名のリスト
            using var insertCmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS group_chats (name TEXT, description TEXT, llm_config_name TEXT, agent_names TEXT)", sqlConn);
            insertCmd.ExecuteNonQuery();
            // チャット定義の情報をDBに登録
            using var checkCmd = new SQLiteCommand("SELECT * FROM group_chats WHERE name = @name", sqlConn);
            checkCmd.Parameters.AddWithValue("@name", name);
            using var reader = checkCmd.ExecuteReader();
            if (reader.HasRows == false) {
                using var insertCmd2 = new SQLiteCommand("INSERT INTO group_chats (name, description, llm_config_name, agent_names) VALUES (@name, @description, @llm_config_name, @agent_names)", sqlConn);
                insertCmd2.Parameters.AddWithValue("@name", name);
                insertCmd2.Parameters.AddWithValue("@description", description);
                insertCmd2.Parameters.AddWithValue("@llm_config_name", llm_config_name);
                insertCmd2.Parameters.AddWithValue("@agent_names", string.Join(",", agent_names));
                insertCmd2.ExecuteNonQuery();
            } else if (overwrite) {
                using var insertCmd2 = new SQLiteCommand("UPDATE group_chats SET description = @description, llm_config_name = @llm_config_name, agent_names = @agent_names WHERE name = @name", sqlConn);
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
            string autogenDBURL = ConfigPrams.GetAutoGenDBPath();
            var sqlConnStr = new SQLiteConnectionStringBuilder(
                $"Data Source={autogenDBURL};Version=3;"
                );
            using var sqlConn = new SQLiteConnection(sqlConnStr.ToString());
            // DBに接続
            sqlConn.Open();
            // チャット定義の情報をDBから削除
            using var checkCmd = new SQLiteCommand("SELECT * FROM group_chats WHERE name = @name", sqlConn);
            checkCmd.Parameters.AddWithValue("@name", name);
            using var reader = checkCmd.ExecuteReader();
            if (reader.HasRows == true) {
                using var insertCmd = new SQLiteCommand("DELETE FROM group_chats WHERE name = @name", sqlConn);
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
            string autogenDBURL = ConfigPrams.GetAutoGenDBPath();
            var sqlConnStr = new SQLiteConnectionStringBuilder(
                $"Data Source={autogenDBURL};Version=3;"
                );
            using var sqlConn = new SQLiteConnection(sqlConnStr.ToString());
            // DBに接続
            sqlConn.Open();
            // Agentの情報をDBから取得
            using var checkCmd = new SQLiteCommand("SELECT * FROM group_chats", sqlConn);
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

    }


}
