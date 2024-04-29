using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ClipboardApp.View.ClipboardItemFolderView;
using WpfAppCommon.Utils;
using WpfAppCommon.Model;

namespace ClipboardApp.View.AutoProcessRuleView
{
    public class ListAutoProcessRuleWindowViewModel : ObservableObject {
        // Instance
        public static ListAutoProcessRuleWindowViewModel? Instance { get; set; } = null;

        // 処理対象のClipboardItemFolder
        public static ClipboardFolderViewModel? TargetFolder { get; set; } = null;

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
        // 初期化
        public void Initialize() {
            // Instance
            Instance = this;
            // AutoProcessRulesを更新
            AutoProcessRules = AutoProcessRuleController.GetAllAutoProcessRules();
            OnPropertyChanged(nameof(AutoProcessRules));

        }

        public void Initialize(ClipboardFolderViewModel? targetFolder) {
            TargetFolder = targetFolder;
            // AutoProcessRulesを更新
            AutoProcessRules = AutoProcessRuleController.GetAutoProcessRules(TargetFolder?.ClipboardItemFolder);
            OnPropertyChanged(nameof(AutoProcessRules));

        }

        public static SimpleDelegateCommand EditAutoProcessRuleCommand => new (EditAutoProcessRuleCommandExecute);

        // 自動処理ルールを編集する処理
        public static void EditAutoProcessRuleCommandExecute(object parameter) {
            // AutoProcessRuleが更新された後の処理
            static void AutoProcessRuleUpdated(AutoProcessRule rule) {
                // InstanceがNullの場合は処理を終了
                if (Instance == null) {
                    return;
                }
                // AutoProcessRulesを更新
                Instance.AutoProcessRules = AutoProcessRuleController.GetAutoProcessRules(TargetFolder?.ClipboardItemFolder);
                Instance.OnPropertyChanged(nameof(AutoProcessRules));
            }
            // debug
            if (SelectedAutoProcessRule == null) {
                System.Windows.MessageBox.Show("自動処理ルールが選択されていません。");
                return;
            }
            EditAutoProcessRuleWindow window = new();
            EditAutoProcessRuleWindowViewModel editAutoProcessRuleWindowViewModel = (EditAutoProcessRuleWindowViewModel)window.DataContext;
            editAutoProcessRuleWindowViewModel.InitializeEdit(TargetFolder, SelectedAutoProcessRule, AutoProcessRuleUpdated);
            window.ShowDialog();
        }

        // 自動処理を追加する処理
        public static SimpleDelegateCommand AddAutoProcessRuleCommand => new((parameter) => {
            // AutoProcessRuleが更新された後の処理
            static void AutoProcessRuleUpdated(AutoProcessRule rule) {
                // InstanceがNullの場合は処理を終了
                if (Instance == null) {
                    return;
                }
                // AutoProcessRulesを更新
                Instance.AutoProcessRules = AutoProcessRuleController.GetAutoProcessRules(TargetFolder?.ClipboardItemFolder);
                Instance.OnPropertyChanged(nameof(AutoProcessRules));
            }
            EditAutoProcessRuleWindow window = new();
            EditAutoProcessRuleWindowViewModel editAutoProcessRuleWindowViewModel = (EditAutoProcessRuleWindowViewModel)window.DataContext;
            editAutoProcessRuleWindowViewModel.InitializeCreate(TargetFolder, AutoProcessRuleUpdated);
            window.ShowDialog();
        });

        // 自動処理を削除する処理
        public SimpleDelegateCommand DeleteAutoProcessRuleCommand => new(DeleteAutoProcessRuleCommandExecute);
        public void DeleteAutoProcessRuleCommandExecute(object parameter) {
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
        }

        // CloseCommand
        public SimpleDelegateCommand CloseCommand => new ((parameter) => {
            if (parameter is System.Windows.Window window) {
                window.Close();
            }
        });
    }
}
