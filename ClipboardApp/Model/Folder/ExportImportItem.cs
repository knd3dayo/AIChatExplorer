namespace ClipboardApp.Model.Folder {
    public class ExportImportItem {

        public string Name { get; set; }
        public string DisplayText { get; set; }
        public bool IsChecked { get; set; }

        // IsPromptItem
        public bool IsPromptItem { get; set; }

        public ExportImportItem(string name, string displayText, bool isChecked, bool isPromptItem) {
            Name = name;
            DisplayText = displayText;
            IsChecked = isChecked;
            IsPromptItem = isPromptItem;

        }

    }
}
