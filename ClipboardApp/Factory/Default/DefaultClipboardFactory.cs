namespace ClipboardApp.Factory.Default {
    public class DefaultClipboardFactory : IClipboardFactory {
        
        private IClipboardDBController? _clipboardDBController;

        public IClipboardDBController GetClipboardDBController() {
            if (_clipboardDBController == null) {
                _clipboardDBController = new DefaultClipboardDBController();
            }
            return _clipboardDBController;
        }
    }
}
