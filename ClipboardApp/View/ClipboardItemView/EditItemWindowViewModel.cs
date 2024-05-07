using System.Windows;
using System.Windows.Controls;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.TagView;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.ClipboardItemView {
    /// <summary>
    /// クリップボードアイテム編集ウィンドウのViewModel
    /// </summary>
    class EditItemWindowViewModel : MyWindowViewModel {

        private TextSelector TextSelector = new();

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

        public void Initialize(ClipboardFolderViewModel folderViewModel, ClipboardItemViewModel? itemViewModel, Action afterUpdate) {
            if (itemViewModel == null) {
                ClipboardItem clipboardItem = new() {
                    // CollectionNameを設定
                    CollectionName = folderViewModel.AbsoluteCollectionName
                };

                ItemViewModel = new ClipboardItemViewModel(folderViewModel, clipboardItem);
                title = "新規アイテム";
            } else {
                title = "アイテム編集";
                ItemViewModel = itemViewModel;
            }
            _afterUpdate = afterUpdate;

        }


        // タグ追加ボタンのコマンド
        public SimpleDelegateCommand AddTagButtonCommand => new((obj) => {

            if (ItemViewModel == null) {
                Tools.Error("クリップボードアイテムが選択されていません");
                return;
            }
            TagWindow tagWindow = new();
            TagWindowViewModel tagWindowViewModel = (TagWindowViewModel)tagWindow.DataContext;
            tagWindowViewModel.Initialize(ItemViewModel, () => {
                // TagsStringを更新
                TagsString = string.Join(",", ItemViewModel.Tags);
            });

            tagWindow.ShowDialog();

        });
        // Ctrl + Aを一回をしたら行選択、二回をしたら全選択
        public SimpleDelegateCommand SelectTextCommand => new((parameter) => {

            if (parameter is not Window window) {
                return;
            }

            object? editorObject = window?.FindName("Editor");
            if (editorObject == null) {
                return;
            }

            TextBox editor = (TextBox)editorObject;
            // テキスト選択
            TextSelector.SelectText(editor);
            return;
        });
        // 選択中のテキストをプロセスとして実行
        public SimpleDelegateCommand ExecuteSelectedTextCommand => new((parameter) => {

            if (parameter is not Window window) {
                return;
            }

            object? editorObject = window?.FindName("Editor");
            if (editorObject == null) {
                return;
            }

            TextBox editor = (TextBox)editorObject;
            // 選択中のテキストをプロセスとして実行
            TextSelector.ExecuteSelectedText(editor);

        });


        // OKボタンのコマンド
        public SimpleDelegateCommand OKButtonCommand => new((parameter) => {

            // TitleとContentの更新を反映
            if (ItemViewModel == null) {
                return;
            }
            // ClipboardItemを更新
            ItemViewModel.Save();
            // 更新後の処理を実行
            _afterUpdate.Invoke();
            if (parameter is not Window window) {
                return;
            }
            // ウィンドウを閉じる
            window.Close();
        });

        // キャンセルボタンのコマンド
        public SimpleDelegateCommand CancelButtonCommand => new(CancelButtonCommandExecute);
        private void CancelButtonCommandExecute(object parameter) {
            if (parameter is not Window window) {
                return;
            }
            // ウィンドウを閉じる
            window.Close();
        }

    }
}
