using System.Windows;
using System.Windows.Controls;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.SearchView;
using ClipboardApp.View.TagView;
using QAChat.View.PromptTemplateWindow;
using WpfAppCommon;
using WpfAppCommon.Control.QAChat;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.ClipboardItemView {
    /// <summary>
    /// クリップボードアイテム編集ウィンドウのViewModel
    /// </summary>
    class EditItemWindowViewModel : MyWindowViewModel {

        private readonly TextSelector TextSelector = new();

        private ClipboardItemViewModel? itemViewModel;
        public ClipboardItemViewModel? ItemViewModel {
            get {
                return itemViewModel;
            }
            set {
                itemViewModel = value;
                TagsString = string.Join(",", itemViewModel?.Tags ?? []);

                OnPropertyChanged(nameof(ItemViewModel));
                OnPropertyChanged(nameof(ImageTabVisibility));
                OnPropertyChanged(nameof(FileTabVisibility));
            }
        }

        private string title = "";
        public string Title {
            get {
                return title;
            }
            set {
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

        // イメージタブの表示可否
        public Visibility ImageTabVisibility {
            get {
                return ItemViewModel?.ContentType == ClipboardContentTypes.Image ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        // ファイルタブの表示可否
        public Visibility FileTabVisibility {
            get {
                return ItemViewModel?.ContentType == ClipboardContentTypes.Files ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // 更新後の処理
        private Action _afterUpdate = () => {};

        // QAChatControlのViewModel
        public QAChatControlViewModel QAChatControlViewModel { get; set; } = new();


        public void Initialize(ClipboardFolderViewModel folderViewModel, ClipboardItemViewModel? itemViewModel, Action afterUpdate) {
            if (itemViewModel == null) {
                ClipboardItem clipboardItem = new(folderViewModel.CollectionName, folderViewModel.FolderPath);


                ItemViewModel = new ClipboardItemViewModel(clipboardItem);
                title = "新規アイテム";
            } else {
                title = "アイテム編集";
                ItemViewModel = itemViewModel;
            }
            // QAChatControlの初期化
            QAChatControlViewModel.Initialize(ItemViewModel.ClipboardItem, PromptTemplateCommandExecute);
            SearchRule rule = ClipboardFolder.GlobalSearchCondition.Copy();

            QAChatControlViewModel.ShowSearchWindowAction = () => {
                SearchWindow.OpenSearchWindow(rule, null, () => {
                    // QAChatのContextを更新
                    List<ClipboardItem> clipboardItems = rule.SearchItems();
                    string contextText = ClipboardItem.GetContentsString(clipboardItems);
                    QAChatControlViewModel.ContextText = contextText;

                });
            };
            QAChatControlViewModel.SetContentTextFromClipboardItemsAction = () => {
                List<ClipboardItem> items = [];
                var clipboardItemViews = MainWindowViewModel.ActiveInstance?.SelectedFolder?.Items;
                if (clipboardItemViews != null) {
                    foreach (var item in clipboardItemViews) {
                        items.Add(item.ClipboardItem);
                    }
                }
                string contextText = ClipboardItem.GetContentsString(items);
                QAChatControlViewModel.ContextText = contextText;
            };

            _afterUpdate = afterUpdate;

        }


        // タグ追加ボタンのコマンド
        public SimpleDelegateCommand<object> AddTagButtonCommand => new((obj) => {

            if (ItemViewModel == null) {
                Tools.Error("クリップボードアイテムが選択されていません");
                return;
            }
            TagWindow.OpenTagWindow(ItemViewModel, () => {
                // TagsStringを更新
                TagsString = string.Join(",", ItemViewModel.Tags);
            });


        });
        // Ctrl + Aを一回をしたら行選択、二回をしたら全選択
        public SimpleDelegateCommand<TextBox> SelectTextCommand => new((textBox) => {

            // テキスト選択
            TextSelector.SelectText(textBox);
            return;
        });
        // 選択中のテキストをプロセスとして実行
        public SimpleDelegateCommand<TextBox> ExecuteSelectedTextCommand => new((textbox) => {

            // 選択中のテキストをプロセスとして実行
            TextSelector.ExecuteSelectedText(textbox);

        });

        // OKボタンのコマンド
        public SimpleDelegateCommand<Window> OKButtonCommand => new((window) => {

            // TitleとContentの更新を反映
            if (ItemViewModel == null) {
                return;
            }
            // フォルダに自動処理が設定されている場合は実行
            ClipboardFolder folder = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolder(ItemViewModel.ClipboardItem.CollectionName);
            ClipboardItem? item = folder.ApplyAutoProcess(ItemViewModel.ClipboardItem);
            // ClipboardItemを更新
            if (item != null) {
                item.Save();
            } else {
                // 自動処理に失敗した場合はTools.Info("自動処理に失敗しました");
                Tools.Info("自動処理に失敗しました");
            }
            // 更新後の処理を実行
            _afterUpdate.Invoke();
            // ウィンドウを閉じる
            window.Close();
        });

        // キャンセルボタンのコマンド
        public SimpleDelegateCommand<Window> CancelButtonCommand => new((window) => {
            // ウィンドウを閉じる
            window.Close();
        });

        // プロンプトテンプレートを開くコマンド
        private void PromptTemplateCommandExecute(object parameter) {
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Select, (promptTemplateWindowViewModel, Mode) => {
                QAChatControlViewModel.PromptText = promptTemplateWindowViewModel.PromptItem.Prompt;

            });
        }

    }
}
