namespace LibMain.Model.File {
    public class CommitInfo {
        // コミットのハッシュ
        public string Hash { get; set; } = "";
        // コミットのメッセージ
        public string Message { get; set; } = "";
        // コミットの日時
        public DateTimeOffset Date { get; set; } = DateTime.Now;

        public string GetDisplayString() {
            string dateString = Date.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
            string result = $"{dateString} {Hash} {Message}";
            return result;
        }
    }
}
