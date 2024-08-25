using System.Collections.ObjectModel;
using LiteDB;
using WpfAppCommon.Factory.Default;

namespace WpfAppCommon.Model.ClipboardApp {
    public enum ScriptType {
        Python,
        PowerShell,
        Batch,
        Shell,
        JavaScript,
        VBScript,
        CSharp,
        Other
    }
    public class ScriptItem {
        public ObjectId? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public ScriptType Type { get; set; }

        public ScriptItem(string name, string description, string content, ScriptType type) {
            Name = name;
            Description = description;
            Content = content;
            Type = type;
        }
        public static ObservableCollection<ScriptItem> ScriptItems {
            get {
                var collection = DefaultClipboardDBController.GetClipboardDatabase().GetCollection<ScriptItem>(DefaultClipboardDBController.SCRIPT_COLLECTION_NAME);
                return new ObservableCollection<ScriptItem>(collection.FindAll());
            }
        }
        public static void SaveScriptItem(ScriptItem scriptItem) {
            var collection = DefaultClipboardDBController.GetClipboardDatabase().GetCollection<ScriptItem>(DefaultClipboardDBController.SCRIPT_COLLECTION_NAME);
            collection.Upsert(scriptItem);
        }
        public static void DeleteScriptItem(ScriptItem scriptItem) {
            var collection = DefaultClipboardDBController.GetClipboardDatabase().GetCollection<ScriptItem>(DefaultClipboardDBController.SCRIPT_COLLECTION_NAME);
            collection.Delete(scriptItem.Id);
        }


    }
}
