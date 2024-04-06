using static WK.Libraries.SharpClipboardNS.SharpClipboard;
using WK.Libraries.SharpClipboardNS;
using System.Windows;
using System.Collections.ObjectModel;

namespace WpfApp1
{
    public class ClipboardItem
    {
        public int Id { get; set; }

        public string? CollectionName { get; set;}

        // 生成日時
        public DateTime CreatedAt { get; set; }

        // 更新日時
        public DateTime UpdatedAt { get; set; }

        // クリップボードの内容
        public string Content { get; set; } = "";
        // クリップボードの内容の種類
        public SharpClipboard.ContentTypes ContentType { get; set; }

        //Tags
        public List<string> Tags { get; set; } = new List<string>();

        //説明
        public string Description { get; set; } = "";

        //　貼り付け元のアプリケーション名
        public string SourceApplicationName { get; set; } = "";
        //　貼り付け元のアプリケーションのタイトル
        public string SourceApplicationTitle { get; set; } = "";
        //　貼り付け元のアプリケーションのID
        public int? SourceApplicationID { get; set; }
        //　貼り付け元のアプリケーションのパス
        public string? SourceApplicationPath { get; set; }

        // 自動処理時のコピー回数カウンター
        public int CopyCount { get; set; } = 0;

        public ClipboardItem()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        public ClipboardItem Copy()
        {
            ClipboardItem newItem = new ClipboardItem();
            newItem.CreatedAt = this.CreatedAt;
            newItem.UpdatedAt = this.UpdatedAt;
            newItem.Content = this.Content;
            newItem.ContentType = this.ContentType;
            newItem.SourceApplicationName = this.SourceApplicationName;
            newItem.SourceApplicationTitle = this.SourceApplicationTitle;
            newItem.SourceApplicationID = this.SourceApplicationID;
            newItem.SourceApplicationPath = this.SourceApplicationPath;
            newItem.Tags = new List<string>(this.Tags);
            newItem.Description = this.Description;
            return newItem;

        }
        // 重複をチェックするメソッド
        public bool IsDuplicate(ClipboardItem item)
        {
            if (item == null)
            {
                return false;
            }
            // Contentが一致してない場合は場合は重複ではない。
            if (item.Content != this.Content)
            {
                return false;
            }
            // Typeが一致していない場合は重複ではない。
            if (item.ContentType != this.ContentType)
            {
                return false;
            }
            // Tagsが一致していない場合は重複ではない。
            if (item.Tags.Count != this.Tags.Count)
            {
                return false;
            }
            for (int i = 0; i < item.Tags.Count; i++)
            {
                if (item.Tags[i] != this.Tags[i])
                {
                    return false;
                }
            }
            // Descriptionが一致していない場合は重複ではない。
            if (item.Description != this.Description)
            {
                return false;
            }
            // SourceApplicationNameが一致していない場合は重複ではない。
            if (item.SourceApplicationName != this.SourceApplicationName)
            {
                return false;
            }
            // SourceApplicationTitleが一致していない場合は重複ではない。
            if (item.SourceApplicationTitle != this.SourceApplicationTitle)
            {
                return false;
            }
            // SourceApplicationIDが一致していない場合は重複ではない。
            if (item.SourceApplicationID != this.SourceApplicationID)
            {
                return false;
            }
            // SourceApplicationPathが一致していない場合は重複ではない。
            if (item.SourceApplicationPath != this.SourceApplicationPath)
            {
                return false;
            }
            return true;

        }  
        // タグ表示用の文字列
        public string TagsString()
        {
            return string.Join(",", Tags);
        }

        // 表示用の文字列
        public string? HeaderText
        {
            get
            {
                string header1 = "";
                // 更新日時文字列を追加
                header1 += "[更新日時]" + UpdatedAt.ToString("yyyy/MM/dd HH:mm:ss") + "\n";
                // 作成日時文字列を追加
                header1 += "[作成日時]" + CreatedAt.ToString("yyyy/MM/dd HH:mm:ss") + "\n";
                // 貼り付け元のアプリケーション名を追加
                header1 += "[ソースアプリ名]" + SourceApplicationName + "\n";
                // 貼り付け元のアプリケーションのタイトルを追加
                header1 += "[ソースタイトル]" + SourceApplicationTitle + "\n";
                // Tags
                header1 += "[タグ]" + TagsString() + "\n";


                if (ContentType == SharpClipboard.ContentTypes.Text)
                {
                    return header1 + "[種類]Text";
                }
                else if (ContentType == SharpClipboard.ContentTypes.Files)
                {
                    return header1 + "[種類]File";
                }
                else
                {
                    return header1 + " [種類]Unknown";
                }
            }
        }
        public string DescriptionText
        {
            get
            {
                return "【" + Description + "】";
            }
        }
        
        public string ToolTipString
        {
            get
            {
                string result = "";
                if ( string.IsNullOrEmpty(Description) == false)
                {
                    result += DescriptionText + "\n";
                }
                result += HeaderText + "\n" + Content;
                return result;
            }
        }

        public void SetApplicationInfo(ClipboardChangedEventArgs sender)
        {
            SourceApplicationName = sender.SourceApplication.Name;
            SourceApplicationTitle = sender.SourceApplication.Title;
            SourceApplicationID = sender.SourceApplication.ID;
            SourceApplicationPath = sender.SourceApplication.Path;
        }
        // GUI関連
        public Visibility DescriptionVisibility
        {
            get
            {
                if (string.IsNullOrEmpty(Description))
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }
        // 分類がFileの場合はVisible,それ以外はCollapsed
        public Visibility FileVisibility
        {
            get
            {
                if (ContentType == SharpClipboard.ContentTypes.Files)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }
        // 分類がTextの場合はVisible,それ以外はCollapsed
        public Visibility TextVisibility
        {
            get
            {
                if (ContentType == SharpClipboard.ContentTypes.Text)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }
        // Ctrl+Cで実行するコマンド
        public static SimpleDelegateCommand CopyToClipboardCommand => new(ClipboardItemCommands.CopyToClipboardCommandExecute);

        // コンテキストメニューの「開く」の実行用コマンド
        public  SimpleDelegateCommand OpenItemCommand => new(ClipboardItemCommands.OpenSelectedItemCommandExecute);
        // コンテキストメニューの「新規で開く」の実行用コマンド
        public  SimpleDelegateCommand OpenItemAsNewCommand => new(ClipboardItemCommands.OpenSelectedItemAsNewCommandExecute);
        // コンテキストメニューの「編集」の実行用コマンド
        public  SimpleDelegateCommand EditItemCommand => new SimpleDelegateCommand(ClipboardItemCommands.EditItemCommandExecute);
        // コンテキストメニューの「タグ」の実行用コマンド
        public static SimpleDelegateCommand EditTagItemCommand => new SimpleDelegateCommand(ClipboardItemCommands.EditTagCommandExecute);
        // コンテキストメニューの「削除」の実行用コマンド
        public  SimpleDelegateCommand DeleteItemCommand => new SimpleDelegateCommand(ClipboardItemCommands.DeleteSelectedItemCommandExecute);
        // コンテキストメニューの「テキストを抽出」の実行用コマンド
        public  SimpleDelegateCommand ExtractTextCommand => new SimpleDelegateCommand(PythonCommands.ExtractTextCommandExecute);
        // コンテキストメニューの「データをマスキング」の実行用コマンド
        public  SimpleDelegateCommand MaskDataCommand => new SimpleDelegateCommand(PythonCommands.MaskDataCommandExecute);

        // コンテキストメニューの「OpenAIチャット」を実行するコマンド
        public  SimpleDelegateCommand OpenAIChatCommand => new SimpleDelegateCommand(PythonCommands.OpenAIChatCommandExecute);

        // コンテキストメニューの「スクリプト」のItemSource
        public static ObservableCollection<ScriptItem> ScriptItems => PythonExecutor.ScriptItems;
    }

}
