using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WK.Libraries.SharpClipboardNS;
using ClipboardApp.Model;
using ClipboardApp.PythonIF;
using ClipboardApp.Utils;

namespace ClipboardApp.View.ClipboardItemView
{
    public class ClipboardItemViewModel(ClipboardItem clipboardItem) : ObservableObject
    {

        // ClipboardItem
        public ClipboardItem ClipboardItem { get; set; } = clipboardItem;

        // Content
        public string Content
        {
            get
            {
                return ClipboardItem.Content;
            }
            set
            {
                ClipboardItem.Content = value;
                OnPropertyChanged("Content");
            }
        }

        public string ToolTipString
        {
            get
            {
                string result = "";
                if (string.IsNullOrEmpty(ClipboardItem.Description) == false)
                {
                    result += DescriptionText + "\n";
                }
                result += HeaderText + "\n" + ClipboardItem.Content;
                return result;
            }
        }
        // GUI関連
        // 説明が空かつタグが空の場合はCollapsed,それ以外はVisible
        public Visibility DescriptionVisibility
        {
            get
            {
                if (string.IsNullOrEmpty(ClipboardItem.Description) && ClipboardItem.Tags.Count() == 0)
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
                if (ClipboardItem.ContentType == SharpClipboard.ContentTypes.Files)
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
                if (ClipboardItem.ContentType == SharpClipboard.ContentTypes.Text)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }
        public string DescriptionText
        {
            get
            {
                string result = "";
                if (string.IsNullOrEmpty(ClipboardItem.Description) == false)
                {
                    result += "【" + ClipboardItem.Description + "】";
                }
                if ( ClipboardItem.Tags.Count > 0)
                {
                    result += "タグ：" + string.Join(",", ClipboardItem.Tags);
                }
                return result;
            }
        }

        // 表示用の文字列
        public string? HeaderText
        {
            get
            {
                return ClipboardItem.HeaderText;
            }
        }
        // コンテキストメニューの「テキストを抽出」の実行用コマンド
        public static SimpleDelegateCommand ExtractTextCommand => new SimpleDelegateCommand(ClipboardItemCommands.MenuItemExtractTextCommandExecute);
        // コンテキストメニューの「データをマスキング」の実行用コマンド
        public static SimpleDelegateCommand MaskDataCommand => new SimpleDelegateCommand(ClipboardItemCommands.MenuItemMaskDataCommandExecute);
     
    }
}
