using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using LibPythonAI.Model.AutoProcess;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Utils.ExportImport;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Folder;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace LibUIPythonAI.ViewModel.Folder {
    public class ExportImportWindowViewModel : CommonViewModelBase {

        public ExportImportWindowViewModel(ContentFolderViewModel applicationFolderViewModel, Action afterUpdate) {
            ApplicationFolderViewModel = applicationFolderViewModel;
            AfterUpdate = afterUpdate;
            Task.Run(async () => {
                ExportItems = await CreateExportItems();
                OnPropertyChanged(nameof(ExportItems));
                ImportItems = await CreateImportItems();
                OnPropertyChanged(nameof(ImportItems));

            });
        }
        // ApplicationFolderViewModel
        public ContentFolderViewModel ApplicationFolderViewModel { get; set; }
        // AfterUpdate
        public Action AfterUpdate { get; set; }


        // ImportItems
        public ObservableCollection<ContentItemDataDefinition> ImportItems { get; set; }

        // ExportItems
        public ObservableCollection<ContentItemDataDefinition> ExportItems { get; set; }

        private static async Task<ObservableCollection<ContentItemDataDefinition>> CreateImportItems() {
            return [.. ContentItemDataDefinition.CreateDefaultDataDefinitions()];
        }

        private static async Task<ObservableCollection<ContentItemDataDefinition>> CreateExportItems() {
            // PromptItemの設定 出力タイプがテキストコンテンツのものを取得
            List<PromptItem> promptItems = await PromptItem.GetPromptItems();
            promptItems = promptItems.Where(item => item.PromptResultType == PromptResultTypeEnum.TextContent).ToList();

            ObservableCollection<ContentItemDataDefinition> items = [
                new ContentItemDataDefinition("Title", CommonStringResources.Instance.Title, true, false),
                new ContentItemDataDefinition("Text", CommonStringResources.Instance.Text, true, false),
                new ContentItemDataDefinition("SourcePath", CommonStringResources.Instance.SourcePath, false, false),
            ];
            foreach (PromptItem promptItem in promptItems) {
                items.Add(new ContentItemDataDefinition(promptItem.Name, promptItem.Description, false, true));
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
                OnPropertyChanged(nameof(ApplicationFolderSelectionButtonVisibility));
            }
        }

        // 
        public ContentFolderWrapper? ExportTargetFolder { get; set; }

        // 選択したフォルダのパス
        public string SelectedApplicationFolderPath {
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

        // ApplicationFolderSelectionButtonVisibility
        public Visibility ApplicationFolderSelectionButtonVisibility => Tools.BoolToVisibility(false);

        public SimpleDelegateCommand<Window> OKCommand => new((window) => {

            CommonViewModelProperties.UpdateIndeterminate(true);
            // 選択されたインデックスによって処理を分岐
            Task.Run(() => {
                // Excelインポート処理 ★TODO 自動処理の実装
                Action<ContentItemWrapper> afterImport = (item) => { };
                if (IsAutoProcessEnabled) {
                    afterImport = (item) => {
                        AutoProcessRuleController.ApplyGlobalAutoActionAsync(item).Result.Save();
                    };
                }

                switch (SelectedIndex) {
                    case 0:
                        // Excelエクスポート処理
                        ImportExportUtil.ExportToExcel(ApplicationFolderViewModel.Folder, SelectedFileName, [.. ExportItems]);
                        break;
                    case 1:
                        // Excelインポート処理 ★TODO 自動処理の実装
                        ImportExportUtil.ImportFromExcel(ApplicationFolderViewModel.Folder, SelectedFileName, [.. ImportItems], afterImport);
                        break;
                    case 2:
                        // URLリストインポート処理
                        ImportExportUtil.ImportFromURLList(ApplicationFolderViewModel.Folder, SelectedFileName, afterImport);
                        break;
                    default:
                        break;
                }
            }).ContinueWith((task) => {
                CommonViewModelProperties.UpdateIndeterminate(false);
                AfterUpdate();
                MainUITask.Run(() => {
                    window.Close();
                });
            });
        });

        public SimpleDelegateCommand<object> SelectExportFileCommand => new((obj) => {
            // SelectedFileNameが空の場合はデフォルトのファイル名を設定
            if (SelectedFileName == "") {
                SelectedFileName = DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + ApplicationFolderViewModel.Folder.Id.ToString() + ".xlsx";
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
        public SimpleDelegateCommand<object> OpenApplicationFolderWindowCommand => new((parameter) => {
            FolderSelectWindow.OpenFolderSelectWindow(FolderViewModelManagerBase.FolderViewModels, (folderViewModel, finished) => {
                if (finished) {
                    ExportTargetFolder = folderViewModel.Folder;
                    OnPropertyChanged(nameof(SelectedApplicationFolderPath));
                }
            });
        });
    }
}
