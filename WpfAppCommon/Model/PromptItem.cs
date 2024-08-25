
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LiteDB;
using WpfAppCommon;

namespace WpfAppCommon.Model {
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

    }
}
