using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PythonAILib.Common;
using PythonAILib.PythonIF;

namespace PythonAILib.Model.AutoGen {
    public class AutoGenLLMConfig {

        public static readonly string API_TYPE_AZURE = "azure";
        public static readonly string API_TYPE_OPENAI = "openai";

        // name コンフィグ名
        public string Name { get; set; } = "";
        // api_type api_type (azure, openaiなど)
        public string ApiType { get; set; } = "";
        // api_version apiのバージョン
        public string ApiVersion { get; set; } = "";
        // model llmのモデル
        public string Model { get; set; } = "";
        // api_key apiキー
        public string ApiKey { get; set; } = "";
        // base_url base_url
        public string BaseURL { get; set; } = "";

        public void Save() {
            UpdateAutoGenLLMConfig(this.Name, this.ApiType, this.ApiVersion, this.Model, this.ApiKey, this.BaseURL);
        }

        public void Delete() {
            DeleteAutoGenLLMConfig(this.Name);
        }

        // LLMConfig設定を更新する.
        public static void UpdateAutoGenLLMConfig(string name, string api_type, string api_version, string model, string api_key, string base_url) {
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
            // name コンフィグ名
            // api_type api_type (azure, openaiなど)
            // api_version apiのバージョン
            // model llmのモデル
            // api_key apiキー
            // base_url base_url
            using var insertCmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS llm_configs (name TEXT, api_type TEXT, api_version TEXT, model TEXT, api_key TEXT, base_url TEXT)", sqlConn);
            insertCmd.ExecuteNonQuery();
            // llm_configの情報をDBに登録
            using var checkCmd = new SQLiteCommand("SELECT * FROM llm_configs WHERE name = @name", sqlConn);
            checkCmd.Parameters.AddWithValue("@name", name);
            using var reader = checkCmd.ExecuteReader();
            if (reader.HasRows == false) {
                using var insertCmd2 = new SQLiteCommand("INSERT INTO llm_configs (name, api_type, api_version, model, api_key, base_url) VALUES (@name, @api_type, @api_version, @model, @api_key, @base_url)", sqlConn);
                insertCmd2.Parameters.AddWithValue("@name", name);
                insertCmd2.Parameters.AddWithValue("@api_type", api_type);
                insertCmd2.Parameters.AddWithValue("@api_version", api_version);
                insertCmd2.Parameters.AddWithValue("@model", model);
                insertCmd2.Parameters.AddWithValue("@api_key", api_key);
                insertCmd2.Parameters.AddWithValue("@base_url", base_url);
                insertCmd2.ExecuteNonQuery();
            } else {
                using var insertCmd2 = new SQLiteCommand("UPDATE llm_configs SET api_type = @api_type, api_version = @api_version, model = @model, api_key = @api_key, base_url = @base_url WHERE name = @name", sqlConn);
                insertCmd2.Parameters.AddWithValue("@name", name);
                insertCmd2.Parameters.AddWithValue("@api_type", api_type);
                insertCmd2.Parameters.AddWithValue("@api_version", api_version);
                insertCmd2.Parameters.AddWithValue("@model", model);
                insertCmd2.Parameters.AddWithValue("@api_key", api_key);
                insertCmd2.Parameters.AddWithValue("@base_url", base_url);
                insertCmd2.ExecuteNonQuery();
            }
            // close
            sqlConn.Close();
        }

        // LLMConfig設定を削除する.
        public static void DeleteAutoGenLLMConfig(string name) {
            IPythonAILibConfigParams ConfigPrams = PythonAILibManager.Instance.ConfigParams;
            // SQLITE3 DBに接続
            string autogenDBURL = ConfigPrams.GetAutoGenDBPath();
            var sqlConnStr = new SQLiteConnectionStringBuilder(
                $"Data Source={autogenDBURL};Version=3;"
                );
            using var sqlConn = new SQLiteConnection(sqlConnStr.ToString());
            // DBに接続
            sqlConn.Open();
            // llm_configの情報をDBから削除
            using var checkCmd = new SQLiteCommand("SELECT * FROM llm_configs WHERE name = @name", sqlConn);
            checkCmd.Parameters.AddWithValue("@name", name);
            using var reader = checkCmd.ExecuteReader();
            if (reader.HasRows == true) {
                using var insertCmd = new SQLiteCommand("DELETE FROM llm_configs WHERE name = @name", sqlConn);
                insertCmd.Parameters.AddWithValue("@name", name);
                insertCmd.ExecuteNonQuery();
            }
            // close
            sqlConn.Close();
        }

        public static List<AutoGenLLMConfig> GetAutoGenLLMConfigList() {
            IPythonAILibConfigParams ConfigPrams = PythonAILibManager.Instance.ConfigParams;
            // SQLITE3 DBに接続
            string autogenDBURL = ConfigPrams.GetAutoGenDBPath();
            var sqlConnStr = new SQLiteConnectionStringBuilder(
                $"Data Source={autogenDBURL};Version=3;"
                );
            using var sqlConn = new SQLiteConnection(sqlConnStr.ToString());
            // DBに接続
            sqlConn.Open();
            // llm_configの情報をDBから取得
            using var checkCmd = new SQLiteCommand("SELECT * FROM llm_configs", sqlConn);
            using var reader = checkCmd.ExecuteReader();
            List<AutoGenLLMConfig> llmConfigs = [];
            while (reader.Read()) {
                AutoGenLLMConfig llmConfig = new() {
                    Name = reader.GetString(0),
                    ApiType = reader.GetString(1),
                    ApiVersion = reader.GetString(2),
                    Model = reader.GetString(3),
                    ApiKey = reader.GetString(4),
                    BaseURL = reader.GetString(5),
                };
                llmConfigs.Add(llmConfig);
            }
            // close
            sqlConn.Close();
            return llmConfigs;
        }


    }
}
