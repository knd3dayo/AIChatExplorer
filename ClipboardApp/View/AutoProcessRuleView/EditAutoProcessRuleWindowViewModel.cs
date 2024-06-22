
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.PythonScriptView;
using QAChat.Model;
using QAChat.View.PromptTemplateWindow;
using QAChat.View.VectorDBWindow;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using static QAChat.View.PromptTemplateWindow.ListPromptTemplateWindowViewModel;

namespace ClipboardApp.View.AutoProcessRuleView {
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
        private bool _IsAllItemsRuleChecked = false;
        public bool IsAllItemsRuleChecked {
            get {
                return _IsAllItemsRuleChecked;
            }
            set {
                _IsAllItemsRuleChecked = value;
                OnPropertyChanged(nameof(IsAllItemsRuleChecked));
            }
        }

        // すべてのアイテムに対してルールを有効にするかどうかの反対の値   
        private bool _IsNotAllItemsRuleChecked = false;
        public bool IsNotAllItemsRuleChecked {
            get {
                return _IsNotAllItemsRuleChecked;
            }
            set {
                _IsNotAllItemsRuleChecked = value;
                OnPropertyChanged(nameof(IsNotAllItemsRuleChecked));
            }
        }
        // アイテムがテキストの場合にルールを適用するかどうか
        public bool IsTextItemApplied { get; set; } = true;

        // テキストアイテムで、ルール適用対象となる最小テキスト行数
        private int _MinTextLineCount = -1;
        public string MinTextLineCount {
            get {
                // -1の場合は空を返す
                if (_MinTextLineCount == -1) {
                    return "";
                }
                return _MinTextLineCount.ToString();
            }
            set {
                // valueが空の場合は-1を設定
                if (string.IsNullOrEmpty(value)) {
                    _MinTextLineCount = -1;
                    OnPropertyChanged(nameof(MinTextLineCount));
                    return;
                }
                // valueが数値でない場合はエラー
                if (!int.TryParse(value.ToString(), out int intValue)) {
                    LogWrapper.Error("数値を入力してください。");
                    return;
                }
                _MinTextLineCount = intValue;
                OnPropertyChanged(nameof(MinTextLineCount));
            }
        }
        public int MinTextLineCountInt {
            get {
                return _MinTextLineCount;
            }
        }

        // テキストアイテムで、ルール適用対象となる最大テキスト行数 - 1
        private int _MaxTextLineCount = -1;
        public string MaxTextLineCount {
            get {
                // -1の場合は空を返す
                if (_MaxTextLineCount == -1) {
                    return "";
                }
                return _MaxTextLineCount.ToString();
            }
            set {
                // valueが空の場合は-1を設定
                if (string.IsNullOrEmpty(value)) {
                    _MaxTextLineCount = -1;
                    OnPropertyChanged(nameof(MaxTextLineCount));
                    return;
                }
                // valueが数値でない場合はエラー
                if (!int.TryParse(value.ToString(), out int intValue)) {
                    LogWrapper.Error("数値を入力してください。");
                    return;
                }

                _MaxTextLineCount = intValue;
                OnPropertyChanged(nameof(MaxTextLineCount));
            }
        }
        public int MaxTextLineCountInt {
            get {
                return _MaxTextLineCount;
            }
        }

        // アイテムが画像の場合にルールを適用するかどうか
        public bool IsImageItemApplied { get; set; } = true;

        // アイテムがファイルの場合にルールを適用するかどうか
        public bool IsFileItemApplied { get; set; } = true;

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
        // VectorDBItem
        private VectorDBItem? _selectedVectorDBItem;
        public VectorDBItem? SelectedVectorDBItem {
            get => _selectedVectorDBItem;
            set {
                _selectedVectorDBItem = value;
                OnPropertyChanged(nameof(SelectedVectorDBItem));
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

        // ベクトルDBに格納する場合のラジオボタンが選択中かどうか
        public bool IsStoreVectorDBChecked { get; set; } = false;

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
        public void Initialize(
            Mode mode, MainWindowViewModel? mainWindowViewModel, AutoProcessRule? autoProcessRule, Action<AutoProcessRule> afterUpdate) {
            if (mainWindowViewModel == null) {
                LogWrapper.Error("MainWindowViewModelがNullです。");
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
                        case AutoProcessRuleCondition.ConditionTypeEnum.AllItems:
                            IsAllItemsRuleChecked = true;
                            OnPropertyChanged(nameof(IsAllItemsRuleChecked));
                            break;

                        case AutoProcessRuleCondition.ConditionTypeEnum.DescriptionContains:
                            IsNotAllItemsRuleChecked = true;

                            IsDescriptionRuleChecked = true;
                            OnPropertyChanged(nameof(IsDescriptionRuleChecked));
                            Description = condition.Keyword;
                            OnPropertyChanged(nameof(Description));
                            break;
                        case AutoProcessRuleCondition.ConditionTypeEnum.ContentContains:
                            IsNotAllItemsRuleChecked = true;

                            IsContentRuleChecked = true;
                            OnPropertyChanged(nameof(IsContentRuleChecked));
                            Content = condition.Keyword;
                            OnPropertyChanged(nameof(Content));
                            break;
                        case AutoProcessRuleCondition.ConditionTypeEnum.SourceApplicationNameContains:
                            IsNotAllItemsRuleChecked = true;

                            IsSourceApplicationRuleChecked = true;
                            OnPropertyChanged(nameof(IsSourceApplicationRuleChecked));
                            SourceApplicationName = condition.Keyword;
                            OnPropertyChanged(nameof(SourceApplicationName));
                            break;
                        case AutoProcessRuleCondition.ConditionTypeEnum.SourceApplicationTitleContains:
                            IsNotAllItemsRuleChecked = true;

                            IsSourceApplicationTitleRuleChecked = true;
                            OnPropertyChanged(nameof(IsSourceApplicationTitleRuleChecked));
                            SourceApplicationTitle = condition.Keyword;
                            OnPropertyChanged(nameof(SourceApplicationTitle));
                            break;
                        case AutoProcessRuleCondition.ConditionTypeEnum.ContentTypeIs:
                            IsNotAllItemsRuleChecked = true;

                            if (condition.ContentTypes.Contains(ClipboardContentTypes.Text  )) {
                                IsTextItemApplied = true;
                                OnPropertyChanged(nameof(IsTextItemApplied));
                                MinTextLineCount = condition.MinLineCount.ToString();
                                MaxTextLineCount = condition.MaxLineCount.ToString();
                            }
                            if (condition.ContentTypes.Contains(ClipboardContentTypes.Image)) {
                                IsImageItemApplied = true;
                                OnPropertyChanged(nameof(IsImageItemApplied));
                            }
                            if (condition.ContentTypes.Contains(ClipboardContentTypes.Files)) {
                                IsFileItemApplied = true;
                                OnPropertyChanged(nameof(IsFileItemApplied));
                            }
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
                    if (promptAutoProcessItem.PromptItemId == LiteDB.ObjectId.Empty) {
                        return;
                    }
                    IsPromptTemplateChecked = true;
                    // PromptItemを取得
                    PromptItem promptItem = PromptItem.GetPromptItemById(promptAutoProcessItem.PromptItemId);
                    SelectedPromptItem = new PromptItemViewModel(promptItem);
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
                // VectorDBAutoProcessItemの場合
                else if (TargetAutoProcessRule.RuleAction is VectorDBAutoProcessItem vectorDBAutoProcessItem) {
                    if (vectorDBAutoProcessItem.VectorDBItemId == LiteDB.ObjectId.Empty) {
                        return;
                    }
                    IsStoreVectorDBChecked = true;
                    SelectedVectorDBItem = VectorDBItem.GetItemById(vectorDBAutoProcessItem.VectorDBItemId);
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
        public SimpleDelegateCommand<Window> OKButtonClickedCommand => new((window) => {
            // TargetFolderがNullの場合はエラー
            if (TargetFolder == null) {
                LogWrapper.Error("フォルダが選択されていません。");
                return;
            }
            // RuleNameが空の場合はエラー
            if (string.IsNullOrEmpty(RuleName)) {
                LogWrapper.Error("ルール名を入力してください。");
                return;
            }
            // SelectedAutoProcessItemが空の場合はエラー
            if (SelectedAutoProcessItem == null) {
                LogWrapper.Error("アクションを選択してください。");
                return;
            }
            // 新規作成
            if (CurrentMode == Mode.Create) {
                TargetAutoProcessRule = new AutoProcessRule(RuleName);
            }
            // 編集
            else {
                if (TargetAutoProcessRule == null) {
                    LogWrapper.Error("編集対象のルールが見つかりません。");
                    return;
                }
                TargetAutoProcessRule.Conditions.Clear();
                TargetAutoProcessRule.RuleName = RuleName;
            }

            // IsAutoProcessRuleEnabledがTrueの場合はIsEnabledをTrueにする
            TargetAutoProcessRule.IsEnabled = IsAutoProcessRuleEnabled;

            // TargetFolderを設定
            TargetAutoProcessRule.TargetFolder = TargetFolder.ClipboardItemFolder;
            // IsAllItemsRuleCheckedがTrueの場合は条件を追加
            if (IsAllItemsRuleChecked) {
                // AllItemsを条件に追加
                TargetAutoProcessRule.Conditions.Add(new AutoProcessRuleCondition(AutoProcessRuleCondition.ConditionTypeEnum.AllItems, ""));
            } else {

                // IsDescriptionRuleCheckedがTrueの場合は条件を追加
                if (IsDescriptionRuleChecked) {
                    // Descriptionを条件に追加

                    TargetAutoProcessRule.Conditions.Add(new AutoProcessRuleCondition(AutoProcessRuleCondition.ConditionTypeEnum.DescriptionContains, Description));
                }
                // IsContentRuleCheckedがTrueの場合は条件を追加
                if (IsContentRuleChecked) {
                    // Contentを条件に追加
                    TargetAutoProcessRule.Conditions.Add(new AutoProcessRuleCondition(AutoProcessRuleCondition.ConditionTypeEnum.ContentContains, Content));
                }
                // IsSourceApplicationRuleCheckedがTrueの場合は条件を追加
                if (IsSourceApplicationRuleChecked) {
                    // SourceApplicationNameを条件に追加
                    TargetAutoProcessRule.Conditions.Add(new AutoProcessRuleCondition(AutoProcessRuleCondition.ConditionTypeEnum.SourceApplicationNameContains, SourceApplicationName));
                }
                // IsSourceApplicationTitleRuleCheckedがTrueの場合は条件を追加
                if (IsSourceApplicationTitleRuleChecked) {
                    // SourceApplicationTitleを条件に追加
                    TargetAutoProcessRule.Conditions.Add(new AutoProcessRuleCondition(AutoProcessRuleCondition.ConditionTypeEnum.SourceApplicationTitleContains, SourceApplicationTitle));
                }
                // ContentTypeの処理
                List<ClipboardContentTypes> contentTypes = new List<ClipboardContentTypes>();
                // IsTextItemAppliedがTrueの場合は条件を追加
                if (IsTextItemApplied) {
                    // TextItemを条件に追加
                    contentTypes.Add(ClipboardContentTypes.Text);
                }
                // IsImageItemAppliedがTrueの場合は条件を追加
                if (IsImageItemApplied) {
                    // ImageItemを条件に追加
                    contentTypes.Add(ClipboardContentTypes.Image);
                }
                // IsFileItemAppliedがTrueの場合は条件を追加
                if (IsFileItemApplied) {
                    // FileItemを条件に追加
                    contentTypes.Add(ClipboardContentTypes.Files);
                }
                // ContentTypeIsを条件に追加
                TargetAutoProcessRule.Conditions.Add(new AutoProcessRuleCondition( contentTypes, MinTextLineCountInt, MaxTextLineCountInt));

            }
            // アクションを追加
            // IsBasicProcessCheckedがTrueの場合はSelectedAutoProcessItemを追加
            if (IsBasicProcessChecked) {
                TargetAutoProcessRule.RuleAction = SelectedAutoProcessItem.AutoProcessItem;
                // アクションタイプがCopyToFolderまたは MoveToFolderの場合はDestinationFolderを設定
                if (SelectedAutoProcessItem.IsCopyOrMoveOrMergeAction()) {
                    if (DestinationFolder == null) {
                        LogWrapper.Error("コピーまたは移動先のフォルダを選択してください。");
                        return;
                    }
                    // TargetFolderとDestinationFolderが同じ場合はエラー
                    if (TargetFolder.ClipboardItemFolder.Id == DestinationFolder.ClipboardItemFolder.Id) {
                        LogWrapper.Error("同じフォルダにはコピーまたは移動できません。");
                        return;
                    }
                    TargetAutoProcessRule.DestinationFolder = DestinationFolder.ClipboardItemFolder;
                }
                // 無限ループのチェック処理
                if (AutoProcessRule.CheckInfiniteLoop(TargetAutoProcessRule)) {
                    LogWrapper.Error("コピー/移動処理の無限ループを検出しました。");
                    return;
                }
            }
            // IsPromptTemplateCheckedがTrueの場合はSelectedPromptItemを追加
            else if (IsPromptTemplateChecked) {
                if (SelectedPromptItem == null) {
                    LogWrapper.Error("PromptTemplateを選択してください。");
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
                    LogWrapper.Error("PythonScriptを選択してください。");
                    return;
                }
                TargetAutoProcessRule.RuleAction = new ScriptAutoProcessItem(SelectedScriptItem);
            }
            // IsStoreVectorDBCheckedがTrueの場合はSelectedVectorDBItemを追加
            if (IsStoreVectorDBChecked) {
                if (SelectedVectorDBItem == null) {
                    LogWrapper.Error("VectorDBを選択してください。");
                    return;
                }
                TargetAutoProcessRule.RuleAction = new VectorDBAutoProcessItem(SelectedVectorDBItem);
            }

            // LiteDBに保存
            TargetAutoProcessRule.Save();
            // ClipboardItemFolderにAutoProcessRuleIdを追加
            TargetFolder.ClipboardItemFolder.AddAutoProcessRule(TargetAutoProcessRule);

            // AutoProcessRuleを更新したあとの処理を実行
            _AfterUpdate?.Invoke(TargetAutoProcessRule);

            // ウィンドウを閉じる
            window.Close();

        });
        // キャンセルボタンが押されたときの処理
        public SimpleDelegateCommand<Window> CancelButtonClickedCommand => new((window) => {
            // ウィンドウを閉じる
            window.Close();

        });
        // OnSelectedFolderChanged
        public void OnSelectedFolderChanged(ClipboardFolderViewModel? folder) {
            if (folder == null) {
                return;
            }
            // コピーor移動先が同じフォルダの場合はエラー
            if (folder.ClipboardItemFolder.Id == TargetFolder?.ClipboardItemFolder.Id) {
                LogWrapper.Error("同じフォルダにはコピーまたは移動できません。");
                return;
            }// コピーor移動先が検索フォルダの場合はエラー
            if (folder.IsSearchFolder) {
                LogWrapper.Error("検索フォルダにはコピーまたは移動できません。");
                return;
            }
            DestinationFolder = folder;

        }
        // OpenSelectDestinationFolderWindowCommand
        public SimpleDelegateCommand<object> OpenSelectDestinationFolderWindowCommand => new((parameter) => {
            // フォルダが選択されたら、DestinationFolderに設定
            ClipboardFolderViewModel? rootFolderViewModel = MainWindowViewModel.RootFolderViewModel;
            if (rootFolderViewModel == null) {
                LogWrapper.Error("RootFolderViewModelがNullです。");
                return;
            }
            FolderSelectWindow.OpenFolderSelectWindow(rootFolderViewModel, (folderViewModel) => {
                DestinationFolder = folderViewModel;
            });
        });

        // OpenSelectTargetFolderWindowCommand
        public SimpleDelegateCommand<object> OpenSelectTargetFolderWindowCommand => new((parameter) => {
            // フォルダが選択されたら、TargetFolderに設定
            ClipboardFolderViewModel? rootFolderViewModel = MainWindowViewModel.RootFolderViewModel;
            if (rootFolderViewModel == null) {
                LogWrapper.Error("RootFolderViewModelがNullです。");
                return;
            }
            FolderSelectWindow.OpenFolderSelectWindow(rootFolderViewModel, (folderViewModel) => {
                TargetFolder = folderViewModel;
            });
        });

        public SimpleDelegateCommand<object> AutoProcessItemSelectionChangedCommand => new((parameter) => {
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
        });

        // OpenSelectPromptTemplateWindowCommand
        public SimpleDelegateCommand<object> OpenSelectPromptTemplateWindowCommand => new((parameter) => {
            // ラジオボタンをIsPromptTemplateChecked = trueにする
            IsPromptTemplateChecked = true;
            OnPropertyChanged(nameof(IsPromptTemplateChecked));

            ListPromptTemplateWindow.OpenListPromptTemplateWindow(
                // PromptTemplateが選択されたら、PromptTemplateに設定
                ActionModeEum.Select, ((promptItemViewModel, mode) => {
                    SelectedPromptItem = promptItemViewModel;
                }));
        });

        //OpenSelectScriptWindowCommand
        public SimpleDelegateCommand<object> OpenSelectScriptWindowCommand => new((parameter) => {
            // ラジオボタンをIsPythonScriptChecked = trueにする
            IsPythonScriptChecked = true;
            OnPropertyChanged(nameof(IsPythonScriptChecked));
            ListPythonScriptWindow.OpenListPythonScriptWindow(ListPythonScriptWindowViewModel.ActionModeEnum.Select, (scriptItem) => {
                SelectedScriptItem = scriptItem;
            });
        });

        // OpenSelectVectorDBWindowCommand
        public SimpleDelegateCommand<object> OpenSelectVectorDBWindowCommand => new((parameter) => {
            // ラジオボタンをIsStoreVectorDBChecked = trueにする
            IsStoreVectorDBChecked = true;
            OnPropertyChanged(nameof(IsStoreVectorDBChecked));

            // ベクトルDB一覧画面を表示する
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, (vectorDBItem) => {
                // ベクトルDBを選択したら、SelectedVectorDBItemに設定
                SelectedVectorDBItem = vectorDBItem;

            });


        });
        // OpenAIExecutionModeSelectionChangeCommand
        public SimpleDelegateCommand<RoutedEventArgs> OpenAIExecutionModeSelectionChangeCommand => new((routedEventArgs) => {
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
