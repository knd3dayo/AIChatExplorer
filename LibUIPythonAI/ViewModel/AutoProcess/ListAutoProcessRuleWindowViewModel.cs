using System.Collections.ObjectModel;
using System.Windows;
using PythonAILib.Model.AutoProcess;
using QAChat.Model;
using QAChat.View.AutoProcessRule;
using QAChat.ViewModel.Folder;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.AutoProcess {
    public class ListAutoProcessRuleWindowViewModel : QAChatViewModelBase {

        // ルールの一覧
        public ObservableCollection<AutoProcessRule> AutoProcessRules { get; set; } = [];
        // 選択中の自動処理ルール
        private static AutoProcessRule? _selectedAutoProcessRule;
        public static AutoProcessRule? SelectedAutoProcessRule {
            get => _selectedAutoProcessRule;
            set {
                _selectedAutoProcessRule = value;
            }
        }

        public ListAutoProcessRuleWindowViewModel(ObservableCollection<ContentFolderViewModel> rootFolderViewModel) {
            RootFolderViewModels = rootFolderViewModel;
            // AutoProcessRulesを更新
            AutoProcessRules = [.. AutoProcessRule.GetAllAutoProcessRules()];
            OnPropertyChanged(nameof(AutoProcessRules));

        }
        // RootFolderViewModel
        public ObservableCollection<ContentFolderViewModel> RootFolderViewModels { get; set; }

        // TabIndex
        private int _tabIndex = 0;
        public int TabIndex {
            get => _tabIndex;
            set {
                _tabIndex = value;
                OnPropertyChanged(nameof(TabIndex));
                OnPropertyChanged(nameof(AutoProcessRuleButtonVisibility));
                OnPropertyChanged(nameof(SaveSystemCommonSettingButtonVisibility));
            }
        }

        // AutoProcessRule用のButtonのVisibility
        public Visibility AutoProcessRuleButtonVisibility {
            get {
                return TabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        // システム共通保存ボタンのVisibility
        public Visibility SaveSystemCommonSettingButtonVisibility {
            get {
                return TabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        //-- コマンド

        // 優先順位を上げる処理
        public SimpleDelegateCommand<string> ChangePriorityCommand => new((parameter) => {
            if (SelectedAutoProcessRule == null) {
                LogWrapper.Error(StringResources.AutoProcessRuleNotSelected);
                return;
            }
            if (parameter == "down") {
                AutoProcessRule.DownPriority(SelectedAutoProcessRule);
            } else {
                AutoProcessRule.UpPriority(SelectedAutoProcessRule);
            }
            // AutoProcessRulesを更新
            AutoProcessRules = [.. AutoProcessRule.GetAllAutoProcessRules()];
            OnPropertyChanged(nameof(AutoProcessRules));
        });

        public SimpleDelegateCommand<object> EditAutoProcessRuleCommand => new((parameter) => {
            // AutoProcessRuleが更新された後の処理
            void AutoProcessRuleUpdated(AutoProcessRule rule) {
                // AutoProcessRulesを更新
                AutoProcessRules = [.. AutoProcessRule.GetAllAutoProcessRules()];
                OnPropertyChanged(nameof(AutoProcessRules));
            }
            // debug
            if (SelectedAutoProcessRule == null) {
                LogWrapper.Error(StringResources.AutoProcessRuleNotSelected);
                return;
            }
            // RootFolderViewModelを取得

            EditAutoProcessRuleWindow.OpenEditAutoProcessRuleWindow(SelectedAutoProcessRule, RootFolderViewModels, AutoProcessRuleUpdated);
        });

        // 自動処理を追加する処理
        public SimpleDelegateCommand<object> AddAutoProcessRuleCommand => new((parameter) => {
            // AutoProcessRuleが更新された後の処理
            void AutoProcessRuleUpdated(AutoProcessRule rule) {
                // InstanceがNullの場合は処理を終了
                // AutoProcessRulesを更新
                AutoProcessRules = [.. AutoProcessRule.GetAllAutoProcessRules()];
                OnPropertyChanged(nameof(AutoProcessRules));
            }
            AutoProcessRule rule = new();
            EditAutoProcessRuleWindow.OpenEditAutoProcessRuleWindow(rule, RootFolderViewModels, AutoProcessRuleUpdated);
        });

        // 自動処理を削除する処理
        public SimpleDelegateCommand<object> DeleteAutoProcessRuleCommand => new((parameter) => {
            AutoProcessRule? rule = SelectedAutoProcessRule;
            if (rule == null) {
                LogWrapper.Error(StringResources.AutoProcessRuleNotSelected);
                return;
            }
            if (MessageBox.Show($"{rule.RuleName}{StringResources.ConfirmDelete}", StringResources.Confirm, MessageBoxButton.YesNo) != MessageBoxResult.Yes) {
                return;
            }
            AutoProcessRules.Remove(rule);
            // LiteDBを更新
            rule.Delete();
            OnPropertyChanged(nameof(AutoProcessRules));
        });

    }
}
