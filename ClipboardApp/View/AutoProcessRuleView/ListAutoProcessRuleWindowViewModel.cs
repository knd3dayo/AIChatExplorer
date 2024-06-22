using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ClipboardApp.View.ClipboardItemFolderView;
using WpfAppCommon.Utils;
using WpfAppCommon.Model;
using System.Windows;
using WpfAppCommon.Control.Settings;

namespace ClipboardApp.View.AutoProcessRuleView
{
    public class ListAutoProcessRuleWindowViewModel : MyWindowViewModel {


        // システム共通ルール設定用
        public SettingUserControlViewModel SettingUserControlViewModel { get; } = new SettingUserControlViewModel();

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
        private static MainWindowViewModel? _mainWindowViewModel;
        public void Initialize(MainWindowViewModel mainWindowViewModel) {
            _mainWindowViewModel = mainWindowViewModel;
            // AutoProcessRulesを更新
            AutoProcessRules = [.. AutoProcessRule.GetAllAutoProcessRules()];
            OnPropertyChanged(nameof(AutoProcessRules));

        }

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
        // IgnoreLineCountChecked
        public bool IgnoreLineCountChecked {
            get {
                // IgnoreLineCountが-1の場合はFalseを返す
                if (SettingUserControlViewModel.IgnoreLineCount == -1) {
                    return false;
                }
                return true;
            }
            set {
                // Falseの場合はIgnoreLineCountを-1にする
                if (!value) {
                    IgnoreLineCountText = "";
                }
                OnPropertyChanged(nameof(IgnoreLineCountChecked));
            }
        }
        // IgnoreLineCountText
        public string IgnoreLineCountText {
            get {
                // IgnoreLineCountが-1の場合は空文字を返す
                if (SettingUserControlViewModel.IgnoreLineCount == -1) {
                    return "";
                }
                return SettingUserControlViewModel.IgnoreLineCount.ToString();
            }
            set {
                // 空文字の場合は-1にする
                if (value == "") {
                    SettingUserControlViewModel.IgnoreLineCount = -1;
                } else {
                    SettingUserControlViewModel.IgnoreLineCount = int.Parse(value);
                }
                OnPropertyChanged(nameof(IgnoreLineCountText));
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
        // 優先順位を上げる処理
        public SimpleDelegateCommand<string> ChangePriorityCommand => new((parameter) => {
            if (SelectedAutoProcessRule == null) {
                System.Windows.MessageBox.Show("自動処理ルールが選択されていません。");
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
                System.Windows.MessageBox.Show("自動処理ルールが選択されていません。");
                return;
            }
            EditAutoProcessRuleWindow.OpenEditAutoProcessRuleWindow(EditAutoProcessRuleWindowViewModel.Mode.Edit, _mainWindowViewModel, SelectedAutoProcessRule, AutoProcessRuleUpdated);
        });

        // 自動処理を追加する処理
        public SimpleDelegateCommand <object> AddAutoProcessRuleCommand => new((parameter) => {
            // AutoProcessRuleが更新された後の処理
            void AutoProcessRuleUpdated(AutoProcessRule rule) {
                // InstanceがNullの場合は処理を終了
                // AutoProcessRulesを更新
                AutoProcessRules = [.. AutoProcessRule.GetAllAutoProcessRules()];
                OnPropertyChanged(nameof(AutoProcessRules));
            }
            EditAutoProcessRuleWindow.OpenEditAutoProcessRuleWindow(EditAutoProcessRuleWindowViewModel.Mode.Create, _mainWindowViewModel, null, AutoProcessRuleUpdated);
        });

        // 自動処理を削除する処理
        public SimpleDelegateCommand<object> DeleteAutoProcessRuleCommand => new((parameter) => {
            AutoProcessRule? rule = SelectedAutoProcessRule;
            if (rule == null) {
                System.Windows.MessageBox.Show("自動処理ルールが選択されていません。");
                return;
            }
            if (System.Windows.MessageBox.Show($"自動処理ルール{rule.RuleName}を削除しますか？", "確認", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes) {
                return;
            }
            AutoProcessRules.Remove(rule);
            // LiteDBを更新
            rule.Delete();
            OnPropertyChanged(nameof(AutoProcessRules));
        });

        // SaveSystemCommonSettingCommand
        public SimpleDelegateCommand<object> SaveSystemCommonSettingCommand => new((parameter) => {
            if (SettingUserControlViewModel.Save()) {
                LogWrapper.Info("システム共通設定を保存しました。");
            } else {
                LogWrapper.Warn("システム共通設定の変更はありません。");
            }
        });
        // CloseCommand
        public SimpleDelegateCommand<Window> CloseCommand => new ((window) => {
            window.Close();
        });
    }
}
