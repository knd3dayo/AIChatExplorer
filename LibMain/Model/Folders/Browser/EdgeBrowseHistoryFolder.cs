using System.Data.SQLite;
using System.IO;
using AIChatExplorer.Model.Folders.Application;
using LibMain.Common;
using LibMain.Data;
using LibMain.Model.Content;
using LibMain.Model.Folders;
using LibMain.Utils.Common;

namespace AIChatExplorer.Model.Folders.Browser {
    public class EdgeBrowseHistoryFolder : ApplicationFolder {

        public static string OriginalHistoryFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Edge", "User Data", "Default", "History");
        public static string CopiedHistoryDirectoryPath = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetAppDataPath(), "edge");
        private const int FileCopyRetryCount = 3;
        private const int FileCopyRetryDelayMs = 1000;
        // コンストラクタ
        public EdgeBrowseHistoryFolder() : base() {
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
            ContentFolderEntity childFolder = new() {
                ParentId = Id,
                FolderName = folderName,
                FolderTypeString = FolderTypeString,
            };
            EdgeBrowseHistoryFolder child = new() { Entity = childFolder };
            return child;
        }

        // 子フォルダ
        public override async Task<List<T>> GetChildrenAsync<T>(bool isSync = true) {
            await Task.CompletedTask;
            return [];
        }

        public override async Task SyncItemsAsync() {
            // 1. EdgeのHistoryファイルが存在しない場合は何もしない
            if (!File.Exists(OriginalHistoryFilePath)) {
                return;
            }

            // 2. コピー先ディレクトリが存在しない場合は作成
            if (!Directory.Exists(CopiedHistoryDirectoryPath)) {
                Directory.CreateDirectory(CopiedHistoryDirectoryPath);
            }

            // 3. Historyファイルをコピー（リトライ処理あり）
            string copiedHistoryFilePath = Path.Combine(CopiedHistoryDirectoryPath, "History");
            for (int i = 0; i < FileCopyRetryCount; i++) {
                try {
                    File.Copy(OriginalHistoryFilePath, copiedHistoryFilePath, true);
                    break;
                } catch (IOException e) {
                    LogWrapper.Info($"IOException:{e.Message}");
                    await Task.Delay(FileCopyRetryDelayMs);
                } catch (UnauthorizedAccessException e) {
                    LogWrapper.Error($"UnauthorizedAccessException:{e.Message}");
                    return;
                } catch (Exception e) {
                    LogWrapper.Error($"Exception:{e.Message}");
                    return;
                }
            }

            // 4. DBから既存アイテムを取得（IsSync=falseで無限ループ防止）
            List<ContentItem> items = await GetItemsAsync<ContentItem>(false);

            // 5. 既存アイテムをURLでDictionary化
            Dictionary<string, ContentItem> itemUrlIdDict = [];
            foreach (var item in items) {
                itemUrlIdDict[item.SourcePath] = item;
            }

            // 6. EdgeのHistoryファイルから履歴情報を取得
            Dictionary<string, (string title, long lastVisitTime)> historyUrlDict = [];
            using (var connection = new SQLiteConnection($"Data Source={copiedHistoryFilePath};Version=3;New=False;Compress=True;")) {
                connection.Open();
                string query = "SELECT url, title, last_visit_time FROM urls ORDER BY last_visit_time ASC";
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        // 履歴情報をDictionaryに追加
                        string url = reader.GetString(0);
                        string title = reader.GetString(1);
                        long lastVisitTime = reader.GetInt64(2);
                        historyUrlDict[url] = (title, lastVisitTime);
                    }
                }
            }

            // 7. 新規追加分と更新分のURLリストを作成
            var addUrls = historyUrlDict.Keys.Except(itemUrlIdDict.Keys).ToList(); // DBに存在しない履歴
            var updateUrls = historyUrlDict.Keys.Intersect(itemUrlIdDict.Keys).ToList(); // DBに存在する履歴

            // 8. 新規追加分のContentItemリストを作成
            var addContentItems = addUrls.Select(url => {
                (string title, long lastVisitTime) = historyUrlDict[url];
                DateTime lastVisitTimeDateTime = ConvertLastVisitTimeToDateTime(lastVisitTime);
                return new ContentItem(this.Entity) {
                    Description = title,
                    SourcePath = url,
                    SourceType = ContentSourceType.Url,
                    UpdatedAt = lastVisitTimeDateTime,
                    CreatedAt = lastVisitTimeDateTime,
                };
            }).ToList();
            // 9. 新規追加分を一括保存
            await ContentItem.SaveItemsAsync(addContentItems);

            // 10. 更新が必要なContentItemリストを作成
            List<ContentItem?> updateContentItems = updateUrls.Select(url => {
                (string title, long lastVisitTime) = historyUrlDict[url];
                DateTime dateTime = ConvertLastVisitTimeToDateTime(lastVisitTime);
                ContentItem contentItem = itemUrlIdDict[url];
                if (contentItem.UpdatedAt < dateTime) {
                    contentItem.UpdatedAt = dateTime;
                    return contentItem;
                }
                return null;
            }).Where(x => x != null).ToList();
            // 11. 更新分を一括保存
            if (updateContentItems.Count > 0) {
                await ContentItem.SaveItemsAsync(updateContentItems);
            }

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
