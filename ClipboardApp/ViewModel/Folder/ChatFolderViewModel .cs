using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.Model;
using ClipboardApp.Model.Folder;
using ClipboardApp.View.ClipboardItemFolderView;
using QAChat.Control;
using ClipboardApp.ViewModel.ClipboardItemView;

namespace ClipboardApp.ViewModel.Folder {
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
            ClipboardItem clipboardItem = new(ClipboardItemFolder.Id);
            ClipboardItemViewModel clipboardItemViewModel = new(this, clipboardItem);
            OpenItemCommandExecute(clipboardItemViewModel);
        }
        public override void OpenItemCommandExecute(ClipboardItemViewModel itemViewModel) {
            QAChatStartupProps props = new(itemViewModel.ClipboardItem);
            QAChat.View.QAChatMain.QAChatMainWindow.OpenOpenAIChatWindow(props);
        }

        public override void CreateFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {
            // 子フォルダを作成する
            // 自身が画像チェックの場合は、画像チェックを作成
            ClipboardFolder childFolder = ClipboardItemFolder.CreateChild("");
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
        public override void EditFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {
            FolderEditWindow.OpenFolderEditWindow(folderViewModel, afterUpdate);
        }

    }
}

