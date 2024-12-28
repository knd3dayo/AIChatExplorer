using System.IO;
using System.Windows;
using ClipboardApp.Model.AutoProcess;
using ClipboardApp.Model.Folder;
using Microsoft.WindowsAPICodePack.Dialogs;
using PythonAILib.Model.Search;
using QAChat.Resource;
using QAChat.ViewModel.Folder;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Folder {
    public abstract class ClipboardFolderBase(ClipboardFolder clipboardItemFolder) : ContentFolderViewModel(clipboardItemFolder) {


        protected virtual void UpdateStatusText() {
            string message = $"{StringResources.Folder}[{FolderName}]";

            if (Folder is ClipboardFolder clipboardFolder) {
                // AutoProcessRuleが設定されている場合
                var rules = AutoProcessRuleController.GetAutoProcessRules(clipboardFolder);
                if (rules.Count > 0) {
                    message += $" {StringResources.AutoProcessingIsSet}[";
                    foreach (AutoProcessRule item in rules) {
                        message += item.RuleName + " ";
                    }
                    message += "]";
                }

                // folderが検索フォルダの場合
                SearchRule? searchConditionRule = FolderManager.GlobalSearchCondition;
                if (clipboardFolder.FolderType == FolderTypeEnum.Search) {
                    searchConditionRule = SearchRuleController.GetSearchRuleByFolder(clipboardFolder);
                }
                SearchCondition? searchCondition = searchConditionRule?.SearchCondition;
                // SearchConditionがNullでなく、 Emptyでもない場合
                if (searchCondition != null && !searchCondition.IsEmpty()) {
                    message += $" {StringResources.SearchCondition}[";
                    message += searchCondition.ToStringSearchCondition();
                    message += "]";
                }
            }

            Tools.StatusText.ReadyText = message;
            Tools.StatusText.Text = message;
        }


        //-----コマンド
        //--------------------------------------------------------------------------------
        //--コマンド
        //--------------------------------------------------------------------------------


        // フォルダ内のアイテムをJSON形式でバックアップする処理
        public SimpleDelegateCommand<object> BackupItemsFromFolderCommand => new((parameter) => {
            if (Folder is not ClipboardFolder clipboardFolder) {
                return;
            }
            DirectoryInfo directoryInfo = new("export");
            // exportフォルダが存在しない場合は作成
            if (!Directory.Exists("export")) {
                directoryInfo = Directory.CreateDirectory("export");
            }
            string fileName = DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + this.Folder.Id.ToString() + ".json";

            //ファイルダイアログを表示
            using var dialog = new CommonOpenFileDialog() {
                Title = CommonStringResources.Instance.SelectFolderPlease,
                InitialDirectory = directoryInfo.FullName,
                // デフォルトのファイル名を設定
                DefaultFileName = fileName,
            };
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            if (dialog.ShowDialog(window) != CommonFileDialogResult.Ok) {
                return;
            } else {
                string resultFilePath = dialog.FileName;
                clipboardFolder.ExportItemsToJson(resultFilePath);
                // フォルダ内のアイテムを読み込む
                LogWrapper.Info(CommonStringResources.Instance.FolderExported);
            }
        });

        // フォルダ内のアイテムをJSON形式でリストアする処理
        public SimpleDelegateCommand<object> RestoreItemsToFolderCommand => new((parameter) => {
            if (Folder is not ClipboardFolder clipboardFolder) {
                return;
            }
            //ファイルダイアログを表示
            using var dialog = new CommonOpenFileDialog() {
                Title = CommonStringResources.Instance.SelectFolderPlease,
                InitialDirectory = @".",
            };
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            if (dialog.ShowDialog(window) != CommonFileDialogResult.Ok) {
                return;
            } else {
                string filaPath = dialog.FileName;
                // ファイルを読み込む
                string jsonString = File.ReadAllText(filaPath);
                clipboardFolder.ImportItemsFromJson(jsonString);
                // フォルダ内のアイテムを読み込む
                this.LoadFolderCommand.Execute();
                LogWrapper.Info(CommonStringResources.Instance.FolderImported);
            }
        });



    }
}
