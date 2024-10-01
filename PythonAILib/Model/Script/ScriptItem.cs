using System.Collections.ObjectModel;
using LiteDB;
using PythonAILib.Resource;
using QAChat;

namespace PythonAILib.Model.Script {
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
                PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
                var collection = libManager.DataFactory.GetScriptItems();
                return new ObservableCollection<ScriptItem>(collection);
            }
        }
        public static void SaveScriptItem(ScriptItem scriptItem) {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            libManager.DataFactory.UpsertScriptItem(scriptItem);
        }
        public static void DeleteScriptItem(ScriptItem scriptItem) {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            libManager.DataFactory.DeleteScriptItem(scriptItem);
        }
    }
}
