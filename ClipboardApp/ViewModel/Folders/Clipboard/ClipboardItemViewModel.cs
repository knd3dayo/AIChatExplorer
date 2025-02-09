using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ClipboardApp.Model.Item;
using ClipboardApp.View.Item;
using ClipboardApp.ViewModel.Folders.Clipboard;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;
using PythonAILib.Model.Content;
using PythonAILib.Model.File;
using PythonAILib.Model.Prompt;
using PythonAILibUI.ViewModel.Item;

namespace ClipboardApp.ViewModel.Content {
    public partial class ClipboardItemViewModel : ContentItemViewModel {

        // コンストラクタ
        public ClipboardItemViewModel(ContentFolderViewModel folderViewModel, ContentItem clipboardItem) : base(folderViewModel, clipboardItem) {
            ContentItem = clipboardItem;
            FolderViewModel = folderViewModel;
            Content = ContentItem.Content;
            Description = ContentItem.Description;
            Tags = ContentItem.Tags;
            SourceApplicationTitleText = ContentItem.SourceApplicationTitle;
            Commands = new ClipboardItemViewModelCommands();
            OnPropertyChanged(nameof(Content));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(Tags));
            OnPropertyChanged(nameof(SourceApplicationTitleText));
            OnPropertyChanged(nameof(FileTabVisibility));

        }

        // Commands
        public override ContentItemViewModelCommands Commands { get; set; }


        // Context Menu

        public ObservableCollection<MenuItem> ContentItemMenuItems {
            get {
                ClipboardItemMenu clipboardItemMenu = new(this);
                return clipboardItemMenu.ContentItemMenuItems;
            }
        }

        public string Content { get => ContentItem.Content; set { ContentItem.Content = value; } }


        // ChatItemsText
        public string ChatItemsText => ContentItem.ChatItemsText;

        // DisplayText
        public string Description { get => ContentItem.Description; set { ContentItem.Description = value; } }


        public string ToolTipString {
            get {
                string result = "";
                if (string.IsNullOrEmpty(ContentItem.Description) == false) {
                    result += DescriptionText + "\n";
                }
                result += HeaderText + "\n" + ContentItem.Content;
                return result;
            }
        }


        // GUI関連
        // 説明が空かつタグが空の場合はCollapsed,それ以外はVisible
        public Visibility DescriptionVisibility => Tools.BoolToVisibility(false == (string.IsNullOrEmpty(ContentItem.Description) && ContentItem.Tags.Count() == 0));

        // ChatItemsTextが空でない場合はVisible,それ以外はCollapsed
        public Visibility ChatItemsTextTabVisibility => Tools.BoolToVisibility(string.IsNullOrEmpty(ContentItem.ChatItemsText) == false);


        // ファイルタブの表示可否
        public Visibility FileTabVisibility => Tools.BoolToVisibility(ContentType == ContentTypes.ContentItemTypes.Files || ContentType == ContentTypes.ContentItemTypes.Image);

        // ImageVisibility
        public Visibility ImageVisibility => Tools.BoolToVisibility(ContentItem.IsImage());


        public string DescriptionText {
            get {
                string result = "";
                if (string.IsNullOrEmpty(ContentItem.Description) == false) {
                    result += ContentItem.Description;
                }
                return result;
            }
        }
        public string TagsText => string.Join(",", ContentItem.Tags);

        // Images
        public BitmapImage? Image => ContentItem.BitmapImage;

        public string SourceApplicationTitleText { get => ContentItem.SourceApplicationTitle; set { ContentItem.SourceApplicationTitle = value; } }

        // 表示用の文字列
        public string? HeaderText => ContentItem.HeaderText;

        public string UpdatedAtString => ContentItem.UpdatedAtString;

        // 作成時
        public string CreatedAtString => ContentItem.CreatedAtString;

        // VectorizedAtString
        public string VectorizedAtString => ContentItem.VectorizedAtString;

        public string ContentTypeString => ContentItem.ContentTypeString;

        // IsPinned
        public bool IsPinned {
            get {
                return ContentItem.IsPinned;
            }
            set {
                ContentItem.IsPinned = value;
                // 保存
                ContentItem.Save();
                OnPropertyChanged(nameof(IsPinned));
            }
        }

        // ContentType
        public ContentTypes.ContentItemTypes ContentType => ContentItem.ContentType;

        // Copy
        public ClipboardItemViewModel Copy() {
            ClipboardItem newItem = (ClipboardItem)ContentItem.Copy();
            return new ClipboardItemViewModel(FolderViewModel, newItem);
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
                    Visibility = Visibility.Visible
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
                foreach (string name in ContentItem.PromptChatResult.Results.Keys) {
                    if (promptNames.Contains(name) || SystemDefinedPromptNames.TitleGeneration.ToString().Equals(name)) {
                        continue;
                    }
                    promptNames.Add(name);
                }

                foreach (string promptName in promptNames) {
                    PromptResultViewModel promptViewModel = new(ContentItem.PromptChatResult, promptName);
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
