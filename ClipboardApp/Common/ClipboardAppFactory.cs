

namespace ClipboardApp.Factory {
    public class ClipboardAppFactory  {

        private static ClipboardAppFactory? _instance;
        // Factory
        public static ClipboardAppFactory Instance {
            get {
                if (_instance == null) {
                    _instance = new ClipboardAppFactory();
                }
                return _instance;
            }
        }
        // GetClipboardDBController
        private ClipboardDBController? _clipboardDBController;
        public ClipboardDBController GetClipboardDBController() {
            if (_clipboardDBController == null) {
                _clipboardDBController = new ClipboardDBController();
            }
            return _clipboardDBController;
        }
    }
}
