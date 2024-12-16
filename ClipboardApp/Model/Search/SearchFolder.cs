using ClipboardApp.Factory;
using ClipboardApp.Model.Folder;
using ClipboardApp.Utils;
using LiteDB;
using PythonAILib.Model.Content;
using WpfAppCommon.Utils;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace ClipboardApp.Model.Search {
    public partial class SearchFolder : ClipboardFolder {


        //--------------------------------------------------------------------------------
        // コンストラクタ
        public SearchFolder() { }

        protected SearchFolder(SearchFolder? parent, string folderName) : base(parent, folderName) {

            ParentId = parent?.Id ?? ObjectId.Empty;
            FolderName = folderName;
            // 親フォルダがnullの場合は、FolderTypeをNormalに設定
            FolderType = parent?.FolderType ?? FolderTypeEnum.Normal;
            // 親フォルダのAutoProcessEnabledを継承
            IsAutoProcessEnabled = parent?.IsAutoProcessEnabled ?? true;

        }

        // フォルダの絶対パス
        public override string FolderPath {
            get {
                ClipboardFolder? parent = (ClipboardFolder?)ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<ClipboardFolder>().FindById(ParentId);
                if (parent == null) {
                    return FolderName;
                }
                return Tools.ConcatenateFileSystemPath(parent.FolderPath, FolderName);
            }
        }

        // アイテム LiteDBには保存しない。
        [BsonIgnore]
        public override List<T> GetItems<T>() {
            return ClipboardFolderUtil.GetSearchFolderItems(this).Cast<T>().ToList();
        }

        // 子フォルダ BSonMapper.GlobalでIgnore設定しているので、LiteDBには保存されない
        public virtual void DeleteChild(ClipboardFolder child) {
            DeleteFolder<ClipboardFolder, ClipboardItem>(child);
        }

        public override ClipboardFolder CreateChild(string folderName) {
            SearchFolder child = new(this, folderName);
            return child;
        }

        // アイテムを追加する処理
        public override void AddItem(ContentItem item) {
            // 何もしない
        }

        // ClipboardItemを削除
        public virtual void DeleteItem(ClipboardItem item) {
            // 何もしない
        }

        #region システムのクリップボードへ貼り付けられたアイテムに関連する処理
        public override void ProcessClipboardItem(ClipboardChangedEventArgs e, Action<ClipboardItem> _afterClipboardChanged) {
            // 何もしない
        }
        #endregion
    }
}

