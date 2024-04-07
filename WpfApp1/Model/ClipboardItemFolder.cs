using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;
using WpfApp1.Utils;
using WpfApp1.View.ClipboardItemView;

namespace WpfApp1.Model {
    public class ClipboardItemFolder : ObservableObject {

        // アプリ共通の検索条件
        public static SearchCondition GlobalSearchCondition = new SearchCondition();

        //--------------------------------------------------------------------------------
        public static ClipboardItemFolder RootFolder {
            get {
                var folder = ClipboardDatabaseController.GetClipboardItemFolder(ClipboardDatabaseController.CLIPBOARD_ROOT_FOLDER_NAME);
                if (folder == null) {
                    folder = new(ClipboardDatabaseController.CLIPBOARD_ROOT_FOLDER_NAME, "クリップボード");
                    ClipboardDatabaseController.UpsertFolder(folder);
                }
                return folder;
            }
        }
        public static ClipboardItemFolder SearchRootFolder {
            get {
                var folder = ClipboardDatabaseController.GetClipboardItemFolder(ClipboardDatabaseController.SEARCH_ROOT_FOLDER_NAME);
                if (folder == null) {
                    folder = new(ClipboardDatabaseController.SEARCH_ROOT_FOLDER_NAME, "検索フォルダ");
                    ClipboardDatabaseController.UpsertFolder(folder);
                }
                return folder;
            }
        }

        // プロパティ
        // LiteDBのID
        public ObjectId? Id { get; set; }
        // フォルダの表示名
        public string DisplayName { get; set; } = "";
        // フォルダの絶対パス
        public string AbsoluteCollectionName { get; set; } = "";

        // 検索フォルダかどうか
        public bool IsSearchFolder { get; set; } = false;

        // 検索対象のフォルダの絶対パス
        public string SearchFolderAbsoluteCollectionName { get; set; } = "";

        // 検索条件
        public SearchCondition SearchCondition { get; set; } = new SearchCondition();


        // 現在、検索条件を適用中かどうか
        public bool IsApplyingSearchCondition { get; set; } = false;

        // 子フォルダ BSonMapper.GlobalでIgnore設定しているので、LiteDBには保存されない
        public ObservableCollection<ClipboardItemFolder> Children {
            get {
                ObservableCollection<ClipboardItemFolder> children = new ObservableCollection<ClipboardItemFolder>();
                // AbsoluteCollectionNameが空の場合は空のリストを返す
                if (string.IsNullOrEmpty(AbsoluteCollectionName)) {
                    return children;
                }
                // LiteDBから自分が親となっているフォルダを取得
                var childrenNames = ClipboardDatabaseController.GetClipboardItemFolderRelation(AbsoluteCollectionName);
                foreach (var childName in childrenNames) {
                    var child = ClipboardDatabaseController.GetClipboardItemFolder(childName);
                    if (child != null) {
                        children.Add(child);
                    }
                }
                return children;
            }
        }

        // アイテム BSonMapper.GlobalでIgnore設定しているので、LiteDBには保存されない
        public ObservableCollection<ClipboardItem> Items {
            get {
                // AbsoluteCollectionNameが空の場合は空のリストを返す
                if (string.IsNullOrEmpty(AbsoluteCollectionName)) {
                    return new ObservableCollection<ClipboardItem>();
                }
                // AbsoluteCollectionNameのコレクションを取得
                var itemsCollection = ClipboardDatabaseController.GetClipboardDatabase().GetCollection<ClipboardItem>(AbsoluteCollectionName);
                return new ObservableCollection<ClipboardItem>(itemsCollection.FindAll());
            }
        }
        //--------------------------------------------------------------------------------
        // コンストラクタ
        public ClipboardItemFolder() {
        }

        public ClipboardItemFolder(ClipboardItemFolder? parent, string collectionName, string displayName) {
            if (parent == null) {
                AbsoluteCollectionName = collectionName;
            } else {
                AbsoluteCollectionName = ClipboardDatabaseController.ConcatenatePath(parent.AbsoluteCollectionName, collectionName);
            }
            DisplayName = displayName;
            // SearchConditionの名前を設定
            SearchCondition.Name = $"{AbsoluteCollectionName}の検索条件";

        }
        public ClipboardItemFolder(string collectionName, string displayName) : this(null, collectionName, displayName) {

        }
        //--------------------------------------------------------------------------------

        // ClipboardItemを作成
        public ClipboardItem CreateItem() {
            ClipboardItem item = new ClipboardItem();
            item.CollectionName = AbsoluteCollectionName;
            // Itemsに追加
            Items.Add(item);
            // 追加したアイテムをSelectedItemに設定
            if (MainWindowViewModel.Instance != null) {
                MainWindowViewModel.Instance.SelectedItem = new ClipboardItemViewModel(item);
            }
            ClipboardDatabaseController.UpsertFolder(this);
            return item;
        }
        // ClipboardItemを追加
        public void UpsertItem(ClipboardItem item) {
            Items.Add(item);
            // 追加したアイテムをSelectedItemに設定
            if (MainWindowViewModel.Instance != null) {
                MainWindowViewModel.Instance.SelectedItem = new ClipboardItemViewModel(item);
            }

            // LiteDBに保存
            ClipboardDatabaseController.UpsertItem(item);
        }
        // ClipboardItemを削除
        public void DeleteItem(ClipboardItem item) {
            // LiteDBに保存
            ClipboardDatabaseController.DeleteItem(item);
        }

        public List<ClipboardItem> GetDuplicateList(ClipboardItem item) {
            List<ClipboardItem> dupList = new List<ClipboardItem>();
            // prevCountで指定された数だけ過去のアイテムと比較する
            for (int i = 0; i < Items.Count; i++) {
                ClipboardItem prev = Items[i];
                if (item.IsDuplicate(prev)) {
                    dupList.Add(prev);
                }
            }
            return dupList;
        }

        private void RemoveDuplicateItems(ClipboardItem item) {
            List<ClipboardItem> dupList = GetDuplicateList(item);
            foreach (var dupItem in dupList) {
                DeleteItem(dupItem);
            }
        }
        public bool Load() {
            bool result = ClipboardDatabaseController.Load(this);
            return result;
        }

        public void DeleteItems() {
            ClipboardDatabaseController.DeleteItems(this);
        }

        public void Filter(SearchCondition searchCondition) {
            ClipboardDatabaseController.Filter(this, searchCondition);
        }

        private bool _IsSelected;
        public bool IsSelected {
            get { return _IsSelected; }
            set {
                _IsSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        // FolderSelectWindowで選択されたフォルダを適用する処理
        private bool _IsSelectedOnFolderSelectWindow;
        public bool IsSelectedOnFolderSelectWindow {
            get { return _IsSelectedOnFolderSelectWindow; }
            set {
                _IsSelectedOnFolderSelectWindow = value;
                OnPropertyChanged("_IsSelectedOnFolderSelectWindow");
            }
        }

        // 自動処理を適用する処理
        public ClipboardItem? ApplyAutoProcess(ClipboardItem clipboardItem) {
            ClipboardItem? result = clipboardItem;
            // AutoProcessRulesを取得
            var AutoProcessRules = ClipboardDatabaseController.GetAutoProcessRules(this);
            foreach (var rule in AutoProcessRules) {
                Tools.Info("自動処理を適用します " + rule.GetDescriptionString());
                result = rule.RunAction(result);
                // resultがNullの場合は処理を中断
                if (result == null) {
                    Tools.Info("自動処理でアイテムが削除されました");
                    return null;
                }
            }
            return result;
        }

        // アイテムを追加する処理
        public ClipboardItem AddItem(ClipboardItem item) {
            // AbsoluteCollectionNameを設定
            item.CollectionName = AbsoluteCollectionName;

            // 自動処理を適用
            ClipboardItem? result = ApplyAutoProcess(item);

            if (result == null) {
                // 自動処理で削除または移動された場合は何もしない
                Tools.Info("自動処理でアイテムが削除または移動されました");
                return item;
            }

            // 重複アイテムを削除
            RemoveDuplicateItems(result);
            // LiteDBに保存
            ClipboardDatabaseController.UpsertItem(result);
            // Itemsに追加
            Items.Add(result);
            // OnPropertyChanged
            OnPropertyChanged("Items");


            return item;

        }

    }
}
