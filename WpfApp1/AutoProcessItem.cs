using System.Collections.ObjectModel;

namespace WpfApp1
{
    public class AutoProcessItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string CommandPath { get; set; }

        public ScriptItem? ScriptItem { get; set; }

        public AutoProcessItem(string name, string description, string commandPath)
        {
            Name = name;
            Description = description;
            CommandPath = commandPath;
        }

        public AutoProcessItem(string name, string description, string commandPath, ScriptItem scriptItem) : this(name, description, commandPath)
        {
            ScriptItem = scriptItem;
        }

        public static ObservableCollection<AutoProcessItem> AutoProcessItems
        {
            get
            {
                ObservableCollection<AutoProcessItem> items = new ObservableCollection<AutoProcessItem>();
                // itemにテキスト抽出コマンドを追加
                string extractTextCommand = "AutoExtractTextCommandExecute";
                items.Add(new AutoProcessItem("テキスト抽出", "クリップボードのテキストを抽出します", extractTextCommand));
                // itemにデータマスキングコマンドを追加
                string maskDataCommand = "AutoMaskDataCommandExecute";
                items.Add(new AutoProcessItem("データマスキング", "クリップボードのテキストをマスキングします", maskDataCommand));
                // itemにPythonスクリプト実行コマンドを追加
                string runPythonScriptCommand = "AutoRunPythonScriptCommandExecute";
                // GetScriptItemCollectionでスクリプトを取得
                var scriptItems = ClipboardController.GetScriptItems();
                // スクリプトを追加
                foreach (var scriptItem in scriptItems)
                {
                    if (scriptItem.Type != ScriptType.Python)
                    {
                        continue;
                    }
                    if (scriptItem.Description == null)
                    {
                        continue;
                    }
                    if (scriptItem.Content == null)
                    {
                        continue;
                    }
                    items.Add(new AutoProcessItem(scriptItem.Description, $"Pythonスクリプト{scriptItem.Description}を実行します", runPythonScriptCommand, scriptItem));
                }


                return items;
            }
        }
        public ClipboardItem Execute(ClipboardItem clipboardItem)
        {
            // CommandPathから関数を取得
            System.Reflection.MethodInfo? method = typeof(PythonCommands).GetMethod(CommandPath);
            if (method == null)
            {
                return clipboardItem;
            }
            // 関数を実行
             object? obj =  method.Invoke(null, [ScriptItem, clipboardItem]);
            if (obj == null)
            {
                return clipboardItem;
            }
            if (obj is not ClipboardItem)
            {
                return clipboardItem;
            }
            return (ClipboardItem)obj;
        }

    }

}
