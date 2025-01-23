using System.Data.SQLite;
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

        public void Save(bool allow_override = true) {
            UpdateAutoGenTool(this.Name, this.Description, this.SourcePath, allow_override);
        }

        public void Delete() {
            DeleteAutoGenTool(this.Name);
        }

        public static void DeleteAll() {
            List<AutoGenTool> autoGenTools = GetAutoGenToolList();
            foreach (AutoGenTool tool in autoGenTools) {
                tool.Delete();
            }
        }

        public static void UpdateAutoGenTool(string toolPath, string toolName, string toolDescription, bool overwrite) {
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
            // テーブルが存在しない場合のみ作成
            // toolsテーブル： ツールの情報を格納
            // name: ツール名
            // path: ツールのパス
            // description: ツールの説明

            using var cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS tools (name TEXT, path TEXT, description TEXT)", sqlConn);
            cmd.ExecuteNonQuery();
            // ツールの情報をDBに登録
            using var checkCmd = new SQLiteCommand("SELECT * FROM tools WHERE name = @name", sqlConn);
            checkCmd.Parameters.AddWithValue("@name", toolName);
            using var reader = checkCmd.ExecuteReader();
            if (reader.HasRows == false) {
                using var insertCmd = new SQLiteCommand("INSERT INTO tools (name, path, description) VALUES (@name, @path, @description)", sqlConn);
                insertCmd.Parameters.AddWithValue("@name", toolName);
                insertCmd.Parameters.AddWithValue("@path", toolPath);
                insertCmd.Parameters.AddWithValue("@description", toolDescription);
                insertCmd.ExecuteNonQuery();

            } else if (overwrite) {
                using var insertCmd = new SQLiteCommand("UPDATE tools SET path = @path, description = @description WHERE name = @name", sqlConn);
                insertCmd.Parameters.AddWithValue("@name", toolName);
                insertCmd.Parameters.AddWithValue("@path", toolPath);
                insertCmd.Parameters.AddWithValue("@description", toolDescription);
                insertCmd.ExecuteNonQuery();
            }


            // close
            sqlConn.Close();

        }
        // DeleteAutoGenTool
        public static void DeleteAutoGenTool(string toolName) {
            IPythonAILibConfigParams ConfigPrams = PythonAILibManager.Instance.ConfigParams;
            // SQLITE3 DBに接続
            string autogenDBURL = ConfigPrams.GetAutoGenDBPath();
            var sqlConnStr = new SQLiteConnectionStringBuilder(
                $"Data Source={autogenDBURL};Version=3;"
                );
            using var sqlConn = new SQLiteConnection(sqlConnStr.ToString());
            // DBに接続
            sqlConn.Open();
            // ツールの情報をDBから削除
            using var checkCmd = new SQLiteCommand("SELECT * FROM tools WHERE name = @name", sqlConn);
            checkCmd.Parameters.AddWithValue("@name", toolName);
            using var reader = checkCmd.ExecuteReader();
            if (reader.HasRows) {
                using var deleteCmd = new SQLiteCommand("DELETE FROM tools WHERE name = @name", sqlConn);
                deleteCmd.Parameters.AddWithValue("@name", toolName);
                deleteCmd.ExecuteNonQuery();
            }
            // close
            sqlConn.Close();
        }

        // GetAutoGenToolList
        public static List<AutoGenTool> GetAutoGenToolList() {
            IPythonAILibConfigParams ConfigPrams = PythonAILibManager.Instance.ConfigParams;
            // SQLITE3 DBに接続
            string autogenDBURL = ConfigPrams.GetAutoGenDBPath();
            var sqlConnStr = new SQLiteConnectionStringBuilder(
                $"Data Source={autogenDBURL};Version=3;"
                );
            using var sqlConn = new SQLiteConnection(sqlConnStr.ToString());
            // DBに接続
            sqlConn.Open();
            // ツールの情報をDBから取得
            using var checkCmd = new SQLiteCommand("SELECT * FROM tools", sqlConn);
            using var reader = checkCmd.ExecuteReader();
            List<AutoGenTool> tools = [];
            while (reader.Read()) {
                AutoGenTool tool = new() {
                    Name = reader.GetString(0),
                    SourcePath = reader.GetString(1),
                    Description = reader.GetString(2),
                };
                tools.Add(tool);
            }
            // close
            sqlConn.Close();
            return tools;
        }

    }
}
