using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;
using PythonAILib.Resource;
using PythonAILib.Utils.Common;

namespace PythonAILib.Model.Content {
    public class ContentFolder {

        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.NewObjectId();

        // プロパティ
        // 親フォルダのID
        public ObjectId ParentId { get; set; } = ObjectId.Empty;

        // ルートフォルダか否か
        public bool IsRootFolder { get; set; } = false;

        // フォルダに設定されたメインベクトルDBを参照用のベクトルDBのリストに含めるかどうかを示すプロパティ名を変更
        public bool IncludeInReferenceVectorDBItems { get; set; } = true;

        // 参照用のベクトルDBのリストのプロパティ
        private List<VectorDBItem> _referenceVectorDBItems = [];
        public List<VectorDBItem> ReferenceVectorDBItems {
            get {
                return _referenceVectorDBItems;
            }
            set {
                _referenceVectorDBItems = value;
            }
        }

        // MainVectorDBのプロパティ
        private VectorDBItem? _mainVectorDBItem;
        public VectorDBItem MainVectorDBItem {
            get {
                var parentFolder = GetParent<ContentFolder>();
                if (parentFolder == null) {
                    _mainVectorDBItem ??= VectorDBItem.GetFolderVectorDBItem();
                } else {
                    _mainVectorDBItem = parentFolder.MainVectorDBItem;
                }
                _mainVectorDBItem.Description = Description;
                _mainVectorDBItem.CollectionName = Id.ToString();
                _mainVectorDBItem.FolderId = Id.ToString();

                return _mainVectorDBItem;
            }
            set {
                var parentFolder = GetParent<ContentFolder>();
                if (parentFolder == null) {
                    _mainVectorDBItem = value;
                } else {
                    _mainVectorDBItem = parentFolder.MainVectorDBItem;
                }
            }
        }

        // フォルダの絶対パス ファイルシステム用
        public virtual string FolderPath { get; } = "";
        //　フォルダ名
        public virtual string FolderName { get; set; } = "";
        // Description
        public virtual string Description { get; set; } = "";

        // 子フォルダ
        public virtual List<T> GetChildren<T>() where T : ContentFolder {
            // DBからParentIDが自分のIDのものを取得
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T>();
            var folders = collection.Find(x => x.ParentId == Id).OrderBy(x => x.FolderName);
            return folders.Cast<T>().ToList();
        }

        // 親フォルダ
        public virtual T? GetParent<T>() where T : ContentFolder {
            if (ParentId == ObjectId.Empty) {
                return null;
            }
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T>();
            return collection.FindById(ParentId);
        }

        // フォルダを削除
        public virtual void DeleteFolder<T1, T2>(T1 folder) where T1 : ContentFolder where T2 : ContentItem {
            // folderの子フォルダを再帰的に削除
            foreach (var child in folder.GetChildren<T1>()) {
                if (child != null) {
                    DeleteFolder<T1, T2>(child);
                }
            }
            // folderのアイテムを削除
            var items = PythonAILibManager.Instance.DataFactory.GetItemCollection<T2>().Find(x => x.CollectionId == folder.Id);
            foreach (var item in items) {
                item.Delete();
            }
            // folderを削除
            var folderCollection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T1>();
            folderCollection.Delete(folder.Id);
        }

        // フォルダを移動する
        public virtual void MoveTo(ContentFolder toFolder) {
            // 自分自身を移動
            ParentId = toFolder.Id;
            Save<ContentFolder, ContentItem>();
        }
        // 名前を変更
        public virtual void Rename(string newName) {
            FolderName = newName;
            Save<ContentFolder, ContentItem>();
        }

        // 保存
        public virtual void Save<T1, T2>() where T1 : ContentFolder where T2 : ContentItem {
            
            // ReferenceVectorDBItemsに自分自身を追加
            // IncludeInReferenceVectorDBItemsがTrueの場合は、ReferenceVectorDBItemsに自分自身を追加
            if (IncludeInReferenceVectorDBItems) {
                AddVectorDBItem(MainVectorDBItem);
            } else {
                // IncludeInReferenceVectorDBItemsがFalseの場合は、ReferenceVectorDBItemsから自分自身を削除
                RemoveVectorDBItem(MainVectorDBItem);
            }

            IDataFactory dataFactory = PythonAILibManager.Instance.DataFactory;
            dataFactory.GetFolderCollection<T1>().Upsert((T1)this);

            // ItemsのIsReferenceVectorDBItemsSyncedをFalseに設定
            foreach (var item in GetItems<T2>()) {
                item.IsReferenceVectorDBItemsSynced = false;
                item.Save(false);
            }
        }
        // 削除
        public virtual void Delete<T1,T2>() where T1 : ContentFolder where T2 : ContentItem {
            DeleteFolder<T1, T2>((T1)this);
        }

        public virtual List<T> GetItems<T>() where T : ContentItem {
            var collection = PythonAILibManager.Instance.DataFactory.GetItemCollection<T>();
            var items = collection.Find(x => x.CollectionId == Id).OrderByDescending(x => x.UpdatedAt);
            return items.Cast<T>().ToList();
        }
        #region ベクトル検索
        // ReferenceVectorDBItemsからVectorDBItemを削除
        public void RemoveVectorDBItem(VectorDBItem vectorDBItem) {
            List<VectorDBItem> existingItems = new(ReferenceVectorDBItems.Where(x => x.Name == vectorDBItem.Name && x.CollectionName == vectorDBItem.CollectionName && x.FolderId == vectorDBItem.FolderId));
            foreach (var item in existingItems) {
                ReferenceVectorDBItems.Remove(item);
            }
        }
        // ReferenceVectorDBItemsにVectorDBItemを追加
        public void AddVectorDBItem(VectorDBItem vectorDBItem) {
            var existingItems = ReferenceVectorDBItems.FirstOrDefault(x => x.Name == vectorDBItem.Name && x.CollectionName == vectorDBItem.CollectionName && x.FolderId == vectorDBItem.FolderId);
            if (existingItems == null) {
                ReferenceVectorDBItems.Add(vectorDBItem);
            }
        }
        // フォルダに設定されたVectorDBのコレクションを削除
        public void DeleteVectorDBCollection() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();

            ChatRequestContext chatRequestContext = new() {
                OpenAIProperties = openAIProperties,
                VectorDBItems = [MainVectorDBItem]
            };

            PythonExecutor.PythonAIFunctions.DeleteVectorDBCollection(chatRequestContext);
        }

        // フォルダに設定されたVectorDBのインデックスを更新
        public void RefreshVectorDBCollection<T>() where T : ContentItem {
            // ベクトルを全削除
            DeleteVectorDBCollection();
            // ベクトルを再作成
            // フォルダ内のアイテムを取得して、ベクトルを作成
            foreach (var item in GetItems<T>()) {
                item.UpdateEmbedding();
                // Save
                item.Save();
            }
        }

        #endregion
        public virtual ContentFolder CreateChild(string folderName) {
            ContentFolder folder = new() {
                ParentId = Id,
                FolderName = folderName,
            };
            return folder;
        }

        public virtual void AddItem(ContentItem item) {
            // CollectionNameを設定
            item.CollectionId = Id;
            // ReferenceVectorDBItemsを設定
            item.ReferenceVectorDBItems = ReferenceVectorDBItems;

            // 保存
            item.Save();
            // 通知
            LogWrapper.Info(PythonAILibStringResources.Instance.AddedItems);

        }

    }
}
