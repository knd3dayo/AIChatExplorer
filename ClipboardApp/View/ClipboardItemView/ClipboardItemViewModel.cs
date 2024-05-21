using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.Views.ClipboardItemView;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon;
using WpfAppCommon.Model;

namespace ClipboardApp.View.ClipboardItemView {
    public partial class ClipboardItemViewModel(ClipboardFolderViewModel folderViewModel, ClipboardItem clipboardItem) : ObservableObject {
        // ClipboardItem
        public ClipboardItem ClipboardItem { get; } = clipboardItem;
        // FolderViewModel
        public ClipboardFolderViewModel FolderViewModel { get; } = folderViewModel;

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
        // Image
        public ImageSource? Image {
            get {
                return ClipboardItem.ClipboardItemImage?.GetBitmapImage();
            }

        }
        // ファイルパス
        public string? FilePath {
            get {
                return ClipboardItem.ClipboardItemFile?.FilePath;
            }
        }
        // フォルダ名
        public string? FolderName {
            get {
                return ClipboardItem.ClipboardItemFile?.FolderName;
            }
        }
        // ファイル名
        public string? FileName {
            get {
                return ClipboardItem.ClipboardItemFile?.FileName;
            }
        }
        // フォルダ名 + \n + ファイル名
        public string? FolderAndFileName {
            get {
                return FolderName + "\n" + FileName;
            }
        }


        // ThumbnailImage
        public ImageSource? ThumbnailImage {
            get {
                BitmapImage? thumbnailBitmapImage = ClipboardItem.ClipboardItemImage?.GetThumbnailBitmapImage();
                return thumbnailBitmapImage;
            }
        }

        // Description
        public string Description {
            get {
                return ClipboardItem.Description;
            }
            set {
                ClipboardItem.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        // Tags
        public HashSet<string> Tags {
            get {
                return ClipboardItem.Tags;
            }
            set {
                ClipboardItem.Tags = value;
                OnPropertyChanged(nameof(Tags));
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

        public ClipboardItemViewModel MaskDataCommandExecute() {
            return new ClipboardItemViewModel(FolderViewModel, ClipboardItem.MaskDataCommandExecute());
        }
        public void SetDataObject() {
            MainWindowViewModel.ClipboardController.SetDataObject(this.ClipboardItem);

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
        public string UpdatedAtString {
            get {
                return ClipboardItem.UpdatedAtString;
            }
        }
        public string ContentSummary {
            get {
                return ClipboardItem.ContentSummary;
            }
        }
        public string ContentTypeString {
            get {
                return ClipboardItem.ContentTypeString;
            }
        }

        // Save
        public void Save(bool updateModifiedTime = true) {
            ClipboardItem.Save(updateModifiedTime);
        }
        // Delete
        public void Delete() {
            ClipboardItem.Delete();
        }
        // IsPinned
        public bool IsPinned {
            get {
                return ClipboardItem.IsPinned;
            }
            set {
                ClipboardItem.IsPinned = value;
                // 保存
                ClipboardItem.Save();
                OnPropertyChanged(nameof(IsPinned));
            }
        }
        // ContentType
        public ClipboardContentTypes ContentType {
            get {
                return ClipboardItem.ContentType;
            }
        }

        // MergeItems
        public void MergeItems(List<ClipboardItemViewModel> itemViewModels, bool mergeWithHeader, Action<ActionMessage>? action) {
            List<ClipboardItem> items = [];
            foreach (var itemViewModel in itemViewModels) {
                items.Add(itemViewModel.ClipboardItem);
            }
            ClipboardItem.MergeItems(items, mergeWithHeader, action);
        }



        // SplitFilePathCommandExecute
        public void SplitFilePathCommandExecute() {
            ClipboardItem.SplitFilePathCommandExecute();
        }

        // Extract Image
        public static void ExtractTextFromImage(ClipboardItemViewModel clipboardItemViewModel) {
            ClipboardItem.ExtractTextFromImageCommandExecute(clipboardItemViewModel.ClipboardItem);

            // 保存
            clipboardItemViewModel.ClipboardItem.Save();
        }

        // Copy
        public ClipboardItemViewModel Copy() {
            return new ClipboardItemViewModel(FolderViewModel, ClipboardItem.Copy());
        }
        // OpenItem
        public void OpenItem(bool openAsNew = false) {
            ClipboardAppFactory.Instance.GetClipboardProcessController().OpenItem(ClipboardItem, true);
        }


        // コンテキストメニュー
        public ClipboardItemFolderContextMenuItems ContextMenuItems {
            get {
                return new ClipboardItemFolderContextMenuItems(this);
            }
        }


    }
}
