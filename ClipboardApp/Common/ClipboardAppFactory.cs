

namespace ClipboardApp.Factory {
    public class ClipboardAppFactory : IClipboardFactory {

        private static IClipboardFactory? _instance;
        // Factory
        public static IClipboardFactory Instance {
            get {
                if (_instance == null) {
                    _instance = new ClipboardAppFactory();
                }
                return _instance;
            }
        }
        // GetClipboardDBController
        private IClipboardDBController? _clipboardDBController;
        public IClipboardDBController GetClipboardDBController() {
            if (_clipboardDBController == null) {
                _clipboardDBController = new ClipboardDBController();
            }
            return _clipboardDBController;
        }
    }
}
