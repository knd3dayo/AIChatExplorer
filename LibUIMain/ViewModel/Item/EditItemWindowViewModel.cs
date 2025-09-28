using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LibMain.Model.AutoProcess;
using LibMain.Model.Content;
using LibMain.Utils.Common;
using LibUIMain.Resource;
using LibUIMain.Utils;
using LibUIMain.View.Tag;
using LibUIMain.ViewModel.Common;
using LibUIMain.ViewModel.Folder;

namespace LibUIMain.ViewModel.Item {
    /// <summary>
    /// アイテム編集ウィンドウのViewModel
    /// </summary>
    public class EditItemWindowViewModel : CommonViewModelBase {

        public EditItemWindowViewModel(ContentFolderViewModel folderViewModel, ContentItemViewModel itemViewModel, Action afterUpdate) {

            this.itemViewModel = itemViewModel;
            FolderViewModel = folderViewModel;
            _afterUpdate = afterUpdate;
            TagsString = string.Join(",", itemViewModel?.Tags ?? []);

            OnPropertyChanged(nameof(ItemViewModel));
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(SourcePath));

            // StatusText.Readyにフォルダ名を設定
            StatusText statusText = StatusText.Instance;
            statusText.ReadyText = $"{CommonStringResources.Instance.Folder}:[{FolderViewModel.FolderName}]";

            CommonViewModelProperties.PropertyChanged += OnPropertyChanged;

        }
        private TabControl? MyTabControl { get; set; }

        public override void OnLoadedAction() {
            base.OnLoadedAction();
            if (MyTabControl == null) {
                MyTabControl = ThisUserControl?.FindName("MyTabControl") as TabControl;
                ItemViewModel.UpdateView(MyTabControl);
            }
        }

        private ContentItemViewModel itemViewModel;
        public ContentItemViewModel ItemViewModel {
            get {
                return itemViewModel;
            }
            set {
                itemViewModel = value;
                OnPropertyChanged(nameof(ItemViewModel));
            }
        }
        private ContentFolderViewModel? _folderViewModel;
        public ContentFolderViewModel? FolderViewModel {
            get {
                return _folderViewModel;
            }
            set {
                _folderViewModel = value;
                OnPropertyChanged(nameof(FolderViewModel));
            }
        }

        public string Title {
            get {
                return itemViewModel.ContentItem.Description;
            }
            set {
                itemViewModel.ContentItem.Description = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        // Path
        public string SourcePath {
            get {
                return itemViewModel.ContentItem.SourcePath;
            }
            set {
                itemViewModel.ContentItem.SourcePath = value;
                OnPropertyChanged(nameof(SourcePath));
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



        // MarkdownViewVisibility
        public Visibility MarkdownViewVisibility => LibUIMain.Utils.Tools.BoolToVisibility(CommonViewModelProperties.MarkdownView);



        // タグ追加ボタンのコマンド
        public SimpleDelegateCommand<object> AddTagButtonCommand => new((obj) => {

            if (ItemViewModel == null) {
                LogWrapper.Error("アイテムが選択されていません");
                return;
            }
            TagWindow.OpenTagWindow(ItemViewModel.ContentItem, () => {
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
            Task.Run(async () => {
                // 自動処理を実行
                ContentItem? item = await AutoProcessRuleController.ApplyGlobalAutoActionAsync(ItemViewModel.ContentItem);
                // ApplicationItemを更新
                if (item != null) {
                    // 保存
                    await item.SaveAsync();
                    MainUITask.Run(() => {
                        // 更新後の処理を実行
                        _afterUpdate.Invoke();
                    });
                } else {
                    // 自動処理に失敗した場合はLogWrapper.Info("自動処理に失敗しました");
                    LogWrapper.Info("自動処理に失敗しました");
                }

            });
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
            CommonViewModelProperties.PropertyChanged -= OnPropertyChanged;
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

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(CommonViewModelProperties.MarkdownView)) {
                ItemViewModel.UpdateView(MyTabControl);
            }
        }
    }
}
