using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ClipboardApp.Model;
using ClipboardApp.View.ClipboardItem;
using ClipboardApp.ViewModel.Main;
using PythonAILib.Model.File;
using PythonAILib.Model.Prompt;
using QAChat.Resource;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Content {
    public partial class ClipboardItemViewModel : ClipboardAppViewModelBase {

        // 最後に選択されたタブのインデックス
        public static int LastSelectedTabIndex = 0;

        // コンストラクタ
        public ClipboardItemViewModel(ClipboardFolderViewModel folderViewModel, Model.ClipboardItem clipboardItem) {
            ClipboardItem = clipboardItem;
            FolderViewModel = folderViewModel;
            Content = ClipboardItem.Content;
            Description = ClipboardItem.Description;
            Tags = ClipboardItem.Tags;
            SourceApplicationTitleText = ClipboardItem.SourceApplicationTitle;
            Commands = new ClipboardAppCommandExecute(this);

            OnPropertyChanged(nameof(Content));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(Tags));
            OnPropertyChanged(nameof(SourceApplicationTitleText));

            OnPropertyChanged(nameof(TextTabVisibility));
            OnPropertyChanged(nameof(FileTabVisibility));

        }

        public ClipboardAppCommandExecute Commands { get; }

        // ContentItem
        public Model.ClipboardItem ClipboardItem { get; }
        // FolderViewModel
        public ClipboardFolderViewModel FolderViewModel { get; set; }


        // Context Menu

        public ObservableCollection<MenuItem> ContentItemMenuItems {
            get {
                ClipboardItemMenu clipboardItemMenu = new(this);
                return clipboardItemMenu.ContentItemMenuItems;
            }
        }

        public string Content { get => ClipboardItem.Content; set { ClipboardItem.Content = value;  } }


        // ChatItemsText
        public string ChatItemsText => ClipboardItem.ChatItemsText;

        // DisplayText
        public string Description { get => ClipboardItem.Description; set { ClipboardItem.Description = value; } }

        // Tags
        public HashSet<string> Tags { get => ClipboardItem.Tags; set { ClipboardItem.Tags = value; }  }

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
        public Visibility DescriptionVisibility => Tools.BoolToVisibility(false == (string.IsNullOrEmpty(ClipboardItem.Description) && ClipboardItem.Tags.Count() == 0));

        // ChatItemsTextが空でない場合はVisible,それ以外はCollapsed
        public Visibility ChatItemsTextTabVisibility  => Tools.BoolToVisibility(string.IsNullOrEmpty(ClipboardItem.ChatItemsText) == false);

        // テキストタブの表示可否
        public Visibility TextTabVisibility  => Tools.BoolToVisibility(ContentType == ContentTypes.ContentItemTypes.Text);

        // ファイルタブの表示可否
        public Visibility FileTabVisibility  => Tools.BoolToVisibility(ContentType == ContentTypes.ContentItemTypes.Files || ContentType == ContentTypes.ContentItemTypes.Image);

        // ImageVisibility
        public Visibility ImageVisibility  => Tools.BoolToVisibility(ClipboardItem.IsImage());


        public string DescriptionText {
            get {
                string result = "";
                if (string.IsNullOrEmpty(ClipboardItem.Description) == false) {
                    result += ClipboardItem.Description;
                }
                return result;
            }
        }
        public string TagsText => string.Join(",", ClipboardItem.Tags);

        // Images
        public BitmapImage? Image => ClipboardItem.BitmapImage;

        public string SourceApplicationTitleText { get => ClipboardItem.SourceApplicationTitle; set { ClipboardItem.SourceApplicationTitle = value; } }

        // 表示用の文字列
        public string? HeaderText => ClipboardItem.HeaderText;

        public string UpdatedAtString => ClipboardItem.UpdatedAtString;

        // VectorizedAtString
        public string VectorizedAtString => ClipboardItem.VectorizedAtString;

        public string ContentTypeString => ClipboardItem.ContentTypeString;

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
        public ContentTypes.ContentItemTypes ContentType => ClipboardItem.ContentType;

        // SelectedTabIndex
        private int selectedTabIndex = LastSelectedTabIndex;
        public int SelectedTabIndex {
            get {
                return selectedTabIndex;
            }
            set {
                selectedTabIndex = value;
                LastSelectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
            }
        }

        // MergeItems
        public void MergeItems(List<ClipboardItemViewModel> itemViewModels) {
            List<Model.ClipboardItem> items = [];
            foreach (var itemViewModel in itemViewModels) {
                items.Add(itemViewModel.ClipboardItem);
            }
            ClipboardItem.MergeItems(items);
        }

        // Copy
        public ClipboardItemViewModel Copy() {
            return new ClipboardItemViewModel(FolderViewModel, ClipboardItem.Copy());
        }

        // TabItems 
        public ObservableCollection<TabItem> TabItems {
            get {
                ObservableCollection<TabItem> tabItems = [];
                // SourcePath 
                ContentPanel contentPanel = new() {
                    DataContext = this,
                };
                TabItem contentTabItem = new() {
                    Header = StringResources.Text,
                    Content = contentPanel,
                    Height = double.NaN,
                    Width = double.NaN,
                    Margin = new Thickness(3, 0, 3, 0),
                    Padding = new Thickness(0, 0, 0, 0),
                    FontSize = 10,
                    Visibility = TextTabVisibility
                };
                tabItems.Add(contentTabItem);
                // FileOrImage
                FilePanel filePanel = new() {
                    DataContext = this,
                };
                TabItem fileTabItem = new() {
                    Header = StringResources.FileOrImage,
                    Content = filePanel,
                    Height = double.NaN,
                    Width = double.NaN,
                    Margin = new Thickness(3, 0, 3, 0),
                    Padding = new Thickness(0, 0, 0, 0),
                    FontSize = 10,
                    Visibility = FileTabVisibility
                };
                tabItems.Add(fileTabItem);
                // ChatItemsTextのタブ
                TabItem chatItemsText = new() {
                    Header = StringResources.ChatContent,
                    Content = new ChatItemsTextPanel() { DataContext = this },
                    Height = double.NaN,
                    Width = double.NaN,
                    Margin = new Thickness(3, 0, 3, 0),
                    Padding = new Thickness(0, 0, 0, 0),
                    FontSize = 10,
                    Visibility = ChatItemsTextTabVisibility
                };

                tabItems.Add(chatItemsText);

                // PromptResultのタブ
                foreach (TabItem promptTabItem in SystemPromptResultTabItems) {
                    tabItems.Add(promptTabItem);
                }
                // ClipboardItemのTypeがFileの場合はFileTabを選択
                if (ClipboardItem.ContentType == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Files
                    || ClipboardItem.ContentType == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Image) {
                    SelectedTabIndex = 1;
                }
                return tabItems;
            }

        }

        // システム定義のPromptItemの結果表示用のタブを作成
        // TabItems 
        private ObservableCollection<TabItem> SystemPromptResultTabItems {
            get {
                ObservableCollection<TabItem> tabItems = [];
                // PromptResultのタブ
                List<string> promptNames = [
                    SystemDefinedPromptNames.BackgroundInformationGeneration.ToString(),
                    SystemDefinedPromptNames.TasksGeneration.ToString(),
                    SystemDefinedPromptNames.SummaryGeneration.ToString()
                    ];
                // PromptChatResultのエントリからPromptItemの名前を取得
                foreach (string name in ClipboardItem.PromptChatResult.Results.Keys) {
                    if (promptNames.Contains(name) || SystemDefinedPromptNames.TitleGeneration.ToString().Equals(name)) {
                        continue;
                    }
                    promptNames.Add(name);
                }

                foreach (string promptName in promptNames) {
                    PromptResultViewModel promptViewModel = new(ClipboardItem.PromptChatResult, promptName);
                    PromptItem? item = PromptItem.GetPromptItemByName(promptName);
                    if (item == null) {
                        continue;
                    }

                    object content = item.PromptResultType switch {
                        PromptResultTypeEnum.TextContent => new PromptResultTextPanel() { DataContext = promptViewModel },
                        PromptResultTypeEnum.TableContent => new PromptResultTablePanel() { DataContext = promptViewModel },
                        _ => ""
                    };
                    Visibility visibility = item.PromptResultType switch {
                        PromptResultTypeEnum.TextContent => promptViewModel.TextContentVisibility,
                        PromptResultTypeEnum.TableContent => promptViewModel.TableContentVisibility,
                        _ => Visibility.Collapsed
                    };

                    TabItem promptTabItem = new() {
                        Header = item.Description,
                        Content = content,
                        Height = double.NaN,
                        Width = double.NaN,
                        Margin = new Thickness(3, 0, 3, 0),
                        Padding = new Thickness(0, 0, 0, 0),
                        FontSize = 10,
                        Visibility = visibility
                    };
                    tabItems.Add(promptTabItem);
                }

                return tabItems;
            }

        }


    }
}
