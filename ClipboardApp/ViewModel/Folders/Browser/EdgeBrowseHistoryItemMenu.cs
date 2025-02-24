using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.ViewModel.Content;
using ClipboardApp.ViewModel.Folders.Clipboard;
using ClipboardApp.ViewModel.Main;
using LibUIPythonAI.Resource;
using PythonAILib.Resources;

namespace ClipboardApp.ViewModel.Folders.Browser {
    public class EdgeBrowseHistoryItemMenu(ClipboardItemViewModel clipboardItemViewModel) : ClipboardItemMenu(clipboardItemViewModel) {
        public override ObservableCollection<MenuItem> CreateBasicItemContextMenuItems(ClipboardItemViewModel itemViewModel) {

            ObservableCollection<MenuItem> menuItems =
            [
                // 新規
                CreateMenuItem,
                // 開く
                OpenMenuItem,
                // Webページをダウンロード
                DownloadWebPageCommandMenuItem,
                // ピン留め
                PinnedMenuItem,
                // 削除
                DeleteMenuItem,
            ];

            // プロンプトメニュー
            MenuItem promptMenuItem = CreatePromptMenuItems(itemViewModel);
            menuItems.Add(promptMenuItem);

            // ベクトル生成
            menuItems.Add(GenerateVectorMenuItem);

            // ベクトル検索
            menuItems.Add(VectorSearchMenuItem);


            // マージチャット
            menuItems.Add(MergeChatMenuItem);

            return menuItems;
        }

        // DownloadWebPageCommandMenuItem

        public MenuItem DownloadWebPageCommandMenuItem {
            get {
                MenuItem downloadWebPageCommandMenuItem = new() {
                    Header = CommonStringResources.Instance.DownloadWebPage,
                    Command = AppCommands.DownloadWebPageCommand,
                    CommandParameter = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems,
                };
                return downloadWebPageCommandMenuItem;
            }
        }

    }
}
