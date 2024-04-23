using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using CommunityToolkit.Mvvm.ComponentModel;
using ClipboardApp.Model;
using ClipboardApp.Utils;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.Factory.Default;

namespace ClipboardApp.View.AutoProcessRuleView
{
    public class ListAutoProcessRuleWindowViewModel : ObservableObject {
        // Instance
        public static ListAutoProcessRuleWindowViewModel? Instance = null;

        // 処理対象のClipboardItemFolder
        public static ClipboardItemFolderViewModel? TargetFolder { get; set; } = null;

        // ルールの一覧
        public ObservableCollection<AutoProcessRule> AutoProcessRules { get; set; } = new ObservableCollection<AutoProcessRule>();
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
            OnPropertyChanged("AutoProcessRules");

        }

        public void Initialize(ClipboardItemFolderViewModel? targetFolder) {
            TargetFolder = targetFolder;
            // AutoProcessRulesを更新
            AutoProcessRules = AutoProcessRuleController.GetAutoProcessRules(TargetFolder?.ClipboardItemFolder);
            OnPropertyChanged("AutoProcessRules");

        }

        public static SimpleDelegateCommand EditAutoProcessRuleCommand => new (EditAutoProcessRuleCommandExecute);

        // 自動処理ルールを編集する処理
        public static void EditAutoProcessRuleCommandExecute(object parameter) {
            // AutoProcessRuleが更新された後の処理
            void AutoProcessRuleUpdated(AutoProcessRule rule) {
                // InstanceがNullの場合は処理を終了
                if (Instance == null) {
                    return;
                }
                // AutoProcessRulesを更新
                Instance.AutoProcessRules = AutoProcessRuleController.GetAutoProcessRules(TargetFolder?.ClipboardItemFolder);
                Instance.OnPropertyChanged("AutoProcessRules");
            }
            // debug
            if (SelectedAutoProcessRule == null) {
                System.Windows.MessageBox.Show("自動処理ルールが選択されていません。");
                return;
            }
            EditAutoProcessRuleWindow window = new EditAutoProcessRuleWindow();
            EditAutoProcessRuleWindowViewModel editAutoProcessRuleWindowViewModel = (EditAutoProcessRuleWindowViewModel)window.DataContext;
            editAutoProcessRuleWindowViewModel.InitializeEdit(TargetFolder, SelectedAutoProcessRule, AutoProcessRuleUpdated);
            window.ShowDialog();
        }

        // 自動処理を追加する処理
        public static SimpleDelegateCommand AddAutoProcessRuleCommand => new SimpleDelegateCommand(AddAutoProcessRuleCommandExecute);

        public static void AddAutoProcessRuleCommandExecute(object parameter) {
            // AutoProcessRuleが更新された後の処理
            void AutoProcessRuleUpdated(AutoProcessRule rule) {
                // InstanceがNullの場合は処理を終了
                if (Instance == null) {
                    return;
                }
                // AutoProcessRulesを更新
                Instance.AutoProcessRules = AutoProcessRuleController.GetAutoProcessRules(TargetFolder?.ClipboardItemFolder);
                Instance.OnPropertyChanged("AutoProcessRules");
            }
            EditAutoProcessRuleWindow window = new EditAutoProcessRuleWindow();
            EditAutoProcessRuleWindowViewModel editAutoProcessRuleWindowViewModel = (EditAutoProcessRuleWindowViewModel)window.DataContext;
            editAutoProcessRuleWindowViewModel.InitializeCreate(TargetFolder, AutoProcessRuleUpdated);
            window.ShowDialog();
        }
        // 自動処理を削除する処理
        public SimpleDelegateCommand DeleteAutoProcessRuleCommand => new SimpleDelegateCommand(DeleteAutoProcessRuleCommandExecute);
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
            OnPropertyChanged("AutoProcessRules");
        }

        // CloseCommand
        public SimpleDelegateCommand CloseCommand => new ((parameter) => {
            if (parameter is System.Windows.Window window) {
                window.Close();
            }
        });
    }
}
