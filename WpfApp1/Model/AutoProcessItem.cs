using System.Collections.ObjectModel;
using System.Windows.Controls;
using LiteDB;
using WpfApp1.Utils;

namespace WpfApp1.Model
{
    public class AutoProcessItem
    {
        public enum ActionType
        {
            CopyToFolder,
            MoveToFolder,
            ExtractText,
            MaskData,
            RunPythonScript,
        }

        public ObjectId? Id { get; set; }
        public string Name { get; set; } = "";
        public ActionType Type { get; set; }

        public string Description { get; set; } = "";
        public ScriptItem? ScriptItem { get; set; }

        public AutoProcessItem()
        {
        }
        public AutoProcessItem(string name, string description, ActionType actionType) : this()
        {
            Name = name;
            Description = description;
            Type = actionType;
        }

        public AutoProcessItem(string name, string description, ActionType actionType, ScriptItem scriptItem) : this(name, description, actionType)
        {
            ScriptItem = scriptItem;

        }

        public static ObservableCollection<AutoProcessItem> AutoProcessItems
        {
            get
            {
                ObservableCollection<AutoProcessItem> items = new ObservableCollection<AutoProcessItem>();

                // itemにフォルダにコピーするコマンドを追加
                items.Add(new AutoProcessItem("フォルダにコピー", "クリップボードの内容を指定されたフォルダにコピーします",
                    ActionType.CopyToFolder));
                // itemにフォルダに移動するコマンドを追加
                items.Add(new AutoProcessItem("フォルダに移動", "クリップボードの内容を指定されたフォルダに移動します",
                    ActionType.MoveToFolder));

                // itemにテキスト抽出コマンドを追加
                items.Add(new AutoProcessItem("テキスト抽出", "クリップボードのテキストを抽出します",
                    ActionType.ExtractText));
                // itemにデータマスキングコマンドを追加
                items.Add(new AutoProcessItem("データマスキング", "クリップボードのテキストをマスキングします",
                    ActionType.MaskData));

                // GetFolderCollectionでフォルダを取得

                // itemにPythonスクリプト実行コマンドを追加
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
                    items.Add(new AutoProcessItem(scriptItem.Description, $"Pythonスクリプト{scriptItem.Description}を実行します",
                        ActionType.RunPythonScript, scriptItem));
                }


                return items;
            }
        }
        public ClipboardItem? Execute(ClipboardItem clipboardItem, ClipboardItemFolder? destinationFolder)
        {
            // Type が RunPythonScriptの場合
            if (Type == ActionType.RunPythonScript)
            {
                if (ScriptItem == null)
                {
                    return clipboardItem;
                }
                // ScriptItemのRunScriptを実行
                return PythonCommands.AutoRunPythonScriptCommandExecute(ScriptItem, clipboardItem);
            }
            // Type が CopyToFolderの場合 
            if (Type == ActionType.CopyToFolder)
            {
                // DestinationFolderがNullの場合はそのまま返す
                if (destinationFolder == null)
                {
                    Tools.Warn("フォルダが選択されていません");
                    return clipboardItem;
                }
                Tools.Info($"フォルダにコピーします{destinationFolder.AbsoluteCollectionName}");
                // DestinationFolderにコピー
                ClipboardItem newItem = clipboardItem.Copy();
                destinationFolder.AddItem(newItem);
                // コピーの場合は元のアイテムを返す
                return clipboardItem;

            }
            // Type が MoveToFolderの場合
            if (Type == ActionType.MoveToFolder)
            {
                // DestinationFolderがNullの場合はそのまま返す
                if (destinationFolder == null)
                {
                    Tools.Warn("フォルダが選択されていません");
                    return clipboardItem;
                }
                // DestinationFolderに追加
                ClipboardItem newItem = clipboardItem.Copy();
                ClipboardItem result = destinationFolder.AddItem(newItem);
                // 元のフォルダから削除
                Tools.Info($"{clipboardItem.CollectionName}から削除します");
                ClipboardDatabaseController.DeleteItem(clipboardItem);
                // Moveの場合は元のアイテムを返さない
                return null;
            }
            // Type が ExtractTextの場合
            if (Type == ActionType.ExtractText)
            {
                return PythonCommands.AutoExtractTextCommandExecute(clipboardItem);
            }
            // Type が MaskDataの場合
            if (Type == ActionType.MaskData)
            {
                return PythonCommands.AutoMaskDataCommandExecute(clipboardItem);
            }
            return clipboardItem;
        }
    }

}
