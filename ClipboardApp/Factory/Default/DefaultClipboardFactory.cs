namespace ClipboardApp.Factory.Default {
    public class DefaultClipboardFactory : IClipboardFactory {
        private IClipboardController? _clipboardController;

        public IClipboardController GetClipboardController() {
            if (_clipboardController == null) {
                _clipboardController = new DefaultClipboardController();
            }
            return _clipboardController;
        }
        private IClipboardDBController? _clipboardDBController;

        public IClipboardDBController GetClipboardDBController() {
            if (_clipboardDBController == null) {
                _clipboardDBController = new DefaultClipboardDBController();
            }
            return _clipboardDBController;
        }
        private IBackupController? _backupController;
        public IBackupController GetBackupController() {
            if (_backupController == null) {
                _backupController = new DefaultBackupController();
            }
            return _backupController;
        }
        private IClipboardProcessController? _clipboardProcessController;
        public IClipboardProcessController GetClipboardProcessController() { 
                   if (_clipboardProcessController == null) {
                _clipboardProcessController = new DefaultClipboardProcessController();
            }
            return _clipboardProcessController;
        }
    }
}
