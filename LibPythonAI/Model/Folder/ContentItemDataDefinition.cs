
using LibPythonAI.Resources;

namespace LibPythonAI.Model.Folder {
    public class ContentItemDataDefinition {

        public static readonly string PropertiesName = "Properties";
        public static readonly string TextName = "Text";
        public static readonly string ImageName = "Image";
        public static readonly string SourcePathName = "SourcePath";
        public static readonly string TitleName = "Title";

        public string Name { get; set; }
        public string DisplayText { get; set; }
        public bool IsChecked { get; set; }

        // IsPromptItem
        public bool IsPromptItem { get; set; }

        public ContentItemDataDefinition(string name, string displayText, bool isChecked, bool isPromptItem) {
            Name = name;
            DisplayText = displayText;
            IsChecked = isChecked;
            IsPromptItem = isPromptItem;

        }

        public static List<ContentItemDataDefinition> CreateDefaultDataDefinitions() {
            return [
                new ContentItemDataDefinition(TextName, PythonAILibStringResources.Instance.Text, true, false),
                new ContentItemDataDefinition(TitleName, PythonAILibStringResources.Instance.Title, false, false),
                new ContentItemDataDefinition(PropertiesName, PythonAILibStringResources.Instance.Properties, false, false),
                new ContentItemDataDefinition(SourcePathName, PythonAILibStringResources.Instance.SourcePath, false, false),
                new ContentItemDataDefinition(ImageName, PythonAILibStringResources.Instance.Image, false, false),
            ];
        }

    }
}
