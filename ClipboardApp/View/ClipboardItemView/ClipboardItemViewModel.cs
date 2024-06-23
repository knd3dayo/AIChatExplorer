using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ClipboardApp.Utils;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.Views.ClipboardItemView;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.ClipboardItemView {
    public partial class ClipboardItemViewModel : ObservableObject {

        // コンストラクタ
        public ClipboardItemViewModel(ClipboardItem clipboardItem) {
            this.ClipboardItem = clipboardItem;
            OnPropertyChanged(nameof(Content));
            OnPropertyChanged(nameof(Images));
            OnPropertyChanged(nameof(Files));
            OnPropertyChanged(nameof(ThumbnailImages));

        }
        // ClipboardItem
        public ClipboardItem ClipboardItem { get; }

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
        public ObservableCollection<ImageSource> Images {
            get {
                ObservableCollection<ImageSource> imageSources = [];
                foreach (var clipboardItemImage in ClipboardItem.ClipboardItemImages) {
                    BitmapImage? bitmapImage = clipboardItemImage.GetBitmapImage();
                    if (bitmapImage != null) {
                        imageSources.Add(bitmapImage);
                    }
                }
                return imageSources;
            }

        }
        // Files
        public ObservableCollection<ClipboardItemFile> Files {
            get {
                return [.. ClipboardItem.ClipboardItemFiles];
            }
        }

        // ThumbnailImage
        public ObservableCollection<ImageSource> ThumbnailImages {
            get {
                ObservableCollection<ImageSource> imageSources = new();
                foreach (var clipboardItemImage in ClipboardItem.ClipboardItemImages) {
                    BitmapImage? bitmapImage = clipboardItemImage.GetThumbnailBitmapImage();
                    if (bitmapImage != null) {
                        imageSources.Add(bitmapImage);
                    }
                }
                return imageSources;
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
                    result += ClipboardItem.Description;
                }
                if (ClipboardItem.Tags.Count > 0) {
                    result += " タグ：" + string.Join(",", ClipboardItem.Tags);
                }
                return result;
            }
        }

        public string SourceApplicationTitleText {
            get {
                return ClipboardItem.SourceApplicationTitle;
            }
            set {
                ClipboardItem.SourceApplicationTitle = value;
                OnPropertyChanged(nameof(SourceApplicationTitleText));
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


        // Extract Image
        private static void ExtractTextFromImage(ClipboardItemViewModel clipboardItemViewModel) {
            ClipboardItem.ExtractTextFromImageCommandExecute(clipboardItemViewModel.ClipboardItem);

            // 保存
            clipboardItemViewModel.ClipboardItem.Save();
        }

        // Copy
        public ClipboardItemViewModel Copy() {
            return new ClipboardItemViewModel(ClipboardItem.Copy());
        }


        // テキストをファイルとして開くコマンド
        public SimpleDelegateCommand<object> OpenContentAsFileCommand => new((obj) => {
            // 選択中のファイルを開く
            OpenContentAsFileCommandExecute(this);
        });
        // 選択中のアイテムを開く処理
        public static void OpenContentAsFileCommandExecute(ClipboardItemViewModel? itemViewModel) {
            if (itemViewModel == null) {
                LogWrapper.Error("クリップボードアイテムが選択されていません。");
                return;
            }
            try {
                // 選択中のアイテムを開く
                ClipboardAppFactory.Instance.GetClipboardProcessController().OpenClipboardItemContent(itemViewModel.ClipboardItem);
            } catch (ClipboardAppException e) {
                LogWrapper.Error(e.Message);
            }

        }


    }
}
