using System.Windows;
using ClipboardApp.View.ClipboardItemFolderView;
using CommunityToolkit.Mvvm.ComponentModel;
using QAChat.View.PromptTemplateWindow;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.ClipboardItemView {
    public class ClipboardItemViewModel(ClipboardFolderViewModel folderViewModel, ClipboardItem clipboardItem) : ObservableObject {

        // ClipboardItem
        public ClipboardItem ClipboardItem { get; } = clipboardItem;
        // FolderViewModel
        public ClipboardFolderViewModel FolderViewModel { get; } = folderViewModel;

        // MainWindowViewModel
        public MainWindowViewModel MainWindowViewModel { get; } = folderViewModel.MainWindowViewModel;

        // Content
        public string Content {
            get {
                return ClipboardItem.Content;
            }
            set {
                ClipboardItem.Content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        public string ToolTipString {
            get {
                string result = "";
                if (string.IsNullOrEmpty(ClipboardItem.Description) == false) {
                    result += DescriptionText + "\n";
                }
                result += HeaderText + "\n" + ClipboardItem.Content;
                return result;
            }
        }
        // GUI関連
        // 説明が空かつタグが空の場合はCollapsed,それ以外はVisible
        public Visibility DescriptionVisibility {
            get {
                if (string.IsNullOrEmpty(ClipboardItem.Description) && ClipboardItem.Tags.Count() == 0) {
                    return Visibility.Collapsed;
                } else {
                    return Visibility.Visible;
                }
            }
        }
        // 分類がFileの場合はVisible,それ以外はCollapsed
        public Visibility FileVisibility {
            get {
                if (ClipboardItem.ContentType == ClipboardContentTypes.Files) {
                    return Visibility.Visible;
                } else {
                    return Visibility.Collapsed;
                }
            }
        }
        // 分類がTextの場合はVisible,それ以外はCollapsed
        public Visibility TextVisibility {
            get {
                if (ClipboardItem.ContentType == ClipboardContentTypes.Text) {
                    return Visibility.Visible;
                } else {
                    return Visibility.Collapsed;
                }
            }
        }
        public string DescriptionText {
            get {
                string result = "";
                if (string.IsNullOrEmpty(ClipboardItem.Description) == false) {
                    result += "【" + ClipboardItem.Description + "】";
                }
                if (ClipboardItem.Tags.Count > 0) {
                    result += "タグ：" + string.Join(",", ClipboardItem.Tags);
                }
                return result;
            }
        }

        // 表示用の文字列
        public string? HeaderText {
            get {
                return ClipboardItem.HeaderText;
            }
        }
        // コンテキストメニューの「テキストを抽出」の実行用コマンド
        public static SimpleDelegateCommand ExtractTextCommand => new((parameter) => {
            ClipboardItemCommands.MenuItemExtractTextCommandExecute(parameter);
        });
        // コンテキストメニューの「データをマスキング」の実行用コマンド
        public static SimpleDelegateCommand MaskDataCommand => new((parameter) => {
            if (MainWindowViewModel.SelectedItemStatic == null) {
                Tools.Error("クリップボードアイテムが選択されていません。");
                return;
            }
            ClipboardItemCommands.MenuItemMaskDataCommandExecute(MainWindowViewModel.SelectedItemStatic);
        });



    }
}
