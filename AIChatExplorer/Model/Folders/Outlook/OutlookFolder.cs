using AIChatExplorer.Model.Folders.Clipboard;
using AIChatExplorer.Model.Folders.FileSystem;
using AIChatExplorer.Model.Main;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
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
            OutlookFolder child = new() { Entity = childFolder};
            return child;
        }

        public override void SyncItems() {
            // MAPIFolderが存在しない場合は終了
            if (MAPIFolder == null) {
                return;
            }

            // OutlookItemのEntryIDとIDのDictionary
            Dictionary<string, OutlookItem> entryIdIdDict = [];
            // GetItems(true)を実行すると無限ループになるため、GetItems(false)を使用
            foreach (var item in GetItems< OutlookItem>(false)) {
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
                    ContentType = ContentItemTypes.ContentItemTypeEnum.Text,
                    Content = outlookItem.Body,
                };
                newItem.Save();
            });
        }

        // 子フォルダ
        public override List<T> GetChildren<T>() {

            // ローカルファイルシステムとApplicationFolderのフォルダを同期
            SyncFolders();
            return base.GetChildren<T>();
        }

        public MAPIFolder? GetMAPIFolder() {
            if (IsRootFolder) {
                return null;
            }
            if (GetParent<OutlookFolder>()?.IsRootFolder == true) {
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

            // GetItems(true)を実行すると無限ループになるため、GetItems(false)を使用
            var items = base.GetItems<ContentItemWrapper>(false);
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
