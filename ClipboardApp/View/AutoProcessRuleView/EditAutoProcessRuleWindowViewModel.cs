
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ClipboardApp.View.ClipboardItemFolderView;
using WpfAppCommon.Utils;
using WpfAppCommon.Model;

namespace ClipboardApp.View.AutoProcessRuleView
{
    public class EditAutoProcessRuleWindowViewModel : ObservableObject {
        public enum Mode {
            Create,
            Edit
        }
        // ルール適用対象のClipboardItemFolder
        private ClipboardFolderViewModel? _TargetFolder { get; set; }
        public ClipboardFolderViewModel? TargetFolder {
            get {
                return _TargetFolder;
            }
            set {
                _TargetFolder = value;
                OnPropertyChanged("TargetFolder");
            }
        }

        // 編集対象の自動処理ルール
        AutoProcessRule? TargetAutoProcessRule { get; set; }

        // 自動処理ルールの名前
        public string RuleName { get; set; } = "";
        // 自動処理が有効かどうか
        private bool _IsAutoProcessRuleEnabled = true;
        public bool IsAutoProcessRuleEnabled {
            get {
                return _IsAutoProcessRuleEnabled;
            }
            set {
                _IsAutoProcessRuleEnabled = value;
                OnPropertyChanged("IsAutoProcessRuleEnabled");
            }
        }
        // AutoProcessRuleのリスト
        public ObservableCollection<AutoProcessItem> AutoProcessItems { get; set; } = new ObservableCollection<AutoProcessItem>(AutoProcessItem.SystemAutoProcesses);

        // 自動処理ルールの条件リスト
        public ObservableCollection<AutoProcessRuleCondition> Conditions { get; set; } = new ObservableCollection<AutoProcessRuleCondition>();
        // 自動処理ルールのアクション
        private AutoProcessItem? _SelectedAutoProcessItem = null;
        public AutoProcessItem? SelectedAutoProcessItem {
            get {
                return _SelectedAutoProcessItem;
            }
            set {
                if (value == null) {
                    return;
                }
                _SelectedAutoProcessItem = value;

                OnPropertyChanged("SelectedAutoProcessItem");

                // アクションがコピーまたは移動の場合はFolderSelectionPanelEnabledをtrueにする
                if (value.IsCopyOrMoveOrMergeAction()) {
                    FolderSelectionPanelEnabled = true;
                } else {
                    FolderSelectionPanelEnabled = false;
                }
            }
        }

        // モード
        public Mode CurrentMode { get; set; }

        // すべてのアイテムに対してルールを有効にするかどうか
        public bool IsAllItemsRuleChecked { get; set; } = false;

        // 説明のルールを有効にするかどうか
        public bool IsDescriptionRuleChecked { get; set; } = false;
        public string Description { get; set; } = "";

        // クリップボードの内容のルールを有効にするかどうか
        public bool IsContentRuleChecked { get; set; } = false;
        public string Content { get; set; } = "";

        // ソースアプリケーションのルールを有効にするかどうか
        public bool IsSourceApplicationRuleChecked { get; set; } = false;
        public string SourceApplicationName { get; set; } = "";

        // ソースアプリケーションのタイトルのルールを有効にするかどうか
        public bool IsSourceApplicationTitleRuleChecked { get; set; } = false;
        public string SourceApplicationTitle { get; set; } = "";


        // コピーまたは移動先のフォルダ
        private ClipboardFolderViewModel? _DestinationFolder = null;
        public ClipboardFolderViewModel? DestinationFolder {
            get {
                return _DestinationFolder;
            }
            set {
                _DestinationFolder = value;
                OnPropertyChanged("DestinationFolder");
            }
        }

        // アクションがコピーまたは移動の場合にFolderSelectionPanelをEnabledにする
        private bool _FolderSelectionPanelEnabled = false;
        public bool FolderSelectionPanelEnabled {
            get {
                return _FolderSelectionPanelEnabled;
            }
            set {
                _FolderSelectionPanelEnabled = value;
                OnPropertyChanged("FolderSelectionPanelEnabled");
            }
        }

        // 自動処理を更新したあとの処理
        private Action<AutoProcessRule>? _AfterUpdate;

        // 
        // 初期化
        private void Initialize(Mode mode, ClipboardFolderViewModel? folderViewModel, AutoProcessRule? autoProcessRule, Action<AutoProcessRule> afterUpdate) {
            CurrentMode = mode;
            TargetFolder = folderViewModel;
            TargetAutoProcessRule = autoProcessRule;
            IsAutoProcessRuleEnabled = autoProcessRule?.IsEnabled ?? true;
            _AfterUpdate = afterUpdate;

            if (autoProcessRule?.TargetFolder != null) {
                TargetFolder = new ClipboardFolderViewModel(autoProcessRule.TargetFolder);
            }
            if (autoProcessRule?.DestinationFolder != null) {
                DestinationFolder = new ClipboardFolderViewModel(autoProcessRule.DestinationFolder);
            }


            // autoProcessRuleがNullでない場合は初期化
            if (TargetAutoProcessRule != null) {
                RuleName = TargetAutoProcessRule.RuleName;
                OnPropertyChanged("RuleName");
                Conditions = new ObservableCollection<AutoProcessRuleCondition>(TargetAutoProcessRule.Conditions);
                SelectedAutoProcessItem = TargetAutoProcessRule.RuleAction;

                foreach (var condition in TargetAutoProcessRule.Conditions) {
                    switch (condition.Type) {
                        case AutoProcessRuleCondition.ConditionType.AllItems:
                            IsAllItemsRuleChecked = true;
                            OnPropertyChanged("IsAllItemsRuleChecked");
                            break;

                        case AutoProcessRuleCondition.ConditionType.DescriptionContains:
                            IsDescriptionRuleChecked = true;
                            OnPropertyChanged("IsDescriptionRuleChecked");
                            Description = condition.Keyword;
                            OnPropertyChanged("Description");
                            break;
                        case AutoProcessRuleCondition.ConditionType.ContentContains:
                            IsContentRuleChecked = true;
                            OnPropertyChanged("IsContentRuleChecked");
                            Content = condition.Keyword;
                            OnPropertyChanged("Content");
                            break;
                        case AutoProcessRuleCondition.ConditionType.SourceApplicationNameContains:
                            IsSourceApplicationRuleChecked = true;
                            OnPropertyChanged("IsSourceApplicationRuleChecked");
                            SourceApplicationName = condition.Keyword;
                            OnPropertyChanged("SourceApplicationName");
                            break;
                        case AutoProcessRuleCondition.ConditionType.SourceApplicationTitleContains:
                            IsSourceApplicationTitleRuleChecked = true;
                            OnPropertyChanged("IsSourceApplicationTitleRuleChecked");
                            SourceApplicationTitle = condition.Keyword;
                            OnPropertyChanged("SourceApplicationTitle");
                            break;
                    }
                }
                OnPropertyChanged("Conditions");
            }
        }

        // 初期化 modeがEditの場合
        public void InitializeEdit(ClipboardFolderViewModel? clipboardItemFolder, AutoProcessRule autoProcessRule, Action<AutoProcessRule> afterUpdate) {
            Initialize(Mode.Edit, clipboardItemFolder, autoProcessRule, afterUpdate);

        }
        // 初期化 modeがCreateの場合
        public void InitializeCreate(ClipboardFolderViewModel? clipboardItemFolder, Action<AutoProcessRule> afterUpdate) {
            Initialize(Mode.Create, clipboardItemFolder, null, afterUpdate);
        }

        // OKボタンが押されたときの処理
        public SimpleDelegateCommand OKButtonClickedCommand => new SimpleDelegateCommand(OKButtonClickedCommandExecute);
        public void OKButtonClickedCommandExecute(object parameter) {
            // TargetFolderがNullの場合はエラー
            if (TargetFolder == null) {
                Tools.Error("フォルダが選択されていません。");
                return;
            }
            // RuleNameが空の場合はエラー
            if (string.IsNullOrEmpty(RuleName)) {
                Tools.Error("ルール名を入力してください。");
                return;
            }
            // SelectedAutoProcessItemが空の場合はエラー
            if (SelectedAutoProcessItem == null) {
                Tools.Error("アクションを選択してください。");
                return;
            }
            // 新規作成
            if (CurrentMode == Mode.Create) {
                TargetAutoProcessRule = new AutoProcessRule(RuleName, TargetFolder.ClipboardItemFolder);
            }
            // 編集
            else {
                if (TargetAutoProcessRule == null) {
                    Tools.Error("編集対象のルールが見つかりません。");
                    return;
                }
                TargetAutoProcessRule.Conditions.Clear();
                TargetAutoProcessRule.RuleName = RuleName;
            }
            // IsAllItemsRuleCheckedがTrueの場合は条件を追加
            if (IsAllItemsRuleChecked) {
                // AllItemsを条件に追加
                TargetAutoProcessRule.Conditions.Add(new AutoProcessRuleCondition(
                                       AutoProcessRuleCondition.ConditionType.AllItems, ""));
            }
            // IsAutoProcessRuleEnabledがTrueの場合はIsEnabledをTrueにする
            TargetAutoProcessRule.IsEnabled = IsAutoProcessRuleEnabled;

            // TargetFolderを設定
            TargetAutoProcessRule.TargetFolder = TargetFolder.ClipboardItemFolder;

            // IsDescriptionRuleCheckedがTrueの場合は条件を追加
            if (IsDescriptionRuleChecked) {
                // Descriptionを条件に追加

                TargetAutoProcessRule.Conditions.Add(new AutoProcessRuleCondition(
                    AutoProcessRuleCondition.ConditionType.DescriptionContains, Description));
            }
            // IsContentRuleCheckedがTrueの場合は条件を追加
            if (IsContentRuleChecked) {
                // Contentを条件に追加
                TargetAutoProcessRule.Conditions.Add(new AutoProcessRuleCondition(
                    AutoProcessRuleCondition.ConditionType.ContentContains, Content));
            }
            // IsSourceApplicationRuleCheckedがTrueの場合は条件を追加
            if (IsSourceApplicationRuleChecked) {
                // SourceApplicationNameを条件に追加
                TargetAutoProcessRule.Conditions.Add(new AutoProcessRuleCondition(
                    AutoProcessRuleCondition.ConditionType.SourceApplicationNameContains, SourceApplicationName));
            }
            // IsSourceApplicationTitleRuleCheckedがTrueの場合は条件を追加
            if (IsSourceApplicationTitleRuleChecked) {
                // SourceApplicationTitleを条件に追加
                TargetAutoProcessRule.Conditions.Add(new AutoProcessRuleCondition(
                    AutoProcessRuleCondition.ConditionType.SourceApplicationTitleContains, SourceApplicationTitle));
            }
            // アクションを追加
            TargetAutoProcessRule.RuleAction = SelectedAutoProcessItem;
            // アクションタイプがCopyToFolderまたは MoveToFolderの場合はDestinationFolderを設定
            if (SelectedAutoProcessItem.IsCopyOrMoveOrMergeAction()) {
                if (DestinationFolder == null) {
                    Tools.Error("コピーまたは移動先のフォルダを選択してください。");
                    return;
                }
                // TargetFolderとDestinationFolderが同じ場合はエラー
                if (TargetFolder.ClipboardItemFolder.AbsoluteCollectionName == DestinationFolder.ClipboardItemFolder.AbsoluteCollectionName) {
                    Tools.Error("同じフォルダにはコピーまたは移動できません。");
                    return;
                }
                TargetAutoProcessRule.DestinationFolder = DestinationFolder.ClipboardItemFolder;
            }
            // 無限ループのチェック処理
            if (AutoProcessRule.CheckInfiniteLoop(TargetAutoProcessRule)) {
                Tools.Error("コピー/移動処理の無限ループを検出しました。");
                return;
            }

            // LiteDBに保存
            TargetAutoProcessRule.Save();

            // AutoProcessRuleを更新したあとの処理を実行
            _AfterUpdate?.Invoke(TargetAutoProcessRule);

            // ウィンドウを閉じる
            if (parameter is System.Windows.Window window) {
                window.Close();
            }

        }
        // キャンセルボタンが押されたときの処理
        public SimpleDelegateCommand CancelButtonClickedCommand => new SimpleDelegateCommand(CancelButtonClickedCommandExecute);
        public void CancelButtonClickedCommandExecute(object parameter) {
            // ウィンドウを閉じる
            if (parameter is System.Windows.Window window) {
                window.Close();
            }

        }
        // OnSelectedFolderChanged
        public void OnSelectedFolderChanged(ClipboardFolderViewModel? folder) {
            if (folder == null) {
                return;
            }
            // コピーor移動先が同じフォルダの場合はエラー
            if (folder.ClipboardItemFolder.AbsoluteCollectionName == TargetFolder?.ClipboardItemFolder.AbsoluteCollectionName) {
                Tools.Error("同じフォルダにはコピーまたは移動できません。");
                return;
            }// コピーor移動先が検索フォルダの場合はエラー
            if (folder.ClipboardItemFolder.IsSearchFolder) {
                Tools.Error("検索フォルダにはコピーまたは移動できません。");
                return;
            }
            DestinationFolder = folder;

        }
        // OpenSelectDestinationFolderWindowCommand
        public SimpleDelegateCommand OpenSelectDestinationFolderWindowCommand => new SimpleDelegateCommand(OpenSelectDestinationFolderWindowCommandExecute);
        public void OpenSelectDestinationFolderWindowCommandExecute(object parameter) {
            // フォルダが選択されたら、DestinationFolderに設定
            void FolderSelectedAction(ClipboardFolderViewModel folderViewModel) {
                DestinationFolder = folderViewModel;
            }
            FolderSelectWindow FolderSelectWindow = new FolderSelectWindow();
            FolderSelectWindowViewModel FolderSelectWindowViewModel = (FolderSelectWindowViewModel)FolderSelectWindow.DataContext;
            ClipboardFolderViewModel? rootFolderViewModel = new ClipboardFolderViewModel(ClipboardFolder.RootFolder);
            FolderSelectWindowViewModel.Initialize(rootFolderViewModel, FolderSelectedAction);
            FolderSelectWindow.ShowDialog();
        }

        // OpenSelectTargetFolderWindowCommand
        public SimpleDelegateCommand OpenSelectTargetFolderWindowCommand => new SimpleDelegateCommand(OpenSelectTargetFolderWindowCommandExecute);
        public void OpenSelectTargetFolderWindowCommandExecute(object parameter) {
            // フォルダが選択されたら、TargetFolderに設定
            void FolderSelectedAction(ClipboardFolderViewModel folderViewModel) {
                TargetFolder = folderViewModel;
            }
            FolderSelectWindow FolderSelectWindow = new FolderSelectWindow();
            FolderSelectWindowViewModel FolderSelectWindowViewModel = (FolderSelectWindowViewModel)FolderSelectWindow.DataContext;
            ClipboardFolderViewModel? rootFolderViewModel = new ClipboardFolderViewModel(ClipboardFolder.RootFolder);
            FolderSelectWindowViewModel.Initialize(rootFolderViewModel, FolderSelectedAction);
            FolderSelectWindow.ShowDialog();
        }
        public SimpleDelegateCommand AutoProcessItemSelectionChangedCommand => new(AutoProcessItemSelectionChangedCommandExecute);
        public void AutoProcessItemSelectionChangedCommandExecute(object parameter) {
            if (SelectedAutoProcessItem == null) {
                return;
            }
            if (SelectedAutoProcessItem.IsCopyOrMoveOrMergeAction()) {
                FolderSelectionPanelEnabled = true;
            } else {
                FolderSelectionPanelEnabled = false;
            }
        }
    }
}
