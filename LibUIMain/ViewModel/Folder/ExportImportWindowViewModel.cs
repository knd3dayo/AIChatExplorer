using System.Collections.ObjectModel;
using System.Windows;
using LibMain.Model.AutoProcess;
using LibMain.Model.Content;
using LibMain.Model.Prompt;
using LibMain.Utils.ExportImport;
using LibUIMain.Resource;
using LibUIMain.Utils;
using LibUIMain.View.Folder;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace LibUIMain.ViewModel.Folder {
    public class ExportImportWindowViewModel : CommonViewModelBase {

        public ExportImportWindowViewModel(ContentFolderViewModel applicationFolderViewModel, Action afterUpdate) {
            ApplicationFolderViewModel = applicationFolderViewModel;
            AfterUpdate = afterUpdate;
            ExportItems = CreateExportItems();
            OnPropertyChanged(nameof(ExportItems));
            ImportItems = CreateImportItems();
            OnPropertyChanged(nameof(ImportItems));
            Task.Run(async() => {
                SelectedApplicationFolderPath = await ApplicationFolderViewModel.Folder.GetContentFolderPath();
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

        private ObservableCollection<ContentItemDataDefinition> CreateImportItems() {
            return [.. ContentItemDataDefinition.CreateDefaultDataDefinitions()];
        }

        private static ObservableCollection<ContentItemDataDefinition> CreateExportItems() {
            // PromptItemの設定 出力タイプがテキストコンテンツのものを取得
            List<PromptItem> promptItems = PromptItem.GetPromptItems();
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


        public enum ImportExportMode {
            ExportExcel = 0,
            ImportExcel = 1,
            ImportUrlList = 2,
        }

        private ImportExportMode _selectedIndex = ImportExportMode.ExportExcel;
        public ImportExportMode SelectedIndex {
            get => _selectedIndex;
            set {
                if (_selectedIndex != value) {
                    _selectedIndex = value;
                    OnPropertyChanged(nameof(CanExecuteOK));
                }
            }
        }

        // 
        public ContentFolderWrapper? ExportTargetFolder { get; set; }

        // 選択したフォルダのパス
        public string SelectedApplicationFolderPath { get; private set; } = String.Empty;

        // 選択したファイル名
        private string _selectedFileName = "";
        public string SelectedFileName {
            get => _selectedFileName;
            set {
                if (_selectedFileName != value) {
                    _selectedFileName = value;
                    OnPropertyChanged(nameof(SelectedFileName));
                    OnPropertyChanged(nameof(CanExecuteOK));
                }
            }
        }

        // インポート時に自動処理を実行
        public bool IsAutoProcessEnabled { get; set; } = false;

        // FileSelectionButtonVisibility

        // OKボタンの有効/無効判定
        public bool CanExecuteOK => !string.IsNullOrWhiteSpace(SelectedFileName);

        public SimpleDelegateCommand<Window> OKCommand => new(async (window) => {
            CommonViewModelProperties.UpdateIndeterminate(true);
            try {
                // Excelインポート処理 ★TODO 自動処理の実装
                Action<ContentItem> afterImport = (item) => { };
                if (IsAutoProcessEnabled) {
                    afterImport = async (item) => {
                        var item1 = await AutoProcessRuleController.ApplyGlobalAutoActionAsync(item);
                        await item1.SaveAsync();
                    };
                }

                switch (SelectedIndex) {
                    case ImportExportMode.ExportExcel:
                        // Excelエクスポート処理
                        await ImportExportUtil.ExportToExcel(ApplicationFolderViewModel.Folder, SelectedFileName, [.. ExportItems]);
                        break;
                    case ImportExportMode.ImportExcel:
                        // Excelインポート処理
                        List<ContentItem> resultItems = await ImportExportUtil.ImportFromExcel(ApplicationFolderViewModel.Folder, SelectedFileName, [.. ImportItems]);
                        foreach (var item in resultItems) {
                            if (IsAutoProcessEnabled) {
                                var item1 = await AutoProcessRuleController.ApplyGlobalAutoActionAsync(item);
                                await item1.SaveAsync();
                            }
                        }

                        break;
                    case ImportExportMode.ImportUrlList:
                        // URLリストインポート処理
                        resultItems = await ImportExportUtil.ImportFromURLListAsync(ApplicationFolderViewModel.Folder, SelectedFileName);
                        foreach (var item in resultItems) {
                            if (IsAutoProcessEnabled) {
                                var item1 = await AutoProcessRuleController.ApplyGlobalAutoActionAsync(item);
                                await item1.SaveAsync();
                            }
                        }
                        break;
                    default:
                        break;
                }
            } catch (Exception ex) {
                // エラー通知やログ出力をここで行う
                MessageBox.Show($"エラーが発生しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            } finally {
                CommonViewModelProperties.UpdateIndeterminate(false);
                AfterUpdate();
                MainUITask.Run(() => {
                    window.Close();
                });
            }
        });


        // ファイル選択ダイアログの共通化
        private bool TrySelectFile(out string fileName, string title, string defaultExt = ".xlsx", string? defaultFileName = null) {
            using var dialog = new CommonOpenFileDialog() {
                Title = title,
                InitialDirectory = @".",
                DefaultExtension = defaultExt,
                DefaultFileName = defaultFileName ?? SelectedFileName,
            };
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            if (dialog.ShowDialog(window) == CommonFileDialogResult.Ok) {
                fileName = dialog.FileName;
                return true;
            }
            fileName = "";
            return false;
        }

        public SimpleDelegateCommand<object> SelectExportFileCommand => new((obj) => {
            if (string.IsNullOrEmpty(SelectedFileName)) {
                SelectedFileName = DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + ApplicationFolderViewModel.Folder.Id.ToString() + ".xlsx";
            }
            if (TrySelectFile(out var fileName, CommonStringResources.Instance.SelectFilePlease, ".xlsx", SelectedFileName)) {
                SelectedFileName = fileName;
            }
        });

        public SimpleDelegateCommand<object> SelectImportFileCommand => new((obj) => {
            if (TrySelectFile(out var fileName, CommonStringResources.Instance.SelectFilePlease, ".xlsx")) {
                SelectedFileName = fileName;
            }
        });

        // OpenSelectTargetFolderWindowCommand
        public SimpleDelegateCommand<object> OpenApplicationFolderWindowCommand => new((parameter) => {
            FolderSelectWindow.OpenFolderSelectWindow(FolderViewModelManagerBase.FolderViewModels, async (folderViewModel, finished) => {
                if (finished) {
                    ExportTargetFolder = folderViewModel.Folder;
                    SelectedApplicationFolderPath = await ExportTargetFolder.GetContentFolderPath() ?? "";
                    OnPropertyChanged(nameof(SelectedApplicationFolderPath));
                }
            });
        });
    }
}
