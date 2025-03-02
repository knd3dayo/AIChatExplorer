using ClipboardApp.Model.Folders.Clipboard;
using ClipboardApp.Model.Item;
using ClipboardApp.ViewModel.Content;
using ClipboardApp.ViewModel.Folders.Clipboard;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Folder;
using LibUIPythonAI.View.Folder;
using LibUIPythonAI.ViewModel;
using LibUIPythonAI.ViewModel.Folder;
using PythonAILibUI.ViewModel.Item;

namespace ClipboardApp.ViewModel.Folders.Chat {
    public class ChatFolderViewModel(ContentFolderWrapper clipboardItemFolder, ContentItemViewModelCommands commands) : ClipboardFolderViewModel(clipboardItemFolder, commands) {

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override ClipboardFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder) {
            var chatFolderViewModel = new ChatFolderViewModel(childFolder, commands);
            // チャットフォルダの親フォルダにこのフォルダを追加
            chatFolderViewModel.ParentFolderViewModel = this;
            return chatFolderViewModel;
        }

        // アイテム作成コマンドの実装. 画像チェックの場合は、画像チェックー画面を開く
        public override void CreateItemCommandExecute() {
            ClipboardItem clipboardItem = new(Folder.Entity);
            ClipboardItemViewModel clipboardItemViewModel = new(this, clipboardItem);
            QAChatStartupProps props = new(clipboardItemViewModel.ContentItem);
            LibUIPythonAI.View.ChatMain.QAChatMainWindow.OpenOpenAIChatWindow(props);
        }

        public override void CreateFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            // 子フォルダを作成する
            // 自身が画像チェックの場合は、画像チェックを作成
            ClipboardFolder childFolder = (ClipboardFolder)Folder.CreateChild("");
            childFolder.Entity.FolderTypeString = FolderTypeEnum.Chat.ToString();
            ChatFolderViewModel childFolderViewModel = new(childFolder, commands);
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

