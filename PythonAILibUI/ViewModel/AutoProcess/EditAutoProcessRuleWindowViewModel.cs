
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Content;
using PythonAILib.Model.File;
using PythonAILib.Model.Folder;
using PythonAILib.Model.Prompt;
using QAChat.Model;
using QAChat.View.Folder;
using QAChat.View.PromptTemplate;
using QAChat.ViewModel.Folder;
using QAChat.ViewModel.PromptTemplate;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.AutoProcess {
    public class EditAutoProcessRuleWindowViewModel : QAChatViewModelBase {

        // 初期化
        public EditAutoProcessRuleWindowViewModel(AutoProcessRule autoProcessRule, ObservableCollection<ContentFolderViewModel> rootFolderViewModels, Action<AutoProcessRule> afterUpdate) {
            TargetAutoProcessRule = autoProcessRule;
            IsAutoProcessRuleEnabled = autoProcessRule.IsEnabled;
            _AfterUpdate = afterUpdate;
            TargetFolder = ContentFolder.GetFolderById<ContentFolder>(autoProcessRule.TargetFolderId);
            // DestinationIdに一致するフォルダを取得
            DestinationFolder = ContentFolder.GetFolderById<ContentFolder>(autoProcessRule.DestinationFolderId);

            // RootFolderViewModelを設定
            RootFolderViewModels = rootFolderViewModels;

            LoadConditions();

        }

        // RootFolderViewModel
        public ObservableCollection<ContentFolderViewModel> RootFolderViewModels { get; set; } = [];

        private void LoadConditions() {
            // autoProcessRuleがNullでない場合は初期化
            if (TargetAutoProcessRule.RuleAction == null) {
                return;
            }
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

                        if (condition.ContentTypes.Contains(ContentTypes.ContentItemTypes.Text)) {
                            IsTextItemApplied = true;
                            OnPropertyChanged(nameof(IsTextItemApplied));
                            MinTextLineCount = condition.MinLineCount.ToString();
                            MaxTextLineCount = condition.MaxLineCount.ToString();
                        }
                        if (condition.ContentTypes.Contains(ContentTypes.ContentItemTypes.Image)) {
                            IsImageItemApplied = true;
                            OnPropertyChanged(nameof(IsImageItemApplied));
                        }
                        if (condition.ContentTypes.Contains(ContentTypes.ContentItemTypes.Files)) {
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

            OnPropertyChanged(nameof(Conditions));

        }

        // ルール適用対象のClipboardItemFolder
        private ContentFolder? _ClipboardItemFolder = null;
        public ContentFolder? TargetFolder {
            get {
                return _ClipboardItemFolder;
            }
            set {
                _ClipboardItemFolder = value;
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
            = new ObservableCollection<AutoProcessItemViewModel>(AutoProcessItemViewModel.SystemAutoProcesses);

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
                    LogWrapper.Error(StringResources.EnterANumber);
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
                    LogWrapper.Error(StringResources.EnterANumber);
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
        private ContentFolder? _DestinationFolder = null;
        public ContentFolder? DestinationFolder {
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
        public SimpleDelegateCommand<Window> OKButtonClickedCommand => new((window) => {
            // TargetFolderがNullの場合はエラー
            if (TargetFolder == null) {
                LogWrapper.Error(StringResources.FolderNotSelected);
                return;
            }
            // RuleNameが空の場合はエラー
            if (string.IsNullOrEmpty(RuleName)) {
                LogWrapper.Error(StringResources.EnterRuleName);
                return;
            }
            // SelectedAutoProcessItemが空の場合はエラー
            if (SelectedAutoProcessItem == null) {
                LogWrapper.Error(StringResources.SelectAction);
                return;
            }

            // 編集
            else {
                if (TargetAutoProcessRule == null) {
                    LogWrapper.Error(StringResources.RuleNotFound);
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
                List<ContentTypes.ContentItemTypes> contentTypes = new List<ContentTypes.ContentItemTypes>();
                // IsTextItemAppliedがTrueの場合は条件を追加
                if (IsTextItemApplied) {
                    // TextItemを条件に追加
                    contentTypes.Add(ContentTypes.ContentItemTypes.Text);
                }
                // IsImageItemAppliedがTrueの場合は条件を追加
                if (IsImageItemApplied) {
                    // ImageItemを条件に追加
                    contentTypes.Add(ContentTypes.ContentItemTypes.Image);
                }
                // IsFileItemAppliedがTrueの場合は条件を追加
                if (IsFileItemApplied) {
                    // FileItemを条件に追加
                    contentTypes.Add(ContentTypes.ContentItemTypes.Files);
                }
                // ContentTypeIsを条件に追加
                TargetAutoProcessRule.Conditions.Add(new AutoProcessRuleCondition(contentTypes, MinTextLineCountInt, MaxTextLineCountInt));

            }
            // アクションを追加
            // IsBasicProcessCheckedがTrueの場合はSelectedAutoProcessItemを追加
            if (IsBasicProcessChecked) {
                TargetAutoProcessRule.RuleAction = SelectedAutoProcessItem.AutoProcessItem;
                // アクションタイプがCopyToFolderまたは MoveToFolderの場合はDestinationFolderを設定
                if (SelectedAutoProcessItem.IsCopyOrMoveOrMergeAction()) {
                    if (DestinationFolder == null) {
                        LogWrapper.Error(StringResources.SelectCopyOrMoveTargetFolder);
                        return;
                    }
                    // TargetFolderとDestinationFolderが同じ場合はエラー
                    if (TargetFolder.Id == DestinationFolder.Id) {
                        LogWrapper.Error(StringResources.CannotCopyOrMoveToTheSameFolder);
                        return;
                    }
                    TargetAutoProcessRule.DestinationFolderId = DestinationFolder.Id;
                }
                // 無限ループのチェック処理
                if (AutoProcessRule.CheckInfiniteLoop(TargetAutoProcessRule)) {
                    LogWrapper.Error(StringResources.DetectedAnInfiniteLoopInCopyMoveProcessing);
                    return;
                }
            }
            // IsPromptTemplateCheckedがTrueの場合はSelectedPromptItemを追加
            else if (IsPromptTemplateChecked) {
                if (SelectedPromptItem == null) {
                    LogWrapper.Error(StringResources.SelectPromptTemplate);
                    return;
                }
                // キャスト
                PromptItem promptItem = SelectedPromptItem.PromptItem;
                PromptAutoProcessItem promptAutoProcessItem = new(promptItem);

                // OpenAIExecutionModeEnumを設定
                promptAutoProcessItem.Mode = OpenAIExecutionModeEnum;
                TargetAutoProcessRule.RuleAction = promptAutoProcessItem;
            }

            // LiteDBに保存
            TargetAutoProcessRule.Save();

            // AutoProcessRuleを更新したあとの処理を実行
            _AfterUpdate?.Invoke(TargetAutoProcessRule);

            // ウィンドウを閉じる
            window.Close();

        });

        // OnSelectedFolderChanged
        public void OnSelectedFolderChanged(ContentFolder? folder) {
            if (folder == null) {
                return;
            }

            // コピーor移動先が同じフォルダの場合はエラー
            if (folder.Id == TargetFolder?.Id) {
                LogWrapper.Error(StringResources.CannotCopyOrMoveToTheSameFolder);
                return;
            }// コピーor移動先が標準のフォルダ以外の場合はエラー
            if (folder.FolderType != FolderTypeEnum.Normal) {
                LogWrapper.Error(StringResources.CannotCopyOrMoveToNonStandardFolders);
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
