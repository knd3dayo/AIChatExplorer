using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Prompt;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Item;
using LibUIPythonAI.ViewModel.Folder;

namespace LibUIPythonAI.ViewModel.Item {
    public class ContentItemViewModel : CommonViewModelBase {

        public ContentItemViewModel(ContentFolderViewModel folderViewModel, ContentItemWrapper contentItemBase) {
            ContentItem = contentItemBase;
            FolderViewModel = folderViewModel;
            Commands = FolderViewModel.Commands;

        }
        public ContentItemWrapper ContentItem { get; set; }

        // FolderViewModel
        public ContentFolderViewModel FolderViewModel { get; set; }

        public ContentItemViewModelCommands Commands { get; set; }

        // IsSelected
        private bool isSelected = false;
        public bool IsSelected {
            get {
                return isSelected;
            }
            set {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
        // IsChecked
        private bool isChecked = false;
        public bool IsChecked {
            get {
                return isChecked;
            }
            set {
                isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        // Tags
        public HashSet<string> Tags {
            get => ContentItem.Tags;
            set => ContentItem.Tags = value;
        }
        // LastSelectedTabIndex
        public int LastSelectedTabIndex { get; set; } = 0;

        // SelectedTabIndex
        private int selectedTabIndex = -1;
        public int SelectedTabIndex {
            get {
                return selectedTabIndex;
            }
            set {
                selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
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


        public Visibility MarkdownVisibility => Tools.BoolToVisibility(CommonViewModelProperties.MarkdownView);

        public Visibility TextVisibility => Tools.BoolToVisibility(CommonViewModelProperties.MarkdownView == false);

        public FlowDocument? MarkdownContent => CommonViewModelProperties.MarkdownView ? LibPythonAI.Utils.Common.Tools.CreateFlowDocument(ContentItem.Content) : null;

        public FlowDocument? MarkdownChatItemsText => CommonViewModelProperties.MarkdownView ? LibPythonAI.Utils.Common.Tools.CreateFlowDocument(ContentItem.ChatItemsText) : null;

        // ContentType
        public ContentItemTypes.ContentItemTypeEnum ContentType => ContentItem.ContentType;

        // ContentPanelContentHint
        public string ContentPanelContentHint {
            get {
                if (ContentItem.SourceType == ContentSourceType.File) {
                    return CommonStringResources.Instance.ExecuteExtractTextToViewFileContent;
                }
                // URLの場合
                if (ContentItem.SourceType == ContentSourceType.Url) {
                    return CommonStringResources.Instance.ExecuteDownloadWebPageToViewContent;
                }
                // 画像の場合
                if (ContentItem.ContentType == ContentItemTypes.ContentItemTypeEnum.Image) {
                    return CommonStringResources.Instance.ExecuteExtractTextToViewFileContent;
                }

                return "";
            }
        }

        public void UpdateView(TabControl? tabControl) {
            // 選択中のタブを更新する処理
            UpdateTabItems(tabControl);
        }
        
        private void UpdateTabItems(TabControl? tabControl) {
            if (tabControl == null) {
                return;
            }

            tabControl.Items.Clear();
            // Path 
            ContentPanel contentPanel = new() {
                DataContext = this,
            };
            Binding binding = new() {
                Source = ThisUserControl
            };

            TabItem contentTabItem = new();
            tabControl.Items.Add(contentTabItem);

            contentTabItem.Header = CommonStringResources.Instance.Text;
            contentTabItem.Content = contentPanel;
            contentTabItem.Height = double.NaN;
            contentTabItem.Width = double.NaN;
            contentTabItem.Margin = new Thickness(3, 0, 3, 0);
            contentTabItem.Padding = new Thickness(0, 0, 0, 0);
            contentTabItem.FontSize = 10;
            contentTabItem.Visibility = Visibility.Visible;



            // FileOrImage
            FilePanel filePanel = new() {
                DataContext = this,
            };
            TabItem fileTabItem = new();
            tabControl.Items.Add(fileTabItem);

            fileTabItem.Header = CommonStringResources.Instance.FileOrImage;
            fileTabItem.Content = filePanel;
            fileTabItem.Height = double.NaN;
            fileTabItem.Width = double.NaN;
            fileTabItem.Margin = new Thickness(3, 0, 3, 0);
            fileTabItem.Padding = new Thickness(0, 0, 0, 0);
            fileTabItem.FontSize = 10;
            fileTabItem.Visibility = FileTabVisibility;


            // ChatItemsTextのタブ
            TabItem chatItemsText = new();
            tabControl.Items.Add(chatItemsText);

            chatItemsText.Header = CommonStringResources.Instance.ChatContent;
            chatItemsText.Content = new ChatItemsTextPanel() { DataContext = this };
            chatItemsText.Height = double.NaN;
            chatItemsText.Width = double.NaN;
            chatItemsText.Margin = new Thickness(3, 0, 3, 0);
            chatItemsText.Padding = new Thickness(0, 0, 0, 0);
            chatItemsText.FontSize = 10;
            chatItemsText.Visibility = ChatItemsTextTabVisibility;


            // PromptResultのタブ
            UpdatePromptResultTabItems(tabControl);

            SelectedTabIndex = LastSelectedTabIndex;

        }

        // PromptItemの結果表示用のタブを作成
        // TabItems 
        private void UpdatePromptResultTabItems(TabControl tabControl) {
            // PromptResultのタブ
            foreach (string promptName in ContentItem.PromptChatResult.Results.Keys) {
                // TitleGenerationの場合は除外
                if ( promptName == SystemDefinedPromptNames.TitleGeneration.ToString()) {
                    continue;
                }

                PromptResultViewModel promptViewModel = new(ContentItem.PromptChatResult, promptName);
                PromptItem? item = PromptItem.GetPromptItemByName(promptName);
                if (item == null) {
                    continue;
                }

                object content = item.PromptResultType switch {
                    PromptResultTypeEnum.TextContent => new PromptResultTextPanel() { DataContext = promptViewModel },
                    PromptResultTypeEnum.TableContent => new PromptResultTablePanel() { DataContext = promptViewModel },
                    PromptResultTypeEnum.ListContent => new PromptResultTablePanel() { DataContext = promptViewModel },
                    PromptResultTypeEnum.DictionaryContent => new PromptResultTablePanel() { DataContext = promptViewModel },
                    _ => ""
                };
                Visibility visibility = item.PromptResultType switch {
                    PromptResultTypeEnum.TextContent => promptViewModel.TextContentVisibility,
                    PromptResultTypeEnum.TableContent => promptViewModel.TableContentVisibility,
                    PromptResultTypeEnum.ListContent => promptViewModel.TableContentVisibility,
                    PromptResultTypeEnum.DictionaryContent => promptViewModel.TableContentVisibility,
                    _ => Visibility.Collapsed
                };

                TabItem promptTabItem = new();
                tabControl.Items.Add(promptTabItem);

                promptTabItem.Header = item.Description;
                promptTabItem.Content = content;
                promptTabItem.Height = double.NaN;
                promptTabItem.Width = double.NaN;
                promptTabItem.Margin = new Thickness(3, 0, 3, 0);
                promptTabItem.Padding = new Thickness(0, 0, 0, 0);
                promptTabItem.FontSize = 10;
                promptTabItem.Visibility = visibility;
            }

        }
        // DeleteItems
        public static Task DeleteItems(List<ContentItemViewModel> items) {
            return Task.Run(() => {
                var contentItems = items.Select(item => item.ContentItem).ToList();
                ContentItemWrapper.DeleteItems(contentItems);
            });
        }



        // GUI関連
        // 説明が空かつタグが空の場合はCollapsed,それ以外はVisible
        public Visibility DescriptionVisibility => Tools.BoolToVisibility(false == (string.IsNullOrEmpty(ContentItem.Description) && ContentItem.Tags.Count() == 0));

        // ChatItemsTextが空でない場合はVisible,それ以外はCollapsed
        public Visibility ChatItemsTextTabVisibility => Tools.BoolToVisibility(string.IsNullOrEmpty(ContentItem.ChatItemsText) == false);


        // ファイルタブの表示可否
        public Visibility FileTabVisibility => Tools.BoolToVisibility(ContentType == ContentItemTypes.ContentItemTypeEnum.Files || ContentType == ContentItemTypes.ContentItemTypeEnum.Image);

        // ImageVisibility
        public Visibility ImageVisibility => Tools.BoolToVisibility(ContentItem.IsImage());


    }
}
