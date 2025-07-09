using LibPythonAI.Resources;

namespace LibPythonAI.Model.Content {
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

        public Dictionary<string, dynamic> ToDict() {
            return new Dictionary<string, dynamic> {
                { "name", Name },
                { "display_text", DisplayText },
                { "is_checked", IsChecked },
                { "is_prompt_item", IsPromptItem }
            };
        }

        // FromDict
        public static ContentItemDataDefinition FromDict(Dictionary<string, dynamic> dict) {
            if (dict == null) {
                throw new ArgumentNullException(nameof(dict), "Dictionary cannot be null");
            }
            if (!dict.TryGetValue("name", out var name) || !(name is string)) {
                throw new ArgumentException("Dictionary must contain a valid 'name' key of type string");
            }
            if (!dict.TryGetValue("display_text", out var displayText) || !(displayText is string)) {
                throw new ArgumentException("Dictionary must contain a valid 'display_text' key of type string");
            }
            if (!dict.TryGetValue("is_checked", out var isChecked) || !(isChecked is bool)) {
                throw new ArgumentException("Dictionary must contain a valid 'is_checked' key of type bool");
            }
            if (!dict.TryGetValue("is_prompt_item", out var isPromptItem) || !(isPromptItem is bool)) {
                throw new ArgumentException("Dictionary must contain a valid 'is_prompt_item' key of type bool");
            }
            return new ContentItemDataDefinition((string)name, (string)displayText, (bool)isChecked, (bool)isPromptItem);
        }
    }
}
