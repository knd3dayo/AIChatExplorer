namespace ClipboardApp.Utils {
    public class ClipboardAppException : Exception {
        public ClipboardAppException(string message) : base(message) {
        }
        public ClipboardAppException(string message, Exception innerException) : base(message, innerException) {
        }

    }
}
