using System.Threading.Tasks;
using AIChatExplorer.Model.Folders.Application;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Folders;
using LibPythonAI.Utils.Common;
using NetOffice.OutlookApi;
using NetOffice.OutlookApi.Enums;
using NetOfficeOutlook = NetOffice.OutlookApi;

namespace AIChatExplorer.Model.Folders.Outlook {
    public class OutlookFolder : ApplicationFolder {

        // コンストラクタ
        public OutlookFolder() : base() {
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
                ParentId = Id,
                FolderName = folderName,
                FolderTypeString = FolderTypeString
            };
            OutlookFolder child = new() { Entity = childFolder };
            return child;
        }

        public override async Task SyncItems() {
            // MAPIFolderが存在しない場合は終了
            if (MAPIFolder == null) {
                return;
            }

            // OutlookItemのEntryIDとIDのDictionary
            Dictionary<string, OutlookItem> entryIdIdDict = [];
            // GetItems(true)を実行すると無限ループになるため、GetItems(false)を使用
            foreach (var item in await GetItems<OutlookItem>(false)) {
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
                await outlookItem.Delete();
            }
            // Parallel処理
            Parallel.ForEach(MAPIFolder.Items.Cast<NetOfficeOutlook.MailItem>(), async outlookItem => {
                OutlookItem newItem = await OutlookItem.Create(Entity, outlookItem.EntryID);
                newItem.Description = outlookItem.Subject;
                newItem.ContentType = ContentItemTypes.ContentItemTypeEnum.Text;
                newItem.Content = outlookItem.Body;
                await newItem.Save();
            });
        }

        // 子フォルダ
        public override async Task<List<T>> GetChildren<T>(bool isSync = true) {
            if (isSync) {
                // SyncFolders
                await SyncFolders();
            }
            return await base.GetChildren<T>();
        }

        public async Task<MAPIFolder?> GetMAPIFolder() {
            if (IsRootFolder) {
                return null;
            }
            var parent = await GetParent<OutlookFolder>();
            if (parent?.IsRootFolder == true) {
                return InboxFolder;
            }
            // FolderPathを/で分割した要素のリスト
            var contentFolderPath = await GetContentFolderPath();
            List<string> strings = [.. contentFolderPath.Split('/')];
            MAPIFolder? mAPIFolder = InboxFolder;
            for (int i = 2; i < strings.Count; i++) {
                mAPIFolder = mAPIFolder?.Folders.FirstOrDefault(x => x.Name == strings[i]);
            }
            return mAPIFolder;
        }

        public virtual async Task SyncFolders() {


            // Outlook上のフォルダのNameのHashSet
            HashSet<string> outlookFolderNames = new();
            if (IsRootFolder) {
                // ルートフォルダの場合はInboxフォルダを取得
                outlookFolderNames.Add(InboxFolder.Name);
            } else {
                MAPIFolder = await GetMAPIFolder();
                if (MAPIFolder == null) {
                    return;
                }
                outlookFolderNames = MAPIFolder.Folders.Cast<MAPIFolder>().Select(x => x.Name).ToHashSet();
            }
            LogWrapper.Info($"Sync Outlook Folder: {InboxFolder.Name}");

            // GetItems(true)を実行すると無限ループになるため、GetItems(false)を使用
            var items = await base.GetItems<ContentItemWrapper>(false);
            // folder内のFolderNameとContentFolderのDictionary
            Dictionary<string, ContentItemWrapper> folderPathIdDict = [];
            foreach (var item in items) {
                if (item is ContentItemWrapper outlookFolder) {
                    folderPathIdDict[outlookFolder.FolderName] = outlookFolder;
                }
            }

            // DBに存在するフォルダがOutlookに存在しない場合は削除
            // Exceptで差集合を取得
            var deleteFolderNames = folderPathIdDict.Keys.Except(outlookFolderNames);
            foreach (var deleteFolderName in deleteFolderNames) {
                ContentItemWrapper deleteFolder = folderPathIdDict[deleteFolderName];
                await deleteFolder.Delete();
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
