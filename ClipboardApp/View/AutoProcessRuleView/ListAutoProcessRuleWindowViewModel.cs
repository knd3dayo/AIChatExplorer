using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ClipboardApp.View.ClipboardItemFolderView;
using WpfAppCommon.Utils;
using WpfAppCommon.Model;
using System.Windows;

namespace ClipboardApp.View.AutoProcessRuleView
{
    public class ListAutoProcessRuleWindowViewModel : MyWindowViewModel {


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

        // CloseCommand
        public SimpleDelegateCommand<Window> CloseCommand => new ((window) => {
            window.Close();
        });
    }
}
