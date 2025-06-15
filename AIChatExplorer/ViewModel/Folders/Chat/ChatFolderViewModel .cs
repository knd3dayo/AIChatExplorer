using AIChatExplorer.Model.Folders.Clipboard;
using AIChatExplorer.Model.Item;
using AIChatExplorer.Model.Main;
using AIChatExplorer.ViewModel.Content;
using AIChatExplorer.ViewModel.Folders.Application;
using LibPythonAI.Model.Content;
using LibUIPythonAI.View.Folder;
using LibUIPythonAI.ViewModel.Chat;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;

namespace AIChatExplorer.ViewModel.Folders.Chat {
    public class ChatFolderViewModel(ContentFolderWrapper applicationItemFolder, ContentItemViewModelCommands commands) : ApplicationFolderViewModel(applicationItemFolder, commands) {

        // 子フォルダのApplicationFolderViewModelを作成するメソッド
        public override ApplicationFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder) {
            var chatFolderViewModel = new ChatFolderViewModel(childFolder, commands) {
                // チャットフォルダの親フォルダにこのフォルダを追加
                ParentFolderViewModel = this
            };
            return chatFolderViewModel;
        }

        // アイテム作成コマンドの実装. 画像チェックの場合は、画像チェックー画面を開く
        public override void CreateItemCommandExecute() {
            ApplicationItem applicationItem = new(Folder.Entity);
            ApplicationItemViewModel applicationItemViewModel = new(this, applicationItem);
            QAChatStartupProps props = new(applicationItemViewModel.ContentItem);
            LibUIPythonAI.View.Chat.ChatWindow.OpenOpenAIChatWindow(props);
        }

        public override void CreateFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            // 子フォルダを作成する
            // 自身が画像チェックの場合は、画像チェックを作成
            ApplicationFolder childFolder = (ApplicationFolder)Folder.CreateChild("");
            childFolder.Entity.FolderTypeString = FolderManager.CHAT_ROOT_FOLDER_NAME_EN;
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

        // LoadLLMConfigListAsync
        public override void LoadItems() {
            LoadItems<ApplicationItem>();
        }

        // LoadChildren
        public override void LoadChildren(int nestLevel) {
            LoadChildren<ChatFolderViewModel, ApplicationFolder>(nestLevel);
        }
    }
}

