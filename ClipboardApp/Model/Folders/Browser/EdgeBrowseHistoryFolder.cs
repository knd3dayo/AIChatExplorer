using System.Data.SQLite;
using System.IO;
using ClipboardApp.Model.Folders.Clipboard;
using ClipboardApp.Model.Main;
using ClipboardApp.ViewModel.Settings;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;

namespace ClipboardApp.Model.Folders.Browser {
    public class EdgeBrowseHistoryFolder : ClipboardFolder {

        public static string OriginalHistoryFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Edge", "User Data", "Default", "History");
        public static string CopiedHistoryFilePath = Path.Combine(ClipboardAppConfig.Instance.AppDataFolder, "edge");
        // コンストラクタ
        public EdgeBrowseHistoryFolder(ContentFolderEntity folder) : base(folder) {
            IsAutoProcessEnabled = false;
            FolderTypeString = FolderManager.RECENT_FILES_ROOT_FOLDER_NAME_EN;
        }

        protected EdgeBrowseHistoryFolder(EdgeBrowseHistoryFolder parent, string folderName) : base(parent, folderName) {
            // ルートフォルダの場合は FileSystemFolderPath = "" とする。それ以外は、親フォルダのFileSystemFolderPath + FolderName
            if (IsRootFolder) {
                FileSystemFolderPath = "";
            } else {
                string parentFileSystemFolderPath = parent.FileSystemFolderPath ?? "";
                FileSystemFolderPath = Path.Combine(parentFileSystemFolderPath, folderName);
            }
            FolderTypeString = FolderManager.RECENT_FILES_ROOT_FOLDER_NAME_EN;

        }

        public override EdgeBrowseHistoryFolder CreateChild(string folderName) {
            EdgeBrowseHistoryFolder child = new(this, folderName);
            return child;
        }

        public override EdgeBrowseHistoryFolder? GetParent() {
            var parentFolder = Entity.Parent;
            if (parentFolder == null) {
                return null;
            }
            return new EdgeBrowseHistoryFolder(parentFolder);
        }

        public override List<ContentItemWrapper> GetItems() {
            // SyncItems
            SyncItems();
            var items = Entity.Children;
            List<ContentItemWrapper> result = [];
            foreach (var item in items) {
                result.Add(new EdgeBrowseHistoryItem(item));
            }
            return result;
        }

        // 子フォルダ
        public override List<ContentFolderWrapper> GetChildren() {
            return []; ;
        }

        public void SyncItems() {
            // オリジナルのHistoryファイルが存在しない場合は何もしない
            if (!File.Exists(OriginalHistoryFilePath)) {
                return;
            }
            // オリジナルのHistoryファイルをコピー先ディレクトリにコピーする
            if (!Directory.Exists(CopiedHistoryFilePath)) {
                Directory.CreateDirectory(CopiedHistoryFilePath);
            }
            string copiedHistoryFilePath = Path.Combine(CopiedHistoryFilePath, "History");
            File.Copy(OriginalHistoryFilePath, copiedHistoryFilePath, true);

            // コレクション
            using PythonAILibDBContext db = new();
            var items = db.ContentItems.Where(x => x.FolderId == Entity.Id).Select(x => new ContentItemWrapper(x)).ToList();

            // Items内のSourcePathとContentItemのDictionary
            Dictionary<string, ContentItemWrapper> itemUrlIdDict = [];
            foreach (var item in items) {
                itemUrlIdDict[item.SourcePath] = item;
            }
            // HistoryのURLと(title, last_visit_time)のDictionary
            Dictionary<string, (string title, long lastVisitTime)> historyUrlDict = [];

            using (var connection = new SQLiteConnection($"Data Source={copiedHistoryFilePath};Version=3;New=False;Compress=True;")) {
                connection.Open();
                string query = "SELECT url, title, last_visit_time FROM urls ORDER BY last_visit_time ASC";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        // Dictionaryに追加
                        string url = reader.GetString(0);
                        string title = reader.GetString(1);
                        long lastVisitTime = reader.GetInt64(2);
                        historyUrlDict[url] = (title, lastVisitTime);
                    }
                }
            }


            //Historyに、ItemにないURLがある場合は追加
            // Exceptで差集合を取得
            var addUrls = historyUrlDict.Keys.Except(itemUrlIdDict.Keys);


            ParallelOptions parallelOptions = new() {
                MaxDegreeOfParallelism = 8
            };

            Parallel.ForEach(addUrls, parallelOptions, url => {
                // urlからTitle, LastVisitTimeを取得
                (string title, long lastVisitTime) = historyUrlDict[url];

                DateTime lastVisitTimeDateTime = ConvertLastVisitTimeToDateTime(lastVisitTime);
                ContentItemWrapper contentItem = new(this.Entity) {
                    Description = title,
                    SourcePath = url,
                    SourceType = ContentSourceType.Url,
                    UpdatedAt = lastVisitTimeDateTime,
                    CreatedAt = lastVisitTimeDateTime,

                };
                contentItem.Save(false, false);
                // 自動処理ルールを適用
                // Task<ContentItem> task = AutoProcessRuleController.ApplyGlobalAutoAction(item);
                // ContentItem result = task.Result;
                // result.Save();
            });

            //HistoryのURLとItemのURLの和集合
            var updateUrls = historyUrlDict.Keys.Intersect(itemUrlIdDict.Keys);

            Parallel.ForEach(updateUrls, parallelOptions, url => {
                // urlからTitle, LastVisitTimeを取得
                (string title, long lastVisitTime) = historyUrlDict[url];
                DateTime dateTime = ConvertLastVisitTimeToDateTime(lastVisitTime);
                ContentItemWrapper contentItem = itemUrlIdDict[url];

                if (contentItem.UpdatedAt < dateTime) {
                    contentItem.UpdatedAt = dateTime;
                    contentItem.Save(false, false);
                }
            });

        }

        public static DateTime ConvertLastVisitTimeToDateTime(long lastVisitTime) {

            // Chrome/Edgeのタイムスタンプは1601年1月1日を基準にしたマイクロ秒単位
            DateTime epoch = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // マイクロ秒から秒に変換してDateTime型に変換
            DateTime visitTime = epoch.AddTicks(lastVisitTime * 10); // 1 tick = 100ナノ秒 (1 マイクロ秒 = 10 ticks)

            return visitTime;
        }
    }
}
