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
    public partial class ClipboardItemViewModel : CommonViewModelBase {

        // コンストラクタ
        public ClipboardItemViewModel(ClipboardFolderViewModel folderViewModel, Model.ClipboardItem clipboardItem) {
            ClipboardItem = clipboardItem;
            FolderViewModel = folderViewModel;

            OnPropertyChanged(nameof(Content));
            OnPropertyChanged(nameof(TextTabVisibility));
            OnPropertyChanged(nameof(FileTabVisibility));

        }
        // StringResources
        public CommonStringResources StringResources { get; } = CommonStringResources.Instance;

        // ContentItem
        public Model.ClipboardItem ClipboardItem { get; }
        // FolderViewModel
        public ClipboardFolderViewModel FolderViewModel { get; set; }


        #region PromptItemsに依存する処理
        // Context Menu

        public ObservableCollection<MenuItem> ContentItemMenuItems {
            get {
                ClipboardItemMenu clipboardItemMenu = new(this);
                return clipboardItemMenu.ContentItemMenuItems;
            }
        }

        #endregion
        // TextWrapping
        public TextWrapping TextWrapping {
            get {
                return ClipboardAppConfig.Instance.TextWrapping;
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


        // ChatItemsText
        public string ChatItemsText {
            get {
                return ClipboardItem.ChatItemsText;
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

        // ChatItemsTextが空でない場合はVisible,それ以外はCollapsed
        public Visibility ChatItemsTextTabVisibility {
            get {
                if (string.IsNullOrEmpty(ClipboardItem.ChatItemsText) == false) {
                    return Visibility.Visible;
                } else {
                    return Visibility.Collapsed;
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
        // ImageVisibility
        public Visibility ImageVisibility {
            get {
                return ClipboardItem.IsImage() ? Visibility.Visible : Visibility.Collapsed;
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
        public BitmapImage? Image {
            get {
                return ClipboardItem.BitmapImage;
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
        // VectorizedAtString
        public string VectorizedAtString {
            get {
                return ClipboardItem.VectorizedAtString;
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
                // Content 
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


        // SelectedTabIndex
        private int selectedTabIndex = 0;
        public int SelectedTabIndex {
            get {
                return selectedTabIndex;
            }
            set {
                selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
            }
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

        #endregion
    }
}
