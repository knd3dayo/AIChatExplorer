using ClipboardApp.Model.Folders.Clipboard;
using LiteDB;
using NetOffice.OutlookApi;
using NetOffice.OutlookApi.Enums;
using PythonAILib.Common;
using PythonAILib.Model.Content;
using PythonAILib.Model.Folder;
using PythonAILib.Utils.Common;
using NetOfficeOutlook = NetOffice.OutlookApi;

namespace ClipboardApp.Model.Folders.Outlook {
    public class OutlookFolder : ClipboardFolder {

        // コンストラクタ
        public OutlookFolder(ContentFolder folder) : base(folder) {
            IsAutoProcessEnabled = false;
            FolderType = FolderTypeEnum.Outlook;
        }

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


        public override OutlookFolder? GetParent() {
            var parentFolder = ContentFolderInstance.GetParent();
            if (parentFolder == null) {
                return null;
            }
            return new OutlookFolder(parentFolder);
        }


        private static NetOfficeOutlook.Application? outlookApplication = null;

        [BsonIgnore]
        public NetOfficeOutlook.MAPIFolder? MAPIFolder { get; set; }

        public static MAPIFolder InboxFolder { get; private set; } = CreateInboxFolder();
        private static MAPIFolder CreateInboxFolder() {

            outlookApplication = new NetOfficeOutlook.Application();
            NetOfficeOutlook._NameSpace outlookNamespace = outlookApplication.GetNamespace("MAPI");
            MAPIFolder inboxFolder = outlookNamespace.GetDefaultFolder(OlDefaultFolders.olFolderInbox);
            return inboxFolder;
        }


        public static bool OutlookApplicationExists() {
            try {
                new NetOfficeOutlook.Application();
            } catch (System.Exception) {
                return false;
            }
            return true;
        }

        public override OutlookFolder CreateChild(string folderName) {
            OutlookFolder child = new(this, folderName);
            return child;
        }

        public override List<ContentItemWrapper> GetItems() {
            var items = ContentFolderInstance.GetItems<ContentItem>();
            List<ContentItemWrapper> result = [];
            foreach (var item in items) {
                result.Add(new OutlookItem(item));
            }
            return result;
        }


        public void SyncItems() {
            // MAPIFolderが存在しない場合は終了
            if (MAPIFolder == null) {
                return;
            }
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetItemCollection<ContentItem>();

            // OutlookItemのEntryIDとIDのDictionary
            Dictionary<string, ObjectId> entryIdIdDict = [];
            foreach (var item in GetItems()) {
                if (item is OutlookItem outlookItem) {
                    entryIdIdDict[outlookItem.EntryID] = outlookItem.Id;
                }
            }
            // Outlookのフォルダ内のEntryIDを格納するHashSet
            HashSet<string> entryIdList = MAPIFolder.Items.Cast<NetOfficeOutlook.MailItem>().Select(x => x.EntryID).ToHashSet();

            // EntryIDが一致するOutlookItemが存在しない場合は削除
            // Exceptで差集合を取得
            foreach (var entryId in entryIdIdDict.Keys.Except(entryIdList)) {
                collection.Delete(entryIdIdDict[entryId]);
            }
            // Parallel処理
            Parallel.ForEach(MAPIFolder.Items.Cast<NetOfficeOutlook.MailItem>(), outlookItem => {
                OutlookItem newItem = new(Id, outlookItem.EntryID) {
                    Description = outlookItem.Subject,
                    ContentType = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text,
                    Content = outlookItem.Body,
                };
                newItem.Save();
            });
        }

        // 子フォルダ
        public override List<ContentFolderWrapper> GetChildren() {

            // ローカルファイルシステムとClipboardFolderのフォルダを同期
            SyncFolders();
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
            IEnumerable<ContentFolder> folders = collection.Find(x => x.ParentId == Id).OrderBy(x => x.FolderName);
            List<ContentFolderWrapper> result = [];
            foreach (var folder in folders) {
                OutlookFolder outlookFolder = new(folder);
                result.Add(outlookFolder);
            }
            return result;
        }

        public MAPIFolder? GetMAPIFolder() {
            if (IsRootFolder) {
                return null;
            }
            if (GetParent()?.IsRootFolder == true) {
                return InboxFolder;
            }
            // FolderPathを/で分割した要素のリスト
            List<string> strings = [.. ContentFolderPath.Split('/')];
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


            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
            // folder内のFolderNameとContentFolderのDictionary
            Dictionary<string, ContentFolder> folderPathIdDict = [];
            foreach (var folder in GetChildren()) {
                if (folder is OutlookFolder outlookFolder) {
                    folderPathIdDict[outlookFolder.FolderName] = outlookFolder.ContentFolderInstance;
                }
            }

            // DBに存在するフォルダがOutlookに存在しない場合は削除
            // Exceptで差集合を取得
            var deleteFolderNames = folderPathIdDict.Keys.Except(outlookFolderNames);
            foreach (var deleteFolderName in deleteFolderNames) {
                ContentFolder contentFolder = folderPathIdDict[deleteFolderName];
                contentFolder.Delete();
            }

            // Outlookに存在するフォルダがDBに存在しない場合は追加
            // Exceptで差集合を取得
            var addFolderNames = outlookFolderNames.Except(folderPathIdDict.Keys);

            // Parallel処理
            Parallel.ForEach(addFolderNames, outlookFolderName => {
                var child = CreateChild(outlookFolderName);
                child.Save();
            });

            // 自分自身を保存
            Save();
        }

    }
}
