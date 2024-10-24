using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClipboardApp.Factory;
using ClipboardApp.Model;
using ClipboardApp.Model.Folder;
using ClipboardApp.View.ClipboardItemView;
using PythonAILib.Model.Prompt;
using QAChat.View.TagView;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel {
    /// <summary>
    /// クリップボードアイテム編集ウィンドウのViewModel
    /// </summary>
    public class EditItemWindowViewModel : ClipboardAppViewModelBase {

        public EditItemWindowViewModel(ClipboardFolderViewModel folderViewModel, ClipboardItemViewModel? itemViewModel, Action afterUpdate) {

            FolderViewModel = folderViewModel;
            if (itemViewModel == null) {
                ClipboardItem clipboardItem = new(folderViewModel.ClipboardItemFolder.Id) {
                    // ReferenceVectorDBItemsを設定
                    ReferenceVectorDBItems = folderViewModel.ClipboardItemFolder.ReferenceVectorDBItems
                };
                ItemViewModel = new ClipboardItemViewModel(folderViewModel, clipboardItem);
                Title = "新規アイテム";
            } else {
                Title = itemViewModel.ClipboardItem.Description;
                ItemViewModel = itemViewModel;
            }
            // ClipboardItemFileがある場合はSelectedFileに設定
            if (ItemViewModel.ClipboardItem.ClipboardItemFiles.Count > 0) {
                SelectedFile = (ClipboardItemFile)ItemViewModel.ClipboardItem.ClipboardItemFiles[0];
            }
            _afterUpdate = afterUpdate;

        }

        private ClipboardItemViewModel? itemViewModel;
        public ClipboardItemViewModel? ItemViewModel {
            get {
                return itemViewModel;
            }
            set {
                itemViewModel = value;
                TagsString = string.Join(",", itemViewModel?.Tags ?? []);

                OnPropertyChanged(nameof(ItemViewModel));
            }
        }
        private ClipboardFolderViewModel? _folderViewModel;
        public ClipboardFolderViewModel? FolderViewModel {
            get {
                return _folderViewModel;
            }
            set {
                _folderViewModel = value;
                OnPropertyChanged(nameof(FolderViewModel));
            }
        }

        public override void OnActivatedAction() {
            if (FolderViewModel == null) {
                return;
            }
            // StatusText.Readyにフォルダ名を設定
            StatusText statusText = Tools.StatusText;
            statusText.ReadyText = $"{StringResources.Folder}:[{FolderViewModel.FolderName}]";
        }

        private string title = "";
        public string Title {
            get {
                return title;
            }
            set {
                if (value == null) {
                    return;
                }
                title = value;
                OnPropertyChanged(nameof(Title));

            }
        }

        //Tagを文字列に変換したもの
        private string _tagsString = "";
        public string TagsString {
            get {
                return _tagsString;
            }
            set {
                _tagsString = value;
                OnPropertyChanged(nameof(TagsString));
            }
        }

        // 更新後の処理
        private Action _afterUpdate = () => { };

        // SelectedFile
        private ClipboardItemFile? selectedFile;
        public ClipboardItemFile? SelectedFile {
            get {
                return selectedFile;
            }
            set {
                selectedFile = value;
                OnPropertyChanged(nameof(SelectedFile));
                OnPropertyChanged(nameof(SelectedFileImageVisibility));
            }
        }
        // 選択したファイルの画像のVisible. SelectedFileがNull、Imageがnullの場合はCollapsed
        public Visibility SelectedFileImageVisibility {
            get {
                if (SelectedFile == null || SelectedFile.BitmapImage == null) {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
        }

        // SelectedImage
        private ImageSource? selectedImage;
        public ImageSource? SelectedImage {
            get {
                return selectedImage;
            }
            set {
                selectedImage = value;
                OnPropertyChanged(nameof(SelectedImage));
            }
        }
        public int SelectedImageIndex { get; set; } = 0;

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

        // TabItems 
        public ObservableCollection<TabItem> TabItems {
            get {
                if (ItemViewModel == null) {
                    return [];
                }
                ObservableCollection<TabItem> tabItems = [];
                // Content 
                ContentPanel contentPanel = new() {
                    DataContext = ItemViewModel,
                };
                TabItem contentTabItem = new() {
                    Header = StringResources.Text,
                    Content = contentPanel,
                    Height = Double.NaN,
                    Width = Double.NaN,
                    Margin = new Thickness(0, 0, 0, 0),
                    Padding = new Thickness(0, 0, 0, 0),
                    FontSize = 10,
                    Visibility = ItemViewModel?.TextTabVisibility ?? Visibility.Visible
                };
                tabItems.Add(contentTabItem);
                // FileOrImage
                FilePanel filePanel = new() {
                    DataContext = this,
                };
                TabItem fileTabItem = new() {
                    Header = StringResources.FileOrImage,
                    Content = filePanel,
                    Height = Double.NaN,
                    Width = Double.NaN,
                    Margin = new Thickness(0, 0, 0, 0),
                    Padding = new Thickness(0, 0, 0, 0),
                    FontSize = 10,
                    Visibility = ItemViewModel?.FileTabVisibility ?? Visibility.Collapsed
                };
                tabItems.Add(fileTabItem);

                // PromptResultのタブ
                foreach (TabItem promptTabItem in SystemPromptResultTabItems) {
                    tabItems.Add(promptTabItem);
                }
                // ClipboardItemのTypeがFileの場合はFileTabを選択
                if (ItemViewModel?.ClipboardItem.ContentType == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Files 
                    || ItemViewModel?.ClipboardItem.ContentType == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Image) {
                    SelectedTabIndex = 1;
                }
                return tabItems;
            }
        }
        // システム定義のPromptItemの結果表示用のタブを作成
        // TabItems 
        private ObservableCollection<TabItem> SystemPromptResultTabItems {
            get {
                if (ItemViewModel == null) {
                    return [];
                }
                ObservableCollection<TabItem> tabItems = [];
                // PromptResultのタブ
                List<string> promptNames = [
                    PromptItem.SystemDefinedPromptNames.BackgroundInformationGeneration.ToString(),
                    PromptItem.SystemDefinedPromptNames.TasksGeneration.ToString(),
                    PromptItem.SystemDefinedPromptNames.SummaryGeneration.ToString()
                    ];
                // PromptChatResultのエントリからPromptItemの名前を取得
                foreach ( string name in ItemViewModel.ClipboardItem.PromptChatResult.Results.Keys) {
                    if (promptNames.Contains(name) || PromptItem.SystemDefinedPromptNames.TitleGeneration.ToString().Equals(name)) {
                        continue;
                    }
                    promptNames.Add(name);
                }

                foreach (string promptName in promptNames) {
                    PromptResultViewModel promptViewModel = new(ItemViewModel.ClipboardItem.PromptChatResult, promptName);
                    PromptItem? item = PromptItem.GetPromptItemByName(promptName);
                    if (item == null) {
                        continue;
                    }

                    object content = item.PromptResultType switch {
                        PromptItem.PromptResultTypeEnum.TextContent => new PromptResultTextPanel() { DataContext = promptViewModel },
                        PromptItem.PromptResultTypeEnum.ComplexContent => new PromptResultComplexPanel() { DataContext = promptViewModel },
                        _ => ""
                    };
                    Visibility visibility = item.PromptResultType switch {
                        PromptItem.PromptResultTypeEnum.TextContent => promptViewModel.TextContentVisibility,
                        PromptItem.PromptResultTypeEnum.ComplexContent => promptViewModel.ComplexContentVisibility,
                        _ => Visibility.Collapsed
                    };

                    TabItem promptTabItem = new() {
                        Header = item.Description,
                        Content = content,
                        Height = Double.NaN,
                        Width = Double.NaN,
                        Margin = new Thickness(0, 0, 0, 0),
                        Padding = new Thickness(0, 0, 0, 0),
                        FontSize = 10,
                        Visibility = visibility
                    };
                    tabItems.Add(promptTabItem);
                }

                return tabItems;
            }

        }

        // コンテキストメニューの「テキストを抽出」の実行用コマンド
        public SimpleDelegateCommand<object> ExtractTextCommand => new((parameter) => {
            if (ItemViewModel == null) {
                LogWrapper.Error("クリップボードアイテムが選択されていません");
                return;
            }
            ClipboardAppCommandExecute.ExtractTextCommand(ItemViewModel.ClipboardItem);
            int index = SelectedTabIndex;
            OnPropertyChanged(nameof(TabItems));
            SelectedTabIndex = index;
            OnPropertyChanged(nameof(SelectedTabIndex));
        });

        // タグ追加ボタンのコマンド
        public SimpleDelegateCommand<object> AddTagButtonCommand => new((obj) => {

            if (ItemViewModel == null) {
                LogWrapper.Error("クリップボードアイテムが選択されていません");
                return;
            }
            TagWindow.OpenTagWindow(ItemViewModel.ClipboardItem, () => {
                // TagsStringを更新
                TagsString = string.Join(",", ItemViewModel.Tags);
            });
        });

        // QAChatButtonCommand
        public SimpleDelegateCommand<object> QAChatButtonCommand => new((obj) => {
            // QAChatControlのDrawerを開く
            ItemViewModel?.OpenOpenAIChatWindowCommand.Execute();
        });

        // Saveコマンド
        public SimpleDelegateCommand<object> SaveCommand => new((obj) => {
            // TitleとContentの更新を反映
            if (ItemViewModel == null) {
                return;
            }
            // フォルダに自動処理が設定されている場合は実行
            ClipboardFolder? folder = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolder(ItemViewModel.ClipboardItem.CollectionId);
            ClipboardItem? item = ItemViewModel.ClipboardItem.ApplyAutoProcess();
            // ClipboardItemを更新
            if (item != null) {
                item.Save();
            } else {
                // 自動処理に失敗した場合はLogWrapper.Info("自動処理に失敗しました");
                LogWrapper.Info("自動処理に失敗しました");
            }
            // 更新後の処理を実行
            _afterUpdate.Invoke();

        });
        // OKボタンのコマンド
        public SimpleDelegateCommand<Window> OKButtonCommand => new((window) => {

            // SaveCommandを実行
            SaveCommand.Execute(null);
            // ウィンドウを閉じる
            window.Close();
        });

    }
}
