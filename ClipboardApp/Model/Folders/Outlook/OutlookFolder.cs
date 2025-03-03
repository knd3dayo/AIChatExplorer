using ClipboardApp.Model.Folders.Clipboard;
using ClipboardApp.Model.Folders.FileSystem;
using ClipboardApp.Model.Main;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Common;
using NetOffice.OutlookApi;
using NetOffice.OutlookApi.Enums;
using NetOfficeOutlook = NetOffice.OutlookApi;

namespace ClipboardApp.Model.Folders.Outlook {
    public class OutlookFolder : ClipboardFolder {

        // コンストラクタ
        public OutlookFolder(ContentFolderEntity folder) : base(folder) {
            IsAutoProcessEnabled = false;
            FolderTypeString = FolderManager.OUTLOOK_ROOT_FOLDER_NAME_EN;
        }

        protected OutlookFolder(OutlookFolder parent, string folderName) : base(parent, folderName) {
            FolderTypeString = FolderManager.OUTLOOK_ROOT_FOLDER_NAME_EN;
            // フォルダ名を設定
            FolderName = folderName;
            // FolderNameに一致するMAPIFolderがある場合は取得
            var mAPIFolder = parent.MAPIFolder?.Folders.FirstOrDefault(x => x.Name == folderName);
            if (mAPIFolder != null) {
                MAPIFolder = mAPIFolder;
            }
        }


        public override OutlookFolder? GetParent() {
            using PythonAILibDBContext db = new();
            var parentFolder = db.ContentFolders.FirstOrDefault(x => x.Id == Entity.ParentId);
            if (parentFolder == null) {
                return null;
            }
            return new OutlookFolder(parentFolder);
        }


        private static NetOfficeOutlook.Application? outlookApplication = null;

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
            ContentFolderEntity childFolder = new() {
                ParentId = Entity.Id,
                FolderName = folderName,
            };
            OutlookFolder child = new(childFolder);
            return child;
        }

        public override List<ContentItemWrapper> GetItems() {
            using PythonAILibDBContext db = new();
            var items = db.ContentItems.Where(x => x.FolderId == Entity.Id).OrderBy(x => x.Description);
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

            // OutlookItemのEntryIDとIDのDictionary
            Dictionary<string, OutlookItem> entryIdIdDict = [];
            foreach (var item in GetItems()) {
                if (item is OutlookItem outlookItem) {
                    entryIdIdDict[outlookItem.EntryID] = outlookItem;
                }
            }
            // Outlookのフォルダ内のEntryIDを格納するHashSet
            HashSet<string> entryIdList = MAPIFolder.Items.Cast<NetOfficeOutlook.MailItem>().Select(x => x.EntryID).ToHashSet();

            // EntryIDが一致するOutlookItemが存在しない場合は削除
            // Exceptで差集合を取得
            foreach (var entryId in entryIdIdDict.Keys.Except(entryIdList)) {
                OutlookItem outlookItem = entryIdIdDict[entryId];
                outlookItem.Delete();
            }
            // Parallel処理
            Parallel.ForEach(MAPIFolder.Items.Cast<NetOfficeOutlook.MailItem>(), outlookItem => {
                OutlookItem newItem = new(Entity, outlookItem.EntryID) {
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
            using PythonAILibDBContext db = new();
            var folders = db.ContentFolders.Where(x => x.ParentId == Entity.Id).OrderBy(x => x.FolderName);

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


            // folder内のFolderNameとContentFolderのDictionary
            Dictionary<string, OutlookFolder> folderPathIdDict = [];
            foreach (var folder in GetChildren()) {
                if (folder is OutlookFolder outlookFolder) {
                    folderPathIdDict[outlookFolder.FolderName] = outlookFolder;
                }
            }

            // DBに存在するフォルダがOutlookに存在しない場合は削除
            // Exceptで差集合を取得
            var deleteFolderNames = folderPathIdDict.Keys.Except(outlookFolderNames);
            foreach (var deleteFolderName in deleteFolderNames) {
                OutlookFolder deleteFolder = folderPathIdDict[deleteFolderName];
                deleteFolder.Delete();
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
