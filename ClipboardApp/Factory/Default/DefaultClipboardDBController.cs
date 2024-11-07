using ClipboardApp.Model;
using ClipboardApp.Model.AutoProcess;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Search;
using LiteDB;
using PythonAILib.Model.Abstract;
using PythonAILib.Model.Content;

namespace ClipboardApp.Factory.Default {
    public class DefaultClipboardDBController : PythonAILibDataFactory, IClipboardDBController {
        public const string CLIPBOARD_FOLDERS_COLLECTION_NAME = "folders";
        public const string CLIPBOARD_ROOT_FOLDERS_COLLECTION_NAME = "root_folders";
        public const string AUTO_PROCESS_RULES_COLLECTION_NAME = "auto_process_rules";
        public const string SEARCH_CONDITION_RULES_COLLECTION_NAME = "search_condition_rules";
        public const string SEARCH_CONDITION_APPLIED_CONDITION_NAME = "applied_globally";

        // --- ClipboardItem ----------------------------------------------
        // ClipboardItemを取得する。
        public override ContentItem? GetItem(ContentItem item) {
            if (item is not ClipboardItem clipboardItem) {
                throw new Exception("item is not ClipboardItem");
            }
            var collection = GetDatabase().GetCollection<ClipboardItem>(CONTENT_ITEM_COLLECTION_NAME);
            var result = collection.FindById(clipboardItem.Id);
            return result;
        }

        // ClipboardItemをLiteDBに追加または更新する
        public override void UpsertItem(ContentItem item, bool updateModifiedTime = true) {

            if (item is not ClipboardItem clipboardItem) {
                throw new Exception("item is not ClipboardItem");
            }
            // 更新日時を設定
            if (updateModifiedTime) {
                item.UpdatedAt = DateTime.Now;
            }
            var collection = GetDatabase().GetCollection<ClipboardItem>(CONTENT_ITEM_COLLECTION_NAME);
            collection.Upsert(clipboardItem);
        }

        // アイテムをDBから削除する
        public override void DeleteItem(ContentItem item) {
            if (item is not ClipboardItem clipboardItem) {
                throw new Exception("item is not ClipboardItem");
            }
            if (item.Id == null) {
                return;
            }
            var collection = GetDatabase().GetCollection<ClipboardItem>(CONTENT_ITEM_COLLECTION_NAME);
            // System.Windows.MessageBox.Show(item.CollectionName);
            collection.Delete(clipboardItem.Id);
        }


        #region フォルダー関連
        //---- フォルダー関連 ----------------------------------------------

        // ClipboardItemFolderをLiteDBに追加または更新する
        public override void UpsertFolder(ContentFolder contentFolder) {
            if (contentFolder is not ClipboardFolder clipboardFolder) {
                throw new Exception("folder is not ClipboardFolder");
            }
            var collection = GetDatabase().GetCollection<ClipboardFolder>(CLIPBOARD_FOLDERS_COLLECTION_NAME);
            // フォルダの親フォルダのIdをチェック
            if (clipboardFolder.ParentId == null || clipboardFolder.ParentId == ObjectId.Empty) {
                // 親フォルダのIDが存在しない場合は、ルートフォルダか否かをチェックする。GetRootFolderを呼び出す
                var rootFolder = GetRootFolderByType(clipboardFolder.FolderType);
                // ルートフォルダが存在しない場合は、新規作成
                if (rootFolder == null) {
                    var rootFolderInfoCollection = GetDatabase().GetCollection<RootFolderInfo>(CLIPBOARD_ROOT_FOLDERS_COLLECTION_NAME);
                    var rootFolderInfo = new RootFolderInfo {
                        FolderName = clipboardFolder.FolderName,
                        Id = ObjectId.NewObjectId(),
                        FolderType = clipboardFolder.FolderType
                    };

                    // ルートフォルダの作成
                    clipboardFolder.Id = rootFolderInfo.Id; ;
                    collection.Upsert(clipboardFolder);

                    // ルートフォルダ情報の作成
                    rootFolderInfoCollection.Upsert(rootFolderInfo);


                } else {
                    //　ルートフォルダの更新。folderにRootFolder.FolderIdを設定してUpsert 
                    clipboardFolder.Id = rootFolder.Id;
                    collection.Upsert(clipboardFolder);
                }

            } else {
                // 親フォルダが存在する場合は、ParentIdとFolderNameが位置するフォルダを取得する
                var childFolders = GetChildFolders(clipboardFolder.ParentId);
                var targetFolder = childFolders.FirstOrDefault(x => x.FolderName == clipboardFolder.FolderName);
                // 存在しない場合は、新規作成
                if (targetFolder == null) {
                    collection.Upsert(clipboardFolder);
                } else {
                    // 存在する場合は、folderのIdを設定してUpsert
                    clipboardFolder.Id = targetFolder.Id;
                    collection.Upsert(clipboardFolder);
                }
            }

        }
        // フォルダーを削除する
        public override void DeleteFolder(ContentFolder contentFolder) {
            if (contentFolder is not ClipboardFolder clipboardFolder) {
                throw new Exception("folder is not ClipboardFolder");
            }
            // folderの子フォルダを再帰的に削除
            foreach (var child in clipboardFolder.Children) {
                if (child != null) {
                    DeleteFolder(child);
                }
            }

            // Itemsを削除
            var collection = GetDatabase().GetCollection<ClipboardItem>(CONTENT_ITEM_COLLECTION_NAME);
            collection.FindAll().Where(x => x.CollectionId == clipboardFolder.Id).ToList().ForEach(x => DeleteItem(x));
            // folderを削除
            var folderCollection = GetDatabase().GetCollection<ClipboardFolder>(CLIPBOARD_FOLDERS_COLLECTION_NAME);
            folderCollection.Delete(clipboardFolder.Id);

        }

        public override ClipboardFolder? GetFolder(ObjectId? objectId) {
            if (objectId == null) return null;
            ClipboardFolder? result = null;
            var collection = GetDatabase().GetCollection<ClipboardFolder>(CLIPBOARD_FOLDERS_COLLECTION_NAME);
            var item = collection.FindById(objectId);
            if (item != null) {
                result = item;
            }
            return result;
        }

        public ClipboardFolder? GetRootFolderByType(FolderTypeEnum folderType) {
            var collection = GetDatabase().GetCollection<RootFolderInfo>(CLIPBOARD_ROOT_FOLDERS_COLLECTION_NAME);
            // Debug FolderName = folderNameのアイテムが複数ある時はエラー
            var items = collection.FindAll().Where(x => x.FolderType == folderType);
            if (items.Count() > 1) {
                throw new Exception("RootFolderInfoに同じFolderNameが複数存在します。");
            }
            // itemsの最初の要素を取得
            var item = items.FirstOrDefault();
            return GetFolder(item?.Id);
        }

        public List<ClipboardFolder> GetFoldersByParentId(ObjectId? objectId) {
            List<ClipboardFolder> result = [];

            // objectIdがnullの場合は、空のリストを返す
            if (objectId == null) {
                return result;
            }
            // objectIdに対応するフォルダをすべて取得して返す
            var collection = GetDatabase().GetCollection<ClipboardFolder>(CLIPBOARD_FOLDERS_COLLECTION_NAME);
            // ParentIdが、objectIdのフォルダを取得。FolderNameでソートして返す
            var items = collection.FindAll().Where(x => x.ParentId == objectId).OrderBy(x => x.FolderName).ToList();

            foreach (var i in items) {
                result.Add(i);
            }
            return result;
        }


        // 親フォルダのCollectionNameを指定して子フォルダのリストを取得する
        private List<ClipboardFolder> GetChildFolders(ObjectId folderObjectId) {
            List<ClipboardFolder> result = [];
            // 親フォルダを取得
            var parent = GetFolder(folderObjectId);
            if (parent == null) {
                return result;
            }
            // 子フォルダをFolderNameでソートして返す
            result = parent.Children.OrderBy(x => x.FolderName).ToList();
            return result;
        }
        # endregion

        //---- 自動処理関連 ----------------------------------------------
        // 指定したTargetFolderを持つAutoProcessRuleを取得する
        public List<AutoProcessRule> GetAutoProcessRules(ClipboardFolder targetFolder) {
            List<AutoProcessRule> result = [];
            if (targetFolder == null) {
                return result;
            }
            var collection = GetDatabase().GetCollection<AutoProcessRule>(AUTO_PROCESS_RULES_COLLECTION_NAME);
            // targetFolderのAutoProcessRuleIdsに対応するAutoProcessRuleを取得
            foreach (var i in targetFolder.AutoProcessRuleIds) {
                var item = collection.FindById(i);
                if (item != null) {
                    result.Add(item);
                }
            }

            return result;
        }

        // すべてのAutoProcessRuleを取得する
        public IEnumerable<AutoProcessRule> GetAllAutoProcessRules() {
            var collection = GetDatabase().GetCollection<AutoProcessRule>(AUTO_PROCESS_RULES_COLLECTION_NAME);
            return collection.FindAll().OrderBy(x => x.Priority);
        }

        // AutoProcessRuleを追加または更新する
        public void UpsertAutoProcessRule(AutoProcessRule rule) {
            var collection = GetDatabase().GetCollection<AutoProcessRule>(AUTO_PROCESS_RULES_COLLECTION_NAME);
            collection.Upsert(rule);
        }
        // AutoProcessRuleを削除する
        public void DeleteAutoProcessRule(AutoProcessRule rule) {
            var collection = GetDatabase().GetCollection<AutoProcessRule>(AUTO_PROCESS_RULES_COLLECTION_NAME);
            collection.Delete(rule.Id);
        }

        // TypeがCopyTo または MoveToのルールをLiteDBから取得する。
        public IEnumerable<AutoProcessRule> GetCopyToMoveToRules() {
            var collection = GetDatabase().GetCollection<AutoProcessRule>(AUTO_PROCESS_RULES_COLLECTION_NAME);
            var items = collection.FindAll().Where(
                x => x.RuleAction != null
                && (x.RuleAction.Name == SystemAutoProcessItem.TypeEnum.CopyToFolder.ToString()
                    || x.RuleAction.Name == SystemAutoProcessItem.TypeEnum.MoveToFolder.ToString()));
            return items;

        }
        // SearchConditionを追加または更新する
        public void UpsertSearchRule(SearchRule conditionRule) {
            var collection = GetDatabase().GetCollection<SearchRule>(SEARCH_CONDITION_RULES_COLLECTION_NAME);
            collection.Upsert(conditionRule);
        }
        // 名前を指定して検索条件を取得する
        public SearchRule? GetSearchRule(string name) {
            var collection = GetDatabase().GetCollection<SearchRule>(SEARCH_CONDITION_RULES_COLLECTION_NAME);
            var item = collection.FindOne(x => x.Name == name);
            return item;
        }
        // 指定したCollectionNameに対応する検索条件を取得する
        public SearchRule? GetSearchRuleByFolder(ClipboardFolder folder) {
            var collection = GetDatabase().GetCollection<SearchRule>(SEARCH_CONDITION_RULES_COLLECTION_NAME);
            var item = collection.FindOne(x => x.SearchFolder != null && x.SearchFolder.Id == folder.Id);
            return item;
        }

        #region クリップボードアイテム関連
        // --- クリップボードアイテム関連 ----------------------------------------------
        // ClipboardItemのリストをLiteDBから取得する
        public IEnumerable<ClipboardItem> GetItems(ClipboardFolder folder) {
            var collection = GetDatabase().GetCollection<ClipboardItem>(CONTENT_ITEM_COLLECTION_NAME);
            var items = collection.FindAll().Where(x => x.CollectionId == folder.Id).OrderByDescending(x => x.UpdatedAt);
            return items;

        }

        // ClipboardItemを検索する。
        public IEnumerable<ClipboardItem> SearchItems(ClipboardFolder folder, SearchCondition searchCondition) {
            // 結果を格納するIEnumerable<ContentItem>を作成
            IEnumerable<ClipboardItem> result = [];
            // フォルダが存在しない場合は、結果を返す
            if (folder == null) {
                return result;
            }
            // 検索条件が空の場合は、結果を返す
            if (searchCondition.IsEmpty()) {
                return result;
            }

            // folder内のアイテムを保持するコレクションを取得
            var collection = GetDatabase().GetCollection<ClipboardItem>(CONTENT_ITEM_COLLECTION_NAME);
            var clipboardItems = collection.FindAll().Where(x => x.CollectionId == folder.Id).OrderByDescending(x => x.UpdatedAt);
            // Filterの結果を結果に追加
            result = Filter(clipboardItems, searchCondition);

            // サブフォルダを含む場合は、対象フォルダとそのサブフォルダを検索
            if (searchCondition.IsIncludeSubFolder) {
                // 対象フォルダの子フォルダを取得
                var childFolders = GetChildFolders(folder.Id);
                foreach (var childFolder in childFolders) {
                    // サブフォルダのアイテムを検索
                    var subFolderResult = SearchItems(childFolder, searchCondition);
                    // Filterの結果を結果に追加
                    result = result.Concat(subFolderResult);
                }
            }
            return result;

        }


        public void DeleteItems(ClipboardFolder targetFolder) {
            foreach (var item in targetFolder.Items) {
                if (item.IsPinned == false) {
                    DeleteItem(item);
                }
                targetFolder.Items.Clear();
                UpsertFolder(targetFolder);

            }
        }

        public IEnumerable<ClipboardItem> Filter(IEnumerable<ClipboardItem> liteCollection, SearchCondition searchCondition) {
            if (searchCondition.IsEmpty()) {
                return liteCollection;
            }

            var results = liteCollection;
            // SearchConditionの内容に従ってフィルタリング
            if (string.IsNullOrEmpty(searchCondition.Description) == false) {
                if (searchCondition.ExcludeDescription) {
                    results = results.Where(x => x.Description != null && x.Description.Contains(searchCondition.Description) == false);
                } else {
                    results = results.Where(x => x.Description != null && x.Description.Contains(searchCondition.Description));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.Content) == false) {
                if (searchCondition.ExcludeContent) {
                    results = results.Where(x => x.Content != null && x.Content.Contains(searchCondition.Content) == false);
                } else {
                    results = results.Where(x => x.Content != null && x.Content.Contains(searchCondition.Content));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.Tags) == false) {
                if (searchCondition.ExcludeTags) {
                    results = results.Where(x => x.Tags != null && x.Tags.Contains(searchCondition.Tags) == false);
                } else {
                    results = results.Where(x => x.Tags != null && x.Tags.Contains(searchCondition.Tags));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.SourceApplicationName) == false) {
                if (searchCondition.ExcludeSourceApplicationName) {
                    results = results.Where(x => x.SourceApplicationName != null && x.SourceApplicationName.Contains(searchCondition.SourceApplicationName) == false);
                } else {
                    results = results.Where(x => x.SourceApplicationName != null && x.SourceApplicationName.Contains(searchCondition.SourceApplicationName));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.SourceApplicationTitle) == false) {
                if (searchCondition.ExcludeSourceApplicationTitle) {
                    results = results.Where(x => x.SourceApplicationTitle != null && x.SourceApplicationTitle.Contains(searchCondition.SourceApplicationTitle) == false);
                } else {
                    results = results.Where(x => x.SourceApplicationTitle != null && x.SourceApplicationTitle.Contains(searchCondition.SourceApplicationTitle));
                }
            }
            if (searchCondition.EnableStartTime) {
                results = results.Where(x => x.CreatedAt > searchCondition.StartTime);
            }
            if (searchCondition.EnableEndTime) {
                results = results.Where(x => x.CreatedAt < searchCondition.EndTime);
            }
            results = results.OrderByDescending(x => x.UpdatedAt);

            return results;

        }
        #endregion
    }
}
