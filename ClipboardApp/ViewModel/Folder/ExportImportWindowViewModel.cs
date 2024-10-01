using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Folder {
    public class ExportImportWindowViewModel(ClipboardFolderViewModel ClipboardolderViewModel, Action AfterUpdate) : ClipboardAppViewModelBase {


        public int SelectedIndex { get; set; }

        // テキストを選択したか否か
        public bool IsTextChecked { get; set; } = true;

        // タイトルを選択したか否か
        public bool IsTitleChecked { get; set; } = true;

        // 背景情報を選択したか否か
        public bool IsBackgroundChecked { get; set; } = true;

        // サマリーを選択したか否か
        public bool IsSummaryChecked { get; set; } = true;

        // 選択したファイル名
        public string SelectedFileName { get; set; } = "";

        // インポート時に自動処理を実行
        public bool IsAutoProcessEnabled { get; set; } = false;

        // プログレスインジケーター
        private bool _isIndeterminate = false;
        public bool IsIndeterminate {
            get => _isIndeterminate;
            set {
                _isIndeterminate = value;
                OnPropertyChanged(nameof(IsIndeterminate));
            }
        }

        public SimpleDelegateCommand<Window> OKCommand => new((window) => {

            IsIndeterminate = true;
            // 選択されたインデックスによって処理を分岐
            Task.Run(() => {
                switch (SelectedIndex) {
                    case 0:
                        // エクスポート処理
                        ClipboardolderViewModel.ClipboardItemFolder.ExportToExcel(SelectedFileName, IsTitleChecked, IsTextChecked, IsBackgroundChecked, IsSummaryChecked);
                        IsIndeterminate = false;
                        break;
                    case 1:
                        // インポート処理
                        IsIndeterminate = true;
                        ClipboardolderViewModel.ClipboardItemFolder.ImportFromExcel(SelectedFileName, IsAutoProcessEnabled, IsTitleChecked, IsTextChecked);
                        IsIndeterminate = false;
                        break;
                    default:
                        break;
                }
                MainUITask.Run(() => {
                    IsIndeterminate = false;
                    AfterUpdate();
                    window.Close();
                });
            });
        });

        public SimpleDelegateCommand<object> SelectExportFileCommand => new((obj) => {
            // SelectedFileNameが空の場合はデフォルトのファイル名を設定
            if (SelectedFileName == "") {
                SelectedFileName = DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + ClipboardolderViewModel.ClipboardItemFolder.Id.ToString() + ".xlsx";
                OnPropertyChanged(nameof(SelectedFileName));
            }

            //ファイルダイアログを表示


            using var dialog = new CommonOpenFileDialog() {
                Title = CommonStringResources.Instance.SelectFilePlease,
                DefaultFileName = SelectedFileName,
                InitialDirectory = @".",
            };
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            if (dialog.ShowDialog(window) != CommonFileDialogResult.Ok) {
                return;
            } else {
                SelectedFileName = dialog.FileName;
                OnPropertyChanged(nameof(SelectedFileName));
            }
        });

        public SimpleDelegateCommand<object> SelectImportFileCommand => new((obj) => {
            //ファイルダイアログを表示
            using var dialog = new CommonOpenFileDialog() {
                Title = CommonStringResources.Instance.SelectFilePlease,
                InitialDirectory = @".",
                DefaultExtension = ".xlsx",
            };
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            if (dialog.ShowDialog(window) != CommonFileDialogResult.Ok) {
                return;
            } else {
                SelectedFileName = dialog.FileName;
                OnPropertyChanged(nameof(SelectedFileName));
            }
        });

    }
}
