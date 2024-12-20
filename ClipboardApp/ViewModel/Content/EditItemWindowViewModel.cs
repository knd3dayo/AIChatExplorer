using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClipboardApp.ViewModel.Main;
using QAChat.View.Tag;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Content {
    /// <summary>
    /// クリップボードアイテム編集ウィンドウのViewModel
    /// </summary>
    public class EditItemWindowViewModel : ClipboardAppViewModelBase {

        public EditItemWindowViewModel(ClipboardFolderViewModel folderViewModel, ClipboardItemViewModel? itemViewModel, Action afterUpdate) {

            FolderViewModel = folderViewModel;
            if (itemViewModel == null) {
                Model.ClipboardItem clipboardItem = new(folderViewModel.ClipboardItemFolder.Id) {
                    // ReferenceVectorDBItemsを設定
                    ReferenceVectorDBItems = folderViewModel.ClipboardItemFolder.ReferenceVectorDBItems
                };
                ItemViewModel = new ClipboardItemViewModel(folderViewModel, clipboardItem);
                Title = StringResources.NewItem;

            } else {
                Title = itemViewModel.ClipboardItem.Description;
                ItemViewModel = itemViewModel;
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

        public Action CloseUserControl { get; set; } = () => { };



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


        // Saveコマンド
        public SimpleDelegateCommand<object> SaveCommand => new((obj) => {
            // TitleとContentの更新を反映
            if (ItemViewModel == null) {
                return;
            }
            // フォルダに自動処理が設定されている場合は実行
            Model.ClipboardItem? item = ItemViewModel.ClipboardItem.ApplyAutoProcess();
            // ClipboardItemを更新
            if (item != null) {

                Task.Run(() => {
                    // ベクトル化
                    item.UpdateEmbedding();
                    // 保存
                    item.Save();
                    MainUITask.Run(() => {
                        // 更新後の処理を実行
                        _afterUpdate.Invoke();
                    });
                });

            } else {
                // 自動処理に失敗した場合はLogWrapper.Info("自動処理に失敗しました");
                LogWrapper.Info("自動処理に失敗しました");
            }

        });
        // OKボタンのコマンド
        public SimpleDelegateCommand<object> OKButtonCommand => new((parameter) => {
            // SaveCommandを実行
            SaveCommand.Execute(null);

            // parameterがWindowの場合
            if (parameter is Window window) {
                // ウィンドウを閉じる
                window.Close();
            }// parameterがUserControlの場合
            if (parameter is UserControl userControl) {
                // UserControlを非表示にする
                CloseUserControl.Invoke();
            }
        });

        public override SimpleDelegateCommand<object> CloseCommand => new((parameter) => {
            // parameterがWindowの場合
            if (parameter is Window window) {
                // ウィンドウを閉じる
                window.Close();
            }
            // parameterがUserControlの場合
            if (parameter is UserControl userControl) {
                // UserControlを非表示にする
                CloseUserControl.Invoke();
            }
        });


    }
}
