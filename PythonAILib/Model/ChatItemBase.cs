
namespace PythonAILib.Model {
    public abstract class ChatItemBase {

        // 生成日時
        public DateTime CreatedAt { get; set; }

        // 更新日時
        public DateTime UpdatedAt { get; set; }

        // クリップボードの内容
        public string Content { get; set; } = "";


        // 背景情報
        public string BackgroundInfo { get; set; } = "";

        // サマリー
        public string Summary { get; set; } = "";


    }
}
