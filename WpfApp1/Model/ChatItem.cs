namespace WpfApp1.Model {
    public class ChatItem : ClipboardItem {


        public static string SystemRole = "system";
        public static string AssistantRole = "assistant";
        public static string UserRole = "user";
        public string Role { get; set; } = SystemRole;
        public new string Content { get; set; } = "";
        public ChatItem() : base() {
        }
        public ChatItem(string role, string text) : this() {
            Role = role;
            Content = text;
        }

    }
}
