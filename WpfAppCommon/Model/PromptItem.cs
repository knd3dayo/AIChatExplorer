
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LiteDB;
using WpfAppCommon;

namespace QAChat.Model {
    public class PromptItem {

        public ObjectId Id { get; set; } = ObjectId.Empty;
        // 名前
        public string Name { get; set; } = "";
        // 説明
        public string Description { get; set; } = "";

        // プロンプト
        public string Prompt { get; set; } = "";

        // PromptItemを取得
        public static PromptItem GetPromptItemById(ObjectId id) {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetPromptTemplate(id);

        }

        // ImageURLのリスト
        public List<string> ImageURLs { get; set; } = new List<string>();

        public string ToJson() {

            //Encode設定
            var op = new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };

            //プロンプト用のデータを作成
            List<Dictionary<string, string>> parameters = [];
            // Promptプロパティを追加
            var dc = new Dictionary<string, string> {
                ["type"] = "text",
                ["text"] = Prompt
            };
            parameters.Add(dc);

            foreach (var url in ImageURLs) {
                // ImageURLプロパティを追加
                dc = new Dictionary<string, string> {
                    ["type"] = "image_url",
                    ["image_url"] = url
                };
                parameters.Add(dc);
            }

            //jsonに変換する
            string json = System.Text.Json.JsonSerializer.Serialize(parameters, op);
            return json;
        }

    }
}
