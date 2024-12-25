using ClipboardApp.Factory;
using ClipboardApp.Item;
using LiteDB;
using NetOffice.OutlookApi;
using NetOffice.OutlookApi.Enums;
using PythonAILib.Common;
using PythonAILib.Model.Content;
using Outlook = NetOffice.OutlookApi;

namespace ClipboardApp.Model.Folder {
    public class OutlookFolder : ClipboardFolder {

        // コンストラクタ
        public OutlookFolder() {}
        protected OutlookFolder(OutlookFolder parent, string folderName) : base(parent, folderName) {
            FolderType = FolderTypeEnum.Outlook;
            // フォルダ名を設定
            FolderName = folderName;
            // FolderNameに一致するMAPIFolderがある場合は取得
            var mAPIFolder = parent.MAPIFolder.Folders.FirstOrDefault(x => x.Name == folderName);
            if (mAPIFolder != null) {
                MAPIFolder = mAPIFolder;
            }
        }

        private static Outlook.Application? outlookApplication = null;

        [BsonIgnore]
        public Outlook.MAPIFolder MAPIFolder { get; set; } = CreateInboxFolder();

        public static MAPIFolder CreateInboxFolder() {

            outlookApplication = new Outlook.Application();
            Outlook._NameSpace outlookNamespace = outlookApplication.GetNamespace("MAPI");
            MAPIFolder inboxFolder = outlookNamespace.GetDefaultFolder(OlDefaultFolders.olFolderInbox);
            return inboxFolder;
        }

        public static bool OutlookApplicationExists() {
            try {
                new Outlook.Application();
            }catch (System.Exception) {
                return false;
            }
            return true;
        }

        public override OutlookFolder CreateChild(string folderName) {
            OutlookFolder child = new(this, folderName);
            return child;
        }

        public override List<T> GetItems<T>() {
            // ローカルファイルシステムとClipboardFolderのファイルを同期
            SyncItems();

            var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemCollection<ContentItem>();
            // FileSystemFolderPathフォルダ内のファイルを取得
            List<ContentItem> items = [.. collection.FindAll().Where(x => x.CollectionId == Id).OrderByDescending(x => x.UpdatedAt)];

            return items.Cast<T>().ToList();
        }

        public void SyncItems() {
            // MAPIFolderが存在しない場合は終了
            if (MAPIFolder == null) {
                return;
            }
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetItemCollection<OutlookItem>();
            // FileSystemFolderPathフォルダ内のファイルを取得
            List<OutlookItem> items = [.. collection.Find(x => x.CollectionId == Id).OrderByDescending(x => x.UpdatedAt)];

            // Outlookのフォルダ内のファイル一覧を取得
            List<MailItem> mailItems = MAPIFolder.Items.Cast<MailItem>().ToList();
            foreach (OutlookItem item in items) {
                // EntryIDが一致するOutlookItemが存在しない場合は削除
                if (!mailItems.Any(x => x.EntryID == item.EntryID)) {
                    collection.Delete(item.Id);
                }
            }
            foreach (MailItem mailItem in mailItems) {
                // EntryIDが一致するOutlookItemが存在しない場合は追加
                if (!items.Any(x => x.EntryID == mailItem.EntryID)) {
                    OutlookItem newItem = new() {
                        EntryID = mailItem.EntryID,
                        Description = mailItem.Subject,
                        ContentType = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text,
                        Content = mailItem.Body,
                    };
                    newItem.Save();
                }
            }

        }

        // 子フォルダ
        public override List<OutlookFolder> GetChildren<OutlookFolder>() {
            // ローカルファイルシステムとClipboardFolderのフォルダを同期
            SyncFolders();
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<OutlookFolder>();
            var folders = collection.Find(x => x.ParentId == Id).OrderBy(x => x.FolderName);
            return folders.Cast<OutlookFolder>().ToList();

        }

        public virtual void SyncFolders() {

            // DBからParentIDが自分のIDのものを取得
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<OutlookFolder>();
            var folders = collection.Find(x => x.ParentId == Id).OrderBy(x => x.FolderName);
            // Outlook上のフォルダの一覧を取得。
            if (MAPIFolder == null) {
                return;
            } else {

                // MAPIFolder内のフォルダ一覧を取得
                List<string> outlookFolderNames = MAPIFolder.Folders.Select(x => x.Name).ToList();
                // DBに存在するフォルダがOutlookに存在しない場合は削除
                foreach (var folder in folders) {
                    if (!outlookFolderNames.Any(x => x == folder.FolderName)) {
                        collection.Delete(folder.Id);
                    }
                }
                // Outlookに存在するフォルダがDBに存在しない場合は追加
                foreach (var outlookFolderName in outlookFolderNames) {
                    if (!folders.Any(x => x.FolderName == outlookFolderName)) {
                        this.CreateChild(outlookFolderName);
                    }
                }
            }

            // 自分自身を保存
            this.Save<OutlookFolder, OutlookItem>();
        }

    }
}
