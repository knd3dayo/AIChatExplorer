namespace ClipboardApp.Model {

    // アクションの情報を表すクラス
    public class ActionMessage {

        public enum MessageTypes {
            // 情報
            Info,
            // エラー
            Error
        }
        public string Message { get; set; }
        public MessageTypes MessageType { get; set; }


        private ActionMessage(MessageTypes messageType, string message) {
            MessageType = messageType;
            Message = message;
        }
        public static ActionMessage Info(string message) {
            return new(MessageTypes.Info, message);
        }
        public static ActionMessage Error(string message) {
            return new(MessageTypes.Error, message);
        }


    }
}
