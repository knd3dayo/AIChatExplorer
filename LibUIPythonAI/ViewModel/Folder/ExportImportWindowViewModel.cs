using System.Collections.ObjectModel;
using System.Windows;
using LibPythonAI.Utils.ExportImport;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Folder;
using Microsoft.WindowsAPICodePack.Dialogs;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Model.Content;
using PythonAILib.Model.Folder;
using PythonAILib.Model.Prompt;

namespace LibUIPythonAI.ViewModel.Folder {
    public class ExportImportWindowViewModel(ContentFolderViewModel ClipboardFolderViewModel, Action AfterUpdate) : ChatViewModelBase {

        // ImportItems
        public ObservableCollection<ExportImportItem> ImportItems { get; set; } = CreateImportItems();

        // ExportItems
        public ObservableCollection<ExportImportItem> ExportItems { get; set; } = CreateExportItems();

        private static ObservableCollection<ExportImportItem> CreateImportItems() {
            return [
                new ExportImportItem("Title", CommonStringResources.Instance.Title, true, false),
                new ExportImportItem("Text", CommonStringResources.Instance.Text, true, false),
                new ExportImportItem("SourcePath", CommonStringResources.Instance.SourcePath, false, false),
            ];
        }

        private static ObservableCollection<ExportImportItem> CreateExportItems() {
            // PromptItemの設定 出力タイプがテキストコンテンツのものを取得
            List<PromptItem> promptItems = PromptItem.GetPromptItems().Where(item => item.PromptResultType == PromptResultTypeEnum.TextContent).ToList();

            ObservableCollection<ExportImportItem> items = [
                new ExportImportItem("Title", CommonStringResources.Instance.Title, true, false),
                new ExportImportItem("Text", CommonStringResources.Instance.Text, true, false),
                new ExportImportItem("SourcePath", CommonStringResources.Instance.SourcePath, false, false),
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
        public ContentFolderWrapper? ExportTargetFolder { get; set; }

        // 選択したクリップボードフォルダのパス
        public string SelectedClipboardFolderPath {
            get {
                return ExportTargetFolder?.ContentFolderPath ?? "";
            }
        }

        // 選択したファイル名
        public string SelectedFileName { get; set; } = "";

        // インポート時に自動処理を実行
        public bool IsAutoProcessEnabled { get; set; } = false;

        // FileSelectionButtonVisibility
        public Visibility FileSelectionButtonVisibility => Tools.BoolToVisibility(SelectedIndex == 0 || SelectedIndex == 1 || SelectedIndex == 2);

        // ClipboardFolderSelectionButtonVisibility
        public Visibility ClipboardFolderSelectionButtonVisibility => Tools.BoolToVisibility(SelectedIndex == 2);

        public SimpleDelegateCommand<Window> OKCommand => new((window) => {

            UpdateIndeterminate(true);
            // 選択されたインデックスによって処理を分岐
            Task.Run(() => {
                // Excelインポート処理 ★TODO 自動処理の実装
                Action<ContentItemWrapper> afterImport = (item) => { };
                if (IsAutoProcessEnabled) {
                    afterImport = (item) => {
                        AutoProcessRuleController.ApplyGlobalAutoAction(item).Result.Save();
                    };
                }

                switch (SelectedIndex) {
                    case 0:
                        // Excelエクスポート処理
                        ImportExportUtil.ExportToExcel(ClipboardFolderViewModel.Folder, SelectedFileName, [.. ExportItems]);
                        break;
                    case 1:
                        // Excelインポート処理 ★TODO 自動処理の実装
                        ImportExportUtil.ImportFromExcel(ClipboardFolderViewModel.Folder, SelectedFileName, [.. ImportItems], afterImport);
                        break;
                    case 2:
                        // URLリストインポート処理
                        ImportExportUtil.ImportFromURLList(ClipboardFolderViewModel.Folder, SelectedFileName, afterImport);
                        break;
                    default:
                        break;
                }
            }).ContinueWith((task) => {
                UpdateIndeterminate(false);
                AfterUpdate();
                MainUITask.Run(() => {
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
