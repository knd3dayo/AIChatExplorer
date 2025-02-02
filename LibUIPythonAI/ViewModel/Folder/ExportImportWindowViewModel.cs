using System.Collections.ObjectModel;
using System.Windows;
using LibUIPythonAI.ViewModel;
using Microsoft.WindowsAPICodePack.Dialogs;
using PythonAILib.Model.Content;
using PythonAILib.Model.Folder;
using PythonAILib.Model.Prompt;
using PythonAILibUI.ViewModel.Folder;
using QAChat.Resource;
using QAChat.View.Folder;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.Folder {
    public class ExportImportWindowViewModel(ContentFolderViewModel ClipboardFolderViewModel, Action AfterUpdate) : ChatViewModelBase {

        // ImportItems
        public ObservableCollection<ExportImportItem> ImportItems { get; set; } = CreateImportItems();

        // ExportItems
        public ObservableCollection<ExportImportItem> ExportItems { get; set; } = CreateExportItems();

        private static ObservableCollection<ExportImportItem> CreateImportItems() {
            return [
                new ExportImportItem("Title", CommonStringResources.Instance.Title, true, false),
                new ExportImportItem("Text", CommonStringResources.Instance.Text, true, false),
            ];
        }

        private static ObservableCollection<ExportImportItem> CreateExportItems() {
            // PromptItemの設定 出力タイプがテキストコンテンツのものを取得
            List<PromptItem> promptItems = PromptItem.GetPromptItems().Where(item => item.PromptResultType == PromptResultTypeEnum.TextContent).ToList();

            ObservableCollection<ExportImportItem> items = [
                new ExportImportItem("Title", CommonStringResources.Instance.Title, true, false),
                new ExportImportItem("Text", CommonStringResources.Instance.Text, true, false),
            ];
            foreach (PromptItem promptItem in promptItems) {
                items.Add(new ExportImportItem(promptItem.Name, promptItem.Description, false, true));
            }
            return items;
        }


        private int _selectedIndex = 0;
        public int SelectedIndex {
            get {
                return _selectedIndex;
            }
            set {
                _selectedIndex = value;
                OnPropertyChanged(nameof(FileSelectionButtonVisibility));
                OnPropertyChanged(nameof(ClipboardFolderSelectionButtonVisibility));
            }
        }

        // 
        public ContentFolder? ExportTargetFolder { get; set; }

        // 選択したクリップボードフォルダのパス
        public string SelectedClipboardFolderPath {
            get { 
                return ExportTargetFolder?.FolderPath ?? "";
            }
        }

        // 選択したファイル名
        public string SelectedFileName { get; set; } = "";

        // インポート時に自動処理を実行
        public bool IsAutoProcessEnabled { get; set; } = false;

        // FileSelectionButtonVisibility
        public Visibility FileSelectionButtonVisibility => Tools.BoolToVisibility(SelectedIndex == 0 || SelectedIndex == 2 || SelectedIndex == 3);

        // ClipboardFolderSelectionButtonVisibility
        public Visibility ClipboardFolderSelectionButtonVisibility => Tools.BoolToVisibility(SelectedIndex == 1);

        public SimpleDelegateCommand<Window> OKCommand => new((window) => {

            IsIndeterminate = true;
            // 選択されたインデックスによって処理を分岐
            Task.Run(() => {
                switch (SelectedIndex) {
                    case 0:
                        // Excelエクスポート処理
                        ClipboardFolderViewModel.Folder.ExportToExcel(SelectedFileName, [.. ExportItems]);
                        break;
                    case 1:
                        // 新規アイテムとしてエクスポート処理
                        // ClipboardFolderViewModel.Folder.ExportToExcel(SelectedFileName, [.. ExportItems]);
                        break;
                    case 2:
                        // Excelインポート処理
                        ClipboardFolderViewModel.Folder.ImportFromExcel(SelectedFileName, [.. ImportItems], IsAutoProcessEnabled);
                        break;
                    case 3:
                        // URLリストインポート処理
                        ClipboardFolderViewModel.Folder.ImportFromURLList(SelectedFileName, IsAutoProcessEnabled);
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
                SelectedFileName = DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + ClipboardFolderViewModel.Folder.Id.ToString() + ".xlsx";
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

        // OpenSelectTargetFolderWindowCommand
        public SimpleDelegateCommand<object> OpenClipboardFolderWindowCommand => new((parameter) => {
            FolderSelectWindow.OpenFolderSelectWindow(RootFolderViewModelContainer.FolderViewModels, (folderViewModel, finished) => {
                if (finished) {
                    ExportTargetFolder = folderViewModel.Folder;
                    OnPropertyChanged(nameof(SelectedClipboardFolderPath));
                }
            });
        });
    }
}
