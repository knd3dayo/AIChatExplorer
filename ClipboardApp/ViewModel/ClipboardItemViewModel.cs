using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ClipboardApp.Factory;
using ClipboardApp.Model;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Search;
using ClipboardApp.View.SelectVectorDBView;
using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Model.Content;
using PythonAILib.Model.File;
using PythonAILib.Model.VectorDB;
using QAChat.Control;
using QAChat.View.VectorDBWindow;
using QAChat.ViewModel.VectorDBWindow;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel {
    public partial class ClipboardItemViewModel : ObservableObject {

        // コンストラクタ
        public ClipboardItemViewModel(ClipboardFolderViewModel folderViewModel, ClipboardItem clipboardItem) {
            ClipboardItem = clipboardItem;
            FolderViewModel = folderViewModel;

            OnPropertyChanged(nameof(Content));
            OnPropertyChanged(nameof(Files));
            OnPropertyChanged(nameof(TextTabVisibility));
            OnPropertyChanged(nameof(FileTabVisibility));
            OnPropertyChanged(nameof(BackgroundInfoVisibility));
            OnPropertyChanged(nameof(SummaryVisibility));

        }
        // StringResources
        public CommonStringResources StringResources { get; } = CommonStringResources.Instance;

        // ContentItem
        public ClipboardItem ClipboardItem { get; }
        // FolderViewModel
        public ClipboardFolderViewModel FolderViewModel { get; set; }

        // Context Menu

        public ObservableCollection<MenuItem> MenuItems {
            get {
                return FolderViewModel.CreateItemContextMenuItems(this);
            }
        }

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

        // BackgroundInfo
        public string BackgroundInfo {
            get {
                return ClipboardItem.BackgroundInfo;
            }
            set {
                ClipboardItem.BackgroundInfo = value;
                OnPropertyChanged(nameof(BackgroundInfo));
            }
        }
        public string Summary {
            get {
                return ClipboardItem.Summary;
            }
            set {
                ClipboardItem.Summary = value;
                OnPropertyChanged(nameof(Summary));
            }
        }

        // Files
        public ObservableCollection<ContentAttachedItem> Files {
            get {
                return [.. ClipboardItem.ClipboardItemFiles];
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

        // Tasks
        public ObservableCollection<TaskItem> Tasks {
            get {
                return [.. ClipboardItem.Tasks];
            }
            set {
                ClipboardItem.Tasks = [.. value];
                OnPropertyChanged(nameof(Tasks));
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
                if (ClipboardItem.ContentType == ContentTypes.ContentItemTypes.Files) {
                    return Visibility.Visible;
                } else {
                    return Visibility.Collapsed;
                }
            }
        }
        // Contentが空でない場合はVisible,それ以外はCollapsed
        public Visibility TextVisibility {
            get {
                if (string.IsNullOrEmpty(ClipboardItem.Content) == false) {
                    return Visibility.Visible;
                } else {
                    return Visibility.Collapsed;
                }
            }
        }
        // ClipboardItemFilesが空でない場合はVisible,それ以外はCollapsed
        public Visibility FileOrImageVisibility {
            get {
                if (ClipboardItem.ClipboardItemFiles.Count > 0) {
                    return Visibility.Visible;
                } else {
                    return Visibility.Collapsed;
                }
            }
        }

        // BackgroundInfoが空の場合はCollapsed,それ以外はVisible
        public Visibility BackgroundInfoVisibility {
            get {
                if (string.IsNullOrEmpty(ClipboardItem.BackgroundInfo)) {
                    return Visibility.Collapsed;
                } else {
                    return Visibility.Visible;
                }
            }
        }
        // Tasksが空の場合はCollapsed,それ以外はVisible
        public Visibility TasksVisibility {
            get {
                if (ClipboardItem.Tasks.Count == 0) {
                    return Visibility.Collapsed;
                } else {
                    return Visibility.Visible;
                }
            }
        }

        // サマリーが空の場合はCollapsed,それ以外はVisible
        public Visibility SummaryVisibility {
            get {
                if (string.IsNullOrEmpty(ClipboardItem.Summary)) {
                    return Visibility.Collapsed;
                } else {
                    return Visibility.Visible;
                }
            }
        }
        // テキストタブの表示可否
        public Visibility TextTabVisibility {
            get {
                return ContentType == ContentTypes.ContentItemTypes.Text ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // ファイルタブの表示可否
        public Visibility FileTabVisibility {
            get {
                return (ContentType == ContentTypes.ContentItemTypes.Files || ContentType == ContentTypes.ContentItemTypes.Image) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public string DescriptionText {
            get {
                string result = "";
                if (string.IsNullOrEmpty(ClipboardItem.Description) == false) {
                    result += ClipboardItem.Description;
                }
                return result;
            }
        }
        public string TagsText {
            get {
                return string.Join(",", ClipboardItem.Tags);
            }
        }

        // Images
        public ObservableCollection<BitmapImage> Images {
            get {
                ObservableCollection<BitmapImage> images = [];
                foreach (var file in ClipboardItem.ClipboardItemFiles) {
                    if (file.BitmapImage != null) {
                        images.Add(file.BitmapImage);
                    }
                }
                return images;
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

        public string ContentTypeString {
            get {
                return ClipboardItem.ContentTypeString;
            }
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
        public ContentTypes.ContentItemTypes ContentType {
            get {
                return ClipboardItem.ContentType;
            }
        }

        // MergeItems
        public void MergeItems(List<ClipboardItemViewModel> itemViewModels) {
            List<ClipboardItem> items = [];
            foreach (var itemViewModel in itemViewModels) {
                items.Add(itemViewModel.ClipboardItem);
            }
            ClipboardItem.MergeItems(items);
        }


        // Copy
        public ClipboardItemViewModel Copy() {
            return new ClipboardItemViewModel(FolderViewModel, ClipboardItem.Copy());
        }

        #region コマンド
        // アイテム保存
        public SimpleDelegateCommand<bool> SaveClipboardItemCommand => new(ClipboardItem.Save);

        // Delete
        public SimpleDelegateCommand<ClipboardItemViewModel> DeleteItemCommand => new((obj) => {
            ClipboardItem.Delete();
        });
        // フォルダを開くコマンド
        public SimpleDelegateCommand<object> OpenFolderCommand => new((parameter) => {
            ClipboardAppCommandExecute.OpenFolderCommand(ClipboardItem);
        });

        // コンテキストメニューの「テキストを抽出」の実行用コマンド
        public SimpleDelegateCommand<object> ExtractTextCommand => new((parameter) => {
            ClipboardAppCommandExecute.ExtractTextCommand(ClipboardItem);
        });

        // OpenAI Chatを開くコマンド
        public SimpleDelegateCommand<object> OpenOpenAIChatWindowCommand => new((parameter) => {
            ClipboardAppCommandExecute.OpenOpenAIChatWindowCommand(ClipboardItem);
        });

        // ファイルを開くコマンド
        public SimpleDelegateCommand<object> OpenFileCommand => new((obj) => {
            ClipboardAppCommandExecute.OpenFileCommand(ClipboardItem);
        });

        // ファイルを新規ファイルとして開くコマンド
        public SimpleDelegateCommand<object> OpenFileAsNewFileCommand => new((obj) => {
            ClipboardAppCommandExecute.OpenFileAsNewFileCommand(ClipboardItem);
        });

        // タイトルを生成するコマンド
        public SimpleDelegateCommand<object> GenerateTitleCommand => new((obj) => {
            ClipboardAppCommandExecute.GenerateTitleCommand([ClipboardItem], obj);
        });

        // 背景情報を生成するコマンド
        public SimpleDelegateCommand<object> GenerateBackgroundInfoCommand => new((obj) => {
            ClipboardAppCommandExecute.GenerateBackgroundInfoCommand([ClipboardItem], obj);

        });

        // サマリーを生成するコマンド
        public SimpleDelegateCommand<object> GenerateSummaryCommand => new((obj) => {
            ClipboardAppCommandExecute.GenerateSummaryCommand([ClipboardItem], obj);
        });

        // 課題リストを生成するコマンド
        public SimpleDelegateCommand<object> GenerateTasksCommand => new((obj) => {
            ClipboardAppCommandExecute.GenerateTasksCommand([ClipboardItem], obj);
        });

        // ベクトルを生成するコマンド
        public SimpleDelegateCommand<object> GenerateVectorCommand => new((obj) => {
            ClipboardAppCommandExecute.GenerateVectorCommand([ClipboardItem], obj);
        });

        // ベクトル検索を実行するコマンド
        public SimpleDelegateCommand<object> VectorSearchCommand => new((obj) => {
            ClipboardAppCommandExecute.OpenVectorSearchWindowCommand(ClipboardItem);
        });

        // テキストをファイルとして開くコマンド
        public SimpleDelegateCommand<object> OpenContentAsFileCommand => new((obj) => {
            ClipboardAppCommandExecute.OpenContentAsFileCommand(ClipboardItem);
        });

        // ピン留めの切り替えコマンド
        public SimpleDelegateCommand<object> ChangePinCommand => new((obj) => {
            IsPinned = !IsPinned;
            // ピン留めの時は更新日時を変更しない
            SaveClipboardItemCommand.Execute(false);
        });

        // Tasksの削除
        public SimpleDelegateCommand<TaskItem> DeleteTaskCommand => new((item) => {
            ClipboardAppCommandExecute.DeleteTaskCommand(ClipboardItem, item);
            OnPropertyChanged(nameof(Tasks));

        });

        #endregion
    }
}
