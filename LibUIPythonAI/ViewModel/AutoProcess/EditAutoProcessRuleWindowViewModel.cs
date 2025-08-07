
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LibPythonAI.Data;
using LibPythonAI.Model.AutoProcess;
using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Folder;
using LibUIPythonAI.View.PromptTemplate;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.PromptTemplate;

namespace LibUIPythonAI.ViewModel.AutoProcess {
    public class EditAutoProcessRuleWindowViewModel : CommonViewModelBase {

        // 初期化
        public EditAutoProcessRuleWindowViewModel(
            AutoProcessRule autoProcessRule, 
            ObservableCollection<ContentFolderViewModel> rootFolderViewModels, 
            Action<AutoProcessRule> afterUpdate) {
            TargetAutoProcessRule = autoProcessRule;
            IsAutoProcessRuleEnabled = autoProcessRule.IsEnabled;
            _AfterUpdate = afterUpdate;

            // RootFolderViewModelを設定
            RootFolderViewModels = rootFolderViewModels;
            Task.Run(async () => {
                TargetFolder = await autoProcessRule.GetTargetFolder();
                DestinationFolder = await autoProcessRule.GetDestinationFolder();
                await LoadConditions();
            });

        }

        // RootFolderViewModel
        public ObservableCollection<ContentFolderViewModel> RootFolderViewModels { get; set; } = [];

        private async Task LoadConditions() {
            // autoProcessRuleがNullでない場合は初期化
            if (TargetAutoProcessRule.GetRuleAction == null) {
                return;
            }
            RuleName = TargetAutoProcessRule.RuleName;
            OnPropertyChanged(nameof(RuleName));
            Conditions = new ObservableCollection<AutoProcessRuleCondition>(TargetAutoProcessRule.Conditions);
            var ruleAction = await TargetAutoProcessRule.GetRuleAction();
            if (ruleAction != null) {
                SelectedAutoProcessItem = new AutoProcessItemViewModel(ruleAction);
            }

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

                        if (condition.ContentTypes.Contains(ContentItemTypes.ContentItemTypeEnum.Text)) {
                            IsTextItemApplied = true;
                            OnPropertyChanged(nameof(IsTextItemApplied));
                            MinTextLineCount = condition.MinLineCount.ToString();
                            MaxTextLineCount = condition.MaxLineCount.ToString();
                        }
                        if (condition.ContentTypes.Contains(ContentItemTypes.ContentItemTypeEnum.Image)) {
                            IsImageItemApplied = true;
                            OnPropertyChanged(nameof(IsImageItemApplied));
                        }
                        if (condition.ContentTypes.Contains(ContentItemTypes.ContentItemTypeEnum.Files)) {
                            IsFileItemApplied = true;
                            OnPropertyChanged(nameof(IsFileItemApplied));
                        }
                        break;

                }
            }
            // DestinationFolderが設定されている場合はFolderSelectionPanelEnabledをTrueにする
            if (DestinationFolder != null) {
                FolderSelectionPanelEnabled = true;
            }
            // PromptAutoProcessItemの場合
            if (ruleAction is PromptAutoProcessItem promptAutoProcessItem) {
                if (promptAutoProcessItem.PromptItemEntity == null) {
                    return;
                }
                IsPromptTemplateChecked = true;
                // PromptItemを取得
                var promptItem = PromptItem.GetPromptItemById(promptAutoProcessItem.PromptItemEntity.Id);
                if (promptItem == null) {
                    return;
                }
                SelectedPromptItem = new PromptItemViewModel(promptItem);
                // OpenAIExecutionModeEnumの値からOpenAIExecutionModeSelectedIndexを設定
                OpenAIExecutionModeSelectedIndex = (int)promptAutoProcessItem.Mode;

            }

            OnPropertyChanged(nameof(Conditions));

        }

        // ルール適用対象のApplicationItemFolder
        private ContentFolderWrapper? _ApplicationItemFolder = null;
        public ContentFolderWrapper? TargetFolder {
            get {
                return _ApplicationItemFolder;
            }
            set {
                _ApplicationItemFolder = value;
                OnPropertyChanged(nameof(TargetFolder));
            }
        }

        // 編集対象の自動処理ルール
        AutoProcessRule TargetAutoProcessRule { get; set; }

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
            = new (AutoProcessItemViewModel.SystemAutoProcesses);

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
                if (value.IsCopyOrMoveAction()) {
                    FolderSelectionPanelEnabled = true;
                } else {
                    FolderSelectionPanelEnabled = false;
                }
            }
        }

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
        // テキストアイテムで、ルール適用対象となる最小・最大テキスト行数
        private int _MinTextLineCount = -1;
        private int _MaxTextLineCount = -1;

        public string MinTextLineCount {
            get => IntToString(_MinTextLineCount);
            set => SetLineCount(ref _MinTextLineCount, value, nameof(MinTextLineCount));
        }
        public int MinTextLineCountInt => _MinTextLineCount;

        public string MaxTextLineCount {
            get => IntToString(_MaxTextLineCount);
            set => SetLineCount(ref _MaxTextLineCount, value, nameof(MaxTextLineCount));
        }
        public int MaxTextLineCountInt => _MaxTextLineCount;

        // 共通化メソッド
        private string IntToString(int value)
        {
            return value == -1 ? "" : value.ToString();
        }

        private void SetLineCount(ref int backingField, string value, string propertyName)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (backingField != -1)
                {
                    backingField = -1;
                    OnPropertyChanged(propertyName);
                }
                return;
            }
            if (!int.TryParse(value, out int intValue))
            {
                LogWrapper.Error(CommonStringResources.Instance.EnterANumber);
                return;
            }
            if (backingField != intValue)
            {
                backingField = intValue;
                OnPropertyChanged(propertyName);
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
        private ContentFolderWrapper? _DestinationFolder = null;
        public ContentFolderWrapper? DestinationFolder {
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

        // ---　コマンド 
        // OKボタンが押されたときの処理
        public SimpleDelegateCommand<Window> OKButtonClickedCommand => new( async (window) => {
            // TargetFolderがNullの場合はエラー
            if (TargetFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
                return;
            }
            // RuleNameが空の場合はエラー
            if (string.IsNullOrEmpty(RuleName)) {
                LogWrapper.Error(CommonStringResources.Instance.EnterRuleName);
                return;
            }
            // SelectedAutoProcessItemが空の場合はエラー
            if (SelectedAutoProcessItem == null) {
                LogWrapper.Error(CommonStringResources.Instance.SelectAction);
                return;
            }

            // 編集
            else {
                if (TargetAutoProcessRule == null) {
                    LogWrapper.Error(CommonStringResources.Instance.RuleNotFound);
                    return;
                }
                TargetAutoProcessRule.Conditions.Clear();
                TargetAutoProcessRule.RuleName = RuleName;
            }

            // IsAutoProcessRuleEnabledがTrueの場合はIsEnabledをTrueにする
            TargetAutoProcessRule.IsEnabled = IsAutoProcessRuleEnabled;

            // TargetFolderを設定
            TargetAutoProcessRule.TargetFolderId = TargetFolder.Id;
            // IsAllItemsRuleCheckedがTrueの場合は条件を追加
            if (IsAllItemsRuleChecked) {
                // AllItemsを条件に追加
                AutoProcessRuleCondition autoProcessRuleConditionEntity = new() {
                    ConditionType = AutoProcessRuleCondition.ConditionTypeEnum.AllItems,
                };
                TargetAutoProcessRule.Conditions.Add(autoProcessRuleConditionEntity);
            } else {
                AutoProcessRuleCondition autoProcessRuleConditionEntity;
                // IsDescriptionRuleCheckedがTrueの場合は条件を追加
                if (IsDescriptionRuleChecked) {
                    // Descriptionを条件に追加
                    autoProcessRuleConditionEntity = new() {
                        ConditionType = AutoProcessRuleCondition.ConditionTypeEnum.DescriptionContains,
                        Keyword = Description
                    };

                    TargetAutoProcessRule.Conditions.Add(autoProcessRuleConditionEntity);
                }
                // IsContentRuleCheckedがTrueの場合は条件を追加
                if (IsContentRuleChecked) {
                    // Contentを条件に追加
                    autoProcessRuleConditionEntity = new() {
                        ConditionType = AutoProcessRuleCondition.ConditionTypeEnum.ContentContains,
                        Keyword = Content
                    };
                    TargetAutoProcessRule.Conditions.Add(autoProcessRuleConditionEntity);
                }
                // IsSourceApplicationRuleCheckedがTrueの場合は条件を追加
                if (IsSourceApplicationRuleChecked) {
                    // SourceApplicationNameを条件に追加
                    autoProcessRuleConditionEntity = new() {
                        ConditionType = AutoProcessRuleCondition.ConditionTypeEnum.SourceApplicationNameContains,
                        Keyword = SourceApplicationName
                    };
                    TargetAutoProcessRule.Conditions.Add(autoProcessRuleConditionEntity);
                }
                // IsSourceApplicationTitleRuleCheckedがTrueの場合は条件を追加
                if (IsSourceApplicationTitleRuleChecked) {
                    // SourceApplicationTitleを条件に追加
                    autoProcessRuleConditionEntity = new() {
                        ConditionType = AutoProcessRuleCondition.ConditionTypeEnum.SourceApplicationTitleContains,
                        Keyword = SourceApplicationTitle
                    };
                    TargetAutoProcessRule.Conditions.Add(autoProcessRuleConditionEntity);
                }
                // ContentTypeの処理
                List<ContentItemTypes.ContentItemTypeEnum> contentTypes = [];
                // IsTextItemAppliedがTrueの場合は条件を追加
                if (IsTextItemApplied) {
                    // TextItemを条件に追加
                    contentTypes.Add(ContentItemTypes.ContentItemTypeEnum.Text);
                }
                // IsImageItemAppliedがTrueの場合は条件を追加
                if (IsImageItemApplied) {
                    // ImageItemを条件に追加
                    contentTypes.Add(ContentItemTypes.ContentItemTypeEnum.Image);
                }
                // IsFileItemAppliedがTrueの場合は条件を追加
                if (IsFileItemApplied) {
                    // FileItemを条件に追加
                    contentTypes.Add(ContentItemTypes.ContentItemTypeEnum.Files);
                }
                // ContentTypeIsを条件に追加
                autoProcessRuleConditionEntity = new() {
                    ContentTypes = contentTypes,
                    MaxLineCount = MaxTextLineCountInt,
                    MinLineCount = MinTextLineCountInt
                };

                TargetAutoProcessRule.Conditions.Add(autoProcessRuleConditionEntity);

            }
            // アクションを追加
            // IsBasicProcessCheckedがTrueの場合はSelectedAutoProcessItemを追加
            if (IsBasicProcessChecked) {
                TargetAutoProcessRule.SetRuleAction( SelectedAutoProcessItem.AutoProcessItem);
                // アクションタイプがCopyToFolderまたは MoveToFolderの場合はDestinationFolderを設定
                if (SelectedAutoProcessItem.IsCopyOrMoveAction()) {
                    if (DestinationFolder == null) {
                        LogWrapper.Error(CommonStringResources.Instance.SelectCopyOrMoveTargetFolder);
                        return;
                    }
                    // TargetFolderとDestinationFolderが同じ場合はエラー
                    if (TargetFolder.Id == DestinationFolder.Id) {
                        LogWrapper.Error(CommonStringResources.Instance.CannotCopyOrMoveToTheSameFolder);
                        return;
                    }
                    TargetAutoProcessRule.DestinationFolderId = DestinationFolder.Id;
                }
                // 無限ループのチェック処理
                var checkResult = await AutoProcessRule.CheckInfiniteLoop(TargetAutoProcessRule);
                if (checkResult) {
                    LogWrapper.Error(CommonStringResources.Instance.DetectedAnInfiniteLoopInCopyMoveProcessing);
                    return;
                }
            }
            // IsPromptTemplateCheckedがTrueの場合はSelectedPromptItemを追加
            else if (IsPromptTemplateChecked) {
                if (SelectedPromptItem == null) {
                    LogWrapper.Error(CommonStringResources.Instance.SelectPromptTemplate);
                    return;
                }
                // キャスト
                PromptItem promptItem = SelectedPromptItem.PromptItem;
                PromptAutoProcessItem promptAutoProcessItem = new(promptItem);

                // OpenAIExecutionModeEnumを設定
                promptAutoProcessItem.Mode = OpenAIExecutionModeEnum;
                TargetAutoProcessRule.SetRuleAction(promptAutoProcessItem);
            }

            // LiteDBに保存
            TargetAutoProcessRule.SaveAsync();

            // AutoProcessRuleを更新したあとの処理を実行
            _AfterUpdate?.Invoke(TargetAutoProcessRule);

            // ウィンドウを閉じる
            window.Close();

        });

        // OnSelectedFolderChanged
        public void OnSelectedFolderChanged(ContentFolderWrapper? folder) {
            if (folder == null) {
                return;
            }

            // コピーor移動先が同じフォルダの場合はエラー
            if (folder.Id == TargetFolder?.Id) {
                LogWrapper.Error(CommonStringResources.Instance.CannotCopyOrMoveToTheSameFolder);
                return;
            }
            DestinationFolder = folder;

        }
        // OpenSelectDestinationFolderWindowCommand
        public SimpleDelegateCommand<object> OpenSelectDestinationFolderWindowCommand => new((parameter) => {
            // フォルダが選択されたら、DestinationFolderに設定
            FolderSelectWindow.OpenFolderSelectWindow(RootFolderViewModels, (folderViewModel, finished) => {
                if (finished) {
                    DestinationFolder = folderViewModel.Folder;
                }
            });
        });

        // OpenSelectTargetFolderWindowCommand
        public SimpleDelegateCommand<object> OpenSelectTargetFolderWindowCommand => new((parameter) => {
            FolderSelectWindow.OpenFolderSelectWindow(RootFolderViewModels, (folderViewModel, finished) => {
                if (finished) {
                    TargetFolder = folderViewModel.Folder;
                }
            });
        });

        public SimpleDelegateCommand<object> AutoProcessItemSelectionChangedCommand => new((parameter) => {
            // ラジオボタンをIsBasicProcessChecked = trueにする
            IsBasicProcessChecked = true;
            OnPropertyChanged(nameof(IsBasicProcessChecked));

            if (SelectedAutoProcessItem == null) {
                return;
            }
            if (SelectedAutoProcessItem.IsCopyOrMoveAction()) {
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
                ListPromptTemplateWindowViewModel.ActionModeEum.Select, (promptItemViewModel, mode) => {
                    SelectedPromptItem = promptItemViewModel;
                });
        });

        // OpenAIExecutionModeSelectionChangeCommand
        public SimpleDelegateCommand<RoutedEventArgs> OpenAIExecutionModeSelectionChangeCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択中のアイテムを取得
            var selectedItem = comboBox.SelectedItem;
            // 選択中のアイテムのインデックスを取得
            int selectedIndex = comboBox.SelectedIndex;

            if (selectedIndex == 0) {
                OpenAIExecutionModeEnum = OpenAIExecutionModeEnum.Normal;
            } else if (selectedIndex == 1) {
                OpenAIExecutionModeEnum = OpenAIExecutionModeEnum.AutoGenGroupChat;
            } else {
                return;
            }

        });

    }
}
