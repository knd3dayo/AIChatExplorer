using ClipboardApp.Factory;
using ClipboardApp.Model.Item;
using LiteDB;
using NetOffice.OutlookApi;
using NetOffice.OutlookApi.Enums;
using PythonAILib.Common;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Model.Content;
using PythonAILib.Model.Folder;
using PythonAILib.Utils.Common;
using Outlook = NetOffice.OutlookApi;

namespace ClipboardApp.Model.Folder {
    public class OutlookFolder : ClipboardFolder {

        public override void Save() {
            Save<OutlookFolder, OutlookItem>();
        }
        // 削除
        public override void Delete() {
            DeleteFolder<OutlookFolder, OutlookItem>(this);
        }
        // 親フォルダ
        public override OutlookFolder? GetParent() {
            return GetParent<OutlookFolder>();
        }

        // コンストラクタ
        public OutlookFolder() { }
        protected OutlookFolder(OutlookFolder parent, string folderName) : base(parent, folderName) {
            FolderType = FolderTypeEnum.Outlook;
            // フォルダ名を設定
            FolderName = folderName;
            // FolderNameに一致するMAPIFolderがある場合は取得
            var mAPIFolder = parent.MAPIFolder?.Folders.FirstOrDefault(x => x.Name == folderName);
            if (mAPIFolder != null) {
                MAPIFolder = mAPIFolder;
            }
        }

        private static Outlook.Application? outlookApplication = null;

        [BsonIgnore]
        public Outlook.MAPIFolder? MAPIFolder { get; set; }

        public static MAPIFolder InboxFolder { get; private set; } = CreateInboxFolder();
        private static MAPIFolder CreateInboxFolder() {

            outlookApplication = new Outlook.Application();
            Outlook._NameSpace outlookNamespace = outlookApplication.GetNamespace("MAPI");
            MAPIFolder inboxFolder = outlookNamespace.GetDefaultFolder(OlDefaultFolders.olFolderInbox);
            return inboxFolder;
        }


        public static bool OutlookApplicationExists() {
            try {
                new Outlook.Application();
            } catch (System.Exception) {
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
            // SyncItems();

            var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemCollection<T>();
            // FileSystemFolderPathフォルダ内のファイルを取得
            List<T> items = [.. collection.Find(x => x.CollectionId == Id).OrderByDescending(x => x.UpdatedAt)];

            return items;
        }

        public void SyncItems() {
            // MAPIFolderが存在しない場合は終了
            if (MAPIFolder == null) {
                return;
            }
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetItemCollection<OutlookItem>();


            // EntryIDを格納するリスト
            List<string> entryIdList = [];
            // Outlookのフォルダ内のファイル一覧を取得
            foreach (var outlookItem in MAPIFolder.Items) {
                if (outlookItem is Outlook.MailItem mailItem) {
                    entryIdList.Add(mailItem.EntryID);

                    // EntryIDが一致するOutlookItemが存在しない場合は追加
                    var items = collection.Find(x => x.EntryID == mailItem.EntryID);
                    if (items == null || items.Count() == 0) {
                        OutlookItem newItem = new(this.Id, mailItem.EntryID) {
                            Description = mailItem.Subject,
                            ContentType = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text,
                            Content = mailItem.Body,
                        };
                        // 自動処理ルールを適用
                        Task<ContentItem> task = AutoProcessRuleController.ApplyGlobalAutoAction(newItem);
                        ContentItem result = task.Result;
                        result.Save();
                    }
                }
            }
            foreach (var item in collection.Find(x => x.CollectionId == this.Id)) {
                // EntryIDが一致するOutlookItemが存在しない場合は削除
                if (!entryIdList.Any(x => x == item.EntryID)) {
                    collection.Delete(item.Id);
                }
            }
        }

        // 子フォルダ
        public override List<T> GetChildren<T>() {
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<OutlookFolder>();
            IEnumerable<OutlookFolder> folders = collection.Find(x => x.ParentId == Id).OrderBy(x => x.FolderName);

            // ローカルファイルシステムとClipboardFolderのフォルダを同期
            SyncFolders(folders);
            return folders.Cast<T>().ToList();

        }

        public MAPIFolder? GetMAPIFolder() {
            if (IsRootFolder) {
                return null;
            }
            if (GetParent<OutlookFolder>()?.IsRootFolder == true) {
                return InboxFolder;
            }
            // FolderPathを/で分割した要素のリスト
            List<string> strings = [.. FolderPath.Split('/')];
            MAPIFolder? mAPIFolder = InboxFolder;
            for (int i = 2; i < strings.Count; i++) {
                mAPIFolder = mAPIFolder?.Folders.FirstOrDefault(x => x.Name == strings[i]);
            }
            return mAPIFolder;
        }

        public virtual void SyncFolders(IEnumerable<OutlookFolder> folders) {
            List<string> outlookFolderNames = [];
            // Outlook上のフォルダの一覧を取得。
            if (IsRootFolder) {
                // ルートフォルダの場合はInboxフォルダを取得
                outlookFolderNames.Add(InboxFolder.Name);
                LogWrapper.Info($"Sync Outlook Folder: {InboxFolder.Name}");

            } else {
                MAPIFolder = GetMAPIFolder();
                if (MAPIFolder == null) {
                    return;
                }
                outlookFolderNames = MAPIFolder.Folders.Select(x => x.Name).ToList();
                LogWrapper.Info($"Sync Outlook Folder: {MAPIFolder.Name}");
            }
            // MAPIFolder内のフォルダ一覧を取得
            // DBに存在するフォルダがOutlookに存在しない場合は削除
            foreach (var folder in folders) {
                if (!outlookFolderNames.Any(x => x == folder.FolderName)) {
                    folder.Delete<OutlookFolder, OutlookItem>();
                }
            }
            // Outlookに存在するフォルダがDBに存在しない場合は追加
            foreach (var outlookFolderName in outlookFolderNames) {
                if (!folders.Any(x => x.FolderName == outlookFolderName)) {
                    var child = this.CreateChild(outlookFolderName);
                    child.Save();
                }
            }

            // 自分自身を保存
            this.Save();
        }

    }
}
