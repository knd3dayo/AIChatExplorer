using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Item;
using ClipboardApp.ViewModel.Content;
using PythonAILib.Model.Folder;
using QAChat.ViewModel;
using QAChat.ViewModel.Folder;
using QAChat.ViewModel.Item;
using QAChat.View.Folder;

namespace ClipboardApp.ViewModel.Chat {
    public class ChatFolderViewModel(ClipboardFolder clipboardItemFolder) : ClipboardFolderViewModel(clipboardItemFolder) {

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override ClipboardFolderViewModel CreateChildFolderViewModel(ClipboardFolder childFolder) {
            var chatFolderViewModel = new ChatFolderViewModel(childFolder);
            // チャットフォルダの親フォルダにこのフォルダを追加
            chatFolderViewModel.ParentFolderViewModel = this;
            return chatFolderViewModel;
        }

        // アイテム作成コマンドの実装. 画像チェックの場合は、画像チェックー画面を開く
        public override void CreateItemCommandExecute() {
            ClipboardItem clipboardItem = new(Folder.Id);
            ClipboardItemViewModel clipboardItemViewModel = new(this, clipboardItem);
            OpenItemCommandExecute(clipboardItemViewModel);
        }
        public override void OpenItemCommandExecute(ContentItemViewModel itemViewModel) {
            QAChatStartupProps props = new(itemViewModel.ContentItem);

            QAChat.View.QAChatMain.QAChatMainWindow.OpenOpenAIChatWindow(props);
        }

        public override void CreateFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            // 子フォルダを作成する
            // 自身が画像チェックの場合は、画像チェックを作成
            ClipboardFolder childFolder = (ClipboardFolder)Folder.CreateChild("");
            childFolder.FolderType = FolderTypeEnum.Chat;
            ChatFolderViewModel childFolderViewModel = new(childFolder);
            // TODO チャット履歴作成画面を開くようにする。フォルダ名とRAGソースのリストを選択可能にする。
            FolderEditWindow.OpenFolderEditWindow(childFolderViewModel, afterUpdate);

        }
        /// <summary>
        ///  フォルダ編集コマンド
        ///  フォルダ編集ウィンドウを表示する処理
        ///  フォルダ編集後に実行するコマンドが設定されている場合は、実行する.
        /// </summary>
        /// <param name="parameter"></param>
        public override void EditFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            FolderEditWindow.OpenFolderEditWindow(folderViewModel, afterUpdate);
        }

    }
}

