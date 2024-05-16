
using System.Collections.ObjectModel;
using ClipboardApp.View.ClipboardItemFolderView;
using WpfAppCommon.Utils;
using WpfAppCommon.Model;
using QAChat.View.PromptTemplateWindow;
using static QAChat.View.PromptTemplateWindow.ListPromptTemplateWindowViewModel;
using ClipboardApp.View.PythonScriptView;
using System.Windows;
using System.Windows.Controls;

namespace ClipboardApp.View.AutoProcessRuleView
{
    public class EditAutoProcessRuleWindowViewModel : MyWindowViewModel {
        public enum Mode {
            Create,
            Edit
        }

        // ルール適用対象のClipboardItemFolder
        private ClipboardFolderViewModel? targetFolder;
        public ClipboardFolderViewModel? TargetFolder {
            get {
                return targetFolder;
            }
            set {
                targetFolder = value;
                OnPropertyChanged(nameof(TargetFolder));
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
                OnPropertyChanged(nameof(IsAutoProcessRuleEnabled));
            }
        }
        // SystemAutoProcessRuleのリスト
        public ObservableCollection<AutoProcessItemViewModel> AutoProcessItems { get; set; } 
            = new ObservableCollection<AutoProcessItemViewModel>(AutoProcessItemViewModel.SystemAutoProcesses);

        // ScriptAutoProcessRuleのリスト
        public ObservableCollection<AutoProcessItemViewModel> ScriptAutoProcessItems { get; set; } 
            = new ObservableCollection<AutoProcessItemViewModel>(AutoProcessItemViewModel.ScriptAutoProcesses);

        // 自動処理ルールの条件リスト
        public ObservableCollection<AutoProcessRuleCondition> Conditions { get; set; } = [];
        // 自動処理ルールのアクション
        private AutoProcessItemViewModel? _SelectedAutoProcessItem = null;
        public AutoProcessItemViewModel? SelectedAutoProcessItem {
            get {
                return _SelectedAutoProcessItem;
            }
            set {
                if (value == null) {
                    return;
                }
                _SelectedAutoProcessItem = value;

                OnPropertyChanged(nameof(SelectedAutoProcessItem));

                // アクションがコピーまたは移動の場合はFolderSelectionPanelEnabledをTrueにする
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
                OnPropertyChanged(nameof(DestinationFolder));
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
                OnPropertyChanged(nameof(FolderSelectionPanelEnabled));
            }
        }
        // promptItemViewModel
        private PromptItemViewModel? _selectedPromptItem;
        public PromptItemViewModel? SelectedPromptItem {
            get => _selectedPromptItem;
            set {
                _selectedPromptItem = value;
                OnPropertyChanged(nameof(SelectedPromptItem));
            }
        }
        // ScriptItem
        private ScriptItem? _selectedScriptItem;
        public ScriptItem? SelectedScriptItem {
            get => _selectedScriptItem;
            set {
                _selectedScriptItem = value;
                OnPropertyChanged(nameof(SelectedScriptItem));
            }
        }
        // 基本処理のラジオボタンが選択中かどうか

        public bool IsBasicProcessChecked { get; set; } = true;
        // PromptTemplateのラジオボタンが選択中かどうか
        private bool _isPromptTemplateChecked = false;
        public bool IsPromptTemplateChecked {
            get {
                return _isPromptTemplateChecked;
            }
            set {
                _isPromptTemplateChecked = value;
                OnPropertyChanged(nameof(IsPromptTemplateChecked));
            }
        }
        public bool IsPythonScriptChecked { get; set; } = false;

        // OpenAIExecutionMode
        public OpenAIExecutionModeEnum OpenAIExecutionModeEnum { get; set; } = OpenAIExecutionModeEnum.Normal;

        // OpenAIExecutionModeSelectedIndex
        private int _openAIExecutionModeSelectedIndex = 0;
        public int OpenAIExecutionModeSelectedIndex {
            get {
                return _openAIExecutionModeSelectedIndex;
            }
            set {
                _openAIExecutionModeSelectedIndex = value;
                OnPropertyChanged(nameof(OpenAIExecutionModeSelectedIndex));
            }
        }

        // 自動処理を更新したあとの処理
        private Action<AutoProcessRule>? _AfterUpdate;

        // MainWindowViewModel
        public MainWindowViewModel? MainWindowViewModel { get; set; }
        // 
        // 初期化
        private void Initialize(
            Mode mode, MainWindowViewModel? mainWindowViewModel, AutoProcessRule? autoProcessRule, Action<AutoProcessRule> afterUpdate) {
            if (mainWindowViewModel == null) {
                Tools.Error("MainWindowViewModelがNullです。");
                return;
            }
            CurrentMode = mode;
            MainWindowViewModel = mainWindowViewModel;
            TargetAutoProcessRule = autoProcessRule;
            IsAutoProcessRuleEnabled = autoProcessRule?.IsEnabled ?? true;
            _AfterUpdate = afterUpdate;

            if (autoProcessRule?.TargetFolder != null) {
                TargetFolder = new ClipboardFolderViewModel(MainWindowViewModel, autoProcessRule.TargetFolder);
            }

            if (autoProcessRule?.DestinationFolder != null) {
                DestinationFolder = new ClipboardFolderViewModel(MainWindowViewModel, autoProcessRule.DestinationFolder);
            }

            // autoProcessRuleがNullでない場合は初期化
            if (TargetAutoProcessRule != null && TargetAutoProcessRule.RuleAction != null) {
                RuleName = TargetAutoProcessRule.RuleName;
                OnPropertyChanged(nameof(RuleName));
                Conditions = new ObservableCollection<AutoProcessRuleCondition>(TargetAutoProcessRule.Conditions);
                SelectedAutoProcessItem = new AutoProcessItemViewModel(TargetAutoProcessRule.RuleAction);

                foreach (var condition in TargetAutoProcessRule.Conditions) {
                    switch (condition.Type) {
                        case AutoProcessRuleCondition.ConditionType.AllItems:
                            IsAllItemsRuleChecked = true;
                            OnPropertyChanged(nameof(IsAllItemsRuleChecked));
                            break;

                        case AutoProcessRuleCondition.ConditionType.DescriptionContains:
                            IsDescriptionRuleChecked = true;
                            OnPropertyChanged(nameof(IsDescriptionRuleChecked));
                            Description = condition.Keyword;
                            OnPropertyChanged(nameof(Description));
                            break;
                        case AutoProcessRuleCondition.ConditionType.ContentContains:
                            IsContentRuleChecked = true;
                            OnPropertyChanged(nameof(IsContentRuleChecked));
                            Content = condition.Keyword;
                            OnPropertyChanged(nameof(Content));
                            break;
                        case AutoProcessRuleCondition.ConditionType.SourceApplicationNameContains:
                            IsSourceApplicationRuleChecked = true;
                            OnPropertyChanged(nameof(IsSourceApplicationRuleChecked));
                            SourceApplicationName = condition.Keyword;
                            OnPropertyChanged(nameof(SourceApplicationName));
                            break;
                        case AutoProcessRuleCondition.ConditionType.SourceApplicationTitleContains:
                            IsSourceApplicationTitleRuleChecked = true;
                            OnPropertyChanged(nameof(IsSourceApplicationTitleRuleChecked));
                            SourceApplicationTitle = condition.Keyword;
                            OnPropertyChanged(nameof(SourceApplicationTitle));
                            break;
                    }
                }
                // DestinationFolderが設定されている場合はFolderSelectionPanelEnabledをTrueにする
                if (TargetAutoProcessRule.DestinationFolder != null) {
                    FolderSelectionPanelEnabled = true;
                    DestinationFolder = new ClipboardFolderViewModel(MainWindowViewModel, TargetAutoProcessRule.DestinationFolder);

                }
                // PromptAutoProcessItemの場合
                if (TargetAutoProcessRule.RuleAction is PromptAutoProcessItem promptAutoProcessItem) {
                    if (promptAutoProcessItem.PromptItem == null) {
                        return;
                    }
                    IsPromptTemplateChecked = true;
                    SelectedPromptItem = new PromptItemViewModel(promptAutoProcessItem.PromptItem);
                    // OpenAIExecutionModeEnumの値からOpenAIExecutionModeSelectedIndexを設定
                    OpenAIExecutionModeSelectedIndex = (int)promptAutoProcessItem.Mode;

                }
                // ScriptAutoProcessItemの場合
                else if (TargetAutoProcessRule.RuleAction is ScriptAutoProcessItem scriptAutoProcessItem) {
                    if (scriptAutoProcessItem.ScriptItem == null) {
                        return;
                    }
                    IsPythonScriptChecked = true;
                    SelectedScriptItem = scriptAutoProcessItem.ScriptItem;
                }

                OnPropertyChanged(nameof(Conditions));
            }
        }

        // 初期化 modeがEditの場合
        public void InitializeEdit(
            MainWindowViewModel? mainWindowViewModel, AutoProcessRule autoProcessRule, Action<AutoProcessRule> afterUpdate) {
            Initialize(
                Mode.Edit, mainWindowViewModel, autoProcessRule, afterUpdate);

        }
        // 初期化 modeがCreateの場合
        public void InitializeCreate(
            MainWindowViewModel? mainWindowViewModel, Action<AutoProcessRule> afterUpdate) {
            Initialize(Mode.Create, mainWindowViewModel, null, afterUpdate);
        }

        // OKボタンが押されたときの処理
        public SimpleDelegateCommand OKButtonClickedCommand => new((parameter) => {
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
                TargetAutoProcessRule = new AutoProcessRule(RuleName);
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
            TargetFolder.SetAutoProcessRuleTargetFolder(TargetAutoProcessRule);

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
            // IsBasicProcessCheckedがTrueの場合はSelectedAutoProcessItemを追加
            if (IsBasicProcessChecked) {
                TargetAutoProcessRule.RuleAction = SelectedAutoProcessItem.AutoProcessItem;
                // アクションタイプがCopyToFolderまたは MoveToFolderの場合はDestinationFolderを設定
                if (SelectedAutoProcessItem.IsCopyOrMoveOrMergeAction()) {
                    if (DestinationFolder == null) {
                        Tools.Error("コピーまたは移動先のフォルダを選択してください。");
                        return;
                    }
                    // TargetFolderとDestinationFolderが同じ場合はエラー
                    if (TargetFolder.CollectionName == DestinationFolder.CollectionName) {
                        Tools.Error("同じフォルダにはコピーまたは移動できません。");
                        return;
                    }
                    DestinationFolder.SetAutoProcessRuleDestinationFolder(TargetAutoProcessRule);
                }
                // 無限ループのチェック処理
                if (AutoProcessRule.CheckInfiniteLoop(TargetAutoProcessRule)) {
                    Tools.Error("コピー/移動処理の無限ループを検出しました。");
                    return;
                }
            }
            // IsPromptTemplateCheckedがTrueの場合はSelectedPromptItemを追加
            else if (IsPromptTemplateChecked) {
                if (SelectedPromptItem == null) {
                    Tools.Error("PromptTemplateを選択してください。");
                    return;
                }
                PromptAutoProcessItem promptAutoProcessItem = new(SelectedPromptItem.PromptItem);
                // OpenAIExecutionModeEnumを設定
                promptAutoProcessItem.Mode = OpenAIExecutionModeEnum;
                TargetAutoProcessRule.RuleAction = promptAutoProcessItem;
            }
            // IsPythonScriptCheckedがTrueの場合はSelectedScriptItemを追加
            else if (IsPythonScriptChecked) {
                if (SelectedScriptItem == null) {
                    Tools.Error("PythonScriptを選択してください。");
                    return;
                }
                TargetAutoProcessRule.RuleAction = new ScriptAutoProcessItem(SelectedScriptItem);
            }

            // LiteDBに保存
            TargetAutoProcessRule.Save();
            // ClipboardItemFolderにAutoProcessRuleIdを追加
            TargetFolder.AddAutoProcessRule(TargetAutoProcessRule);

            // AutoProcessRuleを更新したあとの処理を実行
            _AfterUpdate?.Invoke(TargetAutoProcessRule);

            // ウィンドウを閉じる
            if (parameter is System.Windows.Window window) {
                window.Close();
            }

        });
        // キャンセルボタンが押されたときの処理
        public SimpleDelegateCommand CancelButtonClickedCommand => new((parameter) => {
            // ウィンドウを閉じる
            if (parameter is System.Windows.Window window) {
                window.Close();
            }

        });
        // OnSelectedFolderChanged
        public void OnSelectedFolderChanged(ClipboardFolderViewModel? folder) {
            if (folder == null) {
                return;
            }
            // コピーor移動先が同じフォルダの場合はエラー
            if (folder.CollectionName == TargetFolder?.CollectionName) {
                Tools.Error("同じフォルダにはコピーまたは移動できません。");
                return;
            }// コピーor移動先が検索フォルダの場合はエラー
            if (folder.IsSearchFolder) {
                Tools.Error("検索フォルダにはコピーまたは移動できません。");
                return;
            }
            DestinationFolder = folder;

        }
        // OpenSelectDestinationFolderWindowCommand
        public SimpleDelegateCommand OpenSelectDestinationFolderWindowCommand => new((parameter) => {
            // フォルダが選択されたら、DestinationFolderに設定
            void FolderSelectedAction(ClipboardFolderViewModel folderViewModel) {
                DestinationFolder = folderViewModel;
            }
            FolderSelectWindow FolderSelectWindow = new ();
            FolderSelectWindowViewModel FolderSelectWindowViewModel = (FolderSelectWindowViewModel)FolderSelectWindow.DataContext;

            ClipboardFolderViewModel? rootFolderViewModel = MainWindowViewModel.RootFolderViewModel;
            if (rootFolderViewModel == null) {
                Tools.Error("RootFolderViewModelがNullです。");
                return;
            }
            FolderSelectWindowViewModel.Initialize(rootFolderViewModel, FolderSelectedAction);
            FolderSelectWindow.ShowDialog();
        });

        // OpenSelectTargetFolderWindowCommand
        public SimpleDelegateCommand OpenSelectTargetFolderWindowCommand => new((parameter) => {
            // フォルダが選択されたら、TargetFolderに設定
            void FolderSelectedAction(ClipboardFolderViewModel folderViewModel) {
                TargetFolder = folderViewModel;
            }
            FolderSelectWindow FolderSelectWindow = new();
            FolderSelectWindowViewModel FolderSelectWindowViewModel = (FolderSelectWindowViewModel)FolderSelectWindow.DataContext;

            ClipboardFolderViewModel? rootFolderViewModel = MainWindowViewModel.RootFolderViewModel;
            if (rootFolderViewModel == null) {
                Tools.Error("RootFolderViewModelがNullです。");
                return;
            }
            FolderSelectWindowViewModel.Initialize(rootFolderViewModel, FolderSelectedAction);
            FolderSelectWindow.ShowDialog();
        });

        public SimpleDelegateCommand AutoProcessItemSelectionChangedCommand => new(AutoProcessItemSelectionChangedCommandExecute);
        public void AutoProcessItemSelectionChangedCommandExecute(object parameter) {
            // ラジオボタンをIsBasicProcessChecked = trueにする
            IsBasicProcessChecked = true;
            OnPropertyChanged(nameof(IsBasicProcessChecked));

            if (SelectedAutoProcessItem == null) {
                return;
            }
            if (SelectedAutoProcessItem.IsCopyOrMoveOrMergeAction()) {
                FolderSelectionPanelEnabled = true;
            } else {
                FolderSelectionPanelEnabled = false;
            }
        }

        // OpenSelectPromptTemplateWindowCommand
        public SimpleDelegateCommand OpenSelectPromptTemplateWindowCommand => new((parameter) => {
            // ラジオボタンをIsPromptTemplateChecked = trueにする
            IsPromptTemplateChecked = true;
            OnPropertyChanged(nameof(IsPromptTemplateChecked));
            ListPromptTemplateWindow PromptTemplateSelectWindow = new();
            ListPromptTemplateWindowViewModel PromptTemplateSelectWindowViewModel = (ListPromptTemplateWindowViewModel)PromptTemplateSelectWindow.DataContext;
            PromptTemplateSelectWindowViewModel.Initialize(
                // PromptTemplateが選択されたら、PromptTemplateに設定
                ActionModeEum.Select, ((promptItemViewModel, mode) => {
                    SelectedPromptItem = promptItemViewModel;
                }));
            PromptTemplateSelectWindow.ShowDialog();
        });

        //OpenSelectScriptWindowCommand
        public SimpleDelegateCommand OpenSelectScriptWindowCommand => new((parameter) => {
            // ラジオボタンをIsPythonScriptChecked = trueにする
            IsPythonScriptChecked = true;
            OnPropertyChanged(nameof(IsPythonScriptChecked));
            ListPythonScriptWindow ScriptSelectWindow = new();
            ListPythonScriptWindowViewModel ScriptSelectWindowViewModel = (ListPythonScriptWindowViewModel)ScriptSelectWindow.DataContext;
            ScriptSelectWindowViewModel.Initialize( ListPythonScriptWindowViewModel.ActionModeEnum.Select, (scriptItem) => {
                SelectedScriptItem = scriptItem;
            });
            ScriptSelectWindow.ShowDialog();
        });

        // OpenAIExecutionModeSelectionChangeCommand
        public SimpleDelegateCommand OpenAIExecutionModeSelectionChangeCommand => new((parameter) => {
            // parameterがRoutedEventArgsでない場合はエラー
            if (parameter is not RoutedEventArgs routedEventArgs) {
                return;
            }
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択中のアイテムを取得
            var selectedItem = comboBox.SelectedItem;
            // 選択中のアイテムのインデックスを取得
            int selectedIndex = comboBox.SelectedIndex;
            // インデックスが0の場合はModeをNormalにする, 1の場合はModeをLangChainWithVectorDBにする.それ以外はエラー
            if (selectedIndex == 0) {
                OpenAIExecutionModeEnum = OpenAIExecutionModeEnum.Normal;
            } else if (selectedIndex == 1) {
                OpenAIExecutionModeEnum = OpenAIExecutionModeEnum.RAG;
            } else {
                return;
            }

        });
    }
}
