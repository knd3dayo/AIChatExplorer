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
            // OutlookItemのEntryIDとIDのDictionary
            Dictionary<string, LiteDB.ObjectId> entryIdIdDict = collection.Find(x => x.CollectionId == this.Id).ToDictionary(x => x.EntryID, x => x.Id);

            // Outlookのフォルダ内のEntryIDを格納するHashSet
            HashSet<string> entryIdList = MAPIFolder.Items.Cast<Outlook.MailItem>().Select(x => x.EntryID).ToHashSet();

            // EntryIDが一致するOutlookItemが存在しない場合は削除
            // Exceptで差集合を取得
            foreach (var entryId in entryIdIdDict.Keys.Except(entryIdList)) {
                collection.Delete(entryIdIdDict[entryId]);
            }
            // Parallel処理
            Parallel.ForEach(MAPIFolder.Items.Cast<Outlook.MailItem>(), outlookItem => {
                OutlookItem newItem = new(this.Id, outlookItem.EntryID) {
                    Description = outlookItem.Subject,
                    ContentType = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text,
                    Content = outlookItem.Body,
                };
                newItem.Save();
            });
        }

        // 子フォルダ
        public override List<OutlookFolder> GetChildren<OutlookFolder>() {

            // ローカルファイルシステムとClipboardFolderのフォルダを同期
            SyncFolders();
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<OutlookFolder>();
            IEnumerable<OutlookFolder> folders = collection.Find(x => x.ParentId == Id).OrderBy(x => x.FolderName);
            return folders.Cast<OutlookFolder>().ToList();
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

        public virtual void SyncFolders() {

            // Outlook上のフォルダのNameのHashSet
            HashSet<string> outlookFolderNames = new();


            if (IsRootFolder) {
                // ルートフォルダの場合はInboxフォルダを取得
                outlookFolderNames.Add(InboxFolder.Name);
            } else {
                MAPIFolder = GetMAPIFolder();
                if (MAPIFolder == null) {
                    return;
                }
                outlookFolderNames = MAPIFolder.Folders.Cast<MAPIFolder>().Select(x => x.Name).ToHashSet();
            }
            LogWrapper.Info($"Sync Outlook Folder: {InboxFolder.Name}");

            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<OutlookFolder>();

            // folder内のFolderNameとIdのDictionary
            Dictionary<string, LiteDB.ObjectId> folderPathIdDict = collection.Find(x => x.ParentId == Id).ToDictionary(x => x.FolderName, x => x.Id);

            // DBに存在するフォルダがOutlookに存在しない場合は削除
            // Exceptで差集合を取得
            var deleteFolderNames = folderPathIdDict.Keys.Except(outlookFolderNames);
            foreach (var deleteFolderName in deleteFolderNames) {
                ObjectId deleteId = folderPathIdDict[deleteFolderName];
                collection.Delete(deleteId);
            }

            // Outlookに存在するフォルダがDBに存在しない場合は追加
            // Exceptで差集合を取得
            var addFolderNames = outlookFolderNames.Except(folderPathIdDict.Keys);

            // Parallel処理
            Parallel.ForEach(addFolderNames, outlookFolderName => {
                var child = this.CreateChild(outlookFolderName);
                child.Save();
            });

            // 自分自身を保存
            this.Save();
        }

    }
}
