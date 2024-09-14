namespace PythonAILib.Model {
    public class CommitInfo {
        // コミットのハッシュ
        public string Hash { get; set; } = "";
        // コミットのメッセージ
        public string Message { get; set; } = "";
        // コミットの日時
        public DateTimeOffset Date { get; set; } = DateTime.Now;
    }
}
