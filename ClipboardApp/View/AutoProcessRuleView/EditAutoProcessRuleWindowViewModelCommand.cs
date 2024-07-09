
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.PythonScriptView;
using PythonAILib.Model;
using QAChat.Model;
using QAChat.View.PromptTemplateWindow;
using QAChat.View.VectorDBWindow;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using QAChat.ViewModel;
using ClipboardApp.ViewModel;

namespace ClipboardApp.View.AutoProcessRuleView
{
    public partial class EditAutoProcessRuleWindowViewModel : MyWindowViewModel {

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
            TargetFolder.ClipboardItemFolder.AutoProcessRuleIds.Add(TargetAutoProcessRule.Id);
            // ClipboardItemFolderを保存
            TargetFolder.ClipboardItemFolder.Save();

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
            }// コピーor移動先が標準のフォルダ以外の場合はエラー
            if (folder.ClipboardItemFolder.FolderType != ClipboardFolder.FolderTypeEnum.Normal) {
                LogWrapper.Error("標準フォルダ以外にはコピーまたは移動できません。");
                return;
            }
            DestinationFolder = folder;

        }
        // OpenSelectDestinationFolderWindowCommand
        public SimpleDelegateCommand<object> OpenSelectDestinationFolderWindowCommand => new((parameter) => {
            // フォルダが選択されたら、DestinationFolderに設定
            ClipboardFolderViewModel? rootFolderViewModel = MainWindowViewModel?.RootFolderViewModel;
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
            ClipboardFolderViewModel? rootFolderViewModel = MainWindowViewModel?.RootFolderViewModel;
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
                ListPromptTemplateWindowViewModel.ActionModeEum.Select, ((promptItemViewModel, mode) => {
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
