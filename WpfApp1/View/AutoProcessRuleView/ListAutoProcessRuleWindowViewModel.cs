using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Model;
using WpfApp1.Utils;
using WpfApp1.View.ClipboardItemFolderView;

namespace WpfApp1.View.AutoProcessRuleView
{
    public class ListAutoProcessRuleWindowViewModel : ObservableObject
    {
        // Instance
        public static ListAutoProcessRuleWindowViewModel? Instance = null;

        // 処理対象のClipboardItemFolder
        public static ClipboardItemFolderViewModel? TargetFolder { get; set; } = null;

        // ルールの一覧
        public ObservableCollection<AutoProcessRule> AutoProcessRules { get; set; } = new ObservableCollection<AutoProcessRule>();
        // 選択中の自動処理ルール
        private static AutoProcessRule? _selectedAutoProcessRule;
        public static AutoProcessRule? SelectedAutoProcessRule
        {
            get => _selectedAutoProcessRule;
            set
            {
                _selectedAutoProcessRule = value;
            }
        }
        // 初期化
        public void Initialize()
        {
            // Instance
            Instance = this;
            // AutoProcessRulesを更新
            AutoProcessRules = ClipboardDatabaseController.GetAllAutoProcessRules();
            OnPropertyChanged("AutoProcessRules");

        }

        public void Initialize(ClipboardItemFolderViewModel? targetFolder)
        {
            TargetFolder = targetFolder;
            // AutoProcessRulesを更新
            AutoProcessRules = ClipboardDatabaseController.GetAutoProcessRules(TargetFolder?.ClipboardItemFolder);
            OnPropertyChanged("AutoProcessRules");

        }

        public static SimpleDelegateCommand EditAutoProcessRuleCommand => new SimpleDelegateCommand(EditAutoProcessRuleCommandExecute);

        // 自動処理ルールを編集する処理
        public static void EditAutoProcessRuleCommandExecute(object parameter)
        {
            // AutoProcessRuleが更新された後の処理
            void AutoProcessRuleUpdated(AutoProcessRule rule)
            {
                // InstanceがNullの場合は処理を終了
                if (Instance == null)
                {
                    return;
                }
                // AutoProcessRulesを更新
                Instance.AutoProcessRules = ClipboardDatabaseController.GetAutoProcessRules(TargetFolder?.ClipboardItemFolder);
                Instance.OnPropertyChanged("AutoProcessRules");
            }
            // debug
            if (SelectedAutoProcessRule == null)
            {
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

        public static void AddAutoProcessRuleCommandExecute(object parameter)
        {
            // AutoProcessRuleが更新された後の処理
            void AutoProcessRuleUpdated(AutoProcessRule rule)
            {
                // InstanceがNullの場合は処理を終了
                if (Instance == null)
                {
                    return;
                }
                // AutoProcessRulesを更新
                Instance.AutoProcessRules = ClipboardDatabaseController.GetAutoProcessRules(TargetFolder?.ClipboardItemFolder);
                Instance.OnPropertyChanged("AutoProcessRules");
            }
            EditAutoProcessRuleWindow window = new EditAutoProcessRuleWindow();
            EditAutoProcessRuleWindowViewModel editAutoProcessRuleWindowViewModel = (EditAutoProcessRuleWindowViewModel)window.DataContext;
            editAutoProcessRuleWindowViewModel.InitializeCreate(TargetFolder, AutoProcessRuleUpdated);
            window.ShowDialog();
        }
        // 自動処理を削除する処理
        public SimpleDelegateCommand DeleteAutoProcessRuleCommand => new SimpleDelegateCommand(DeleteAutoProcessRuleCommandExecute);
        public void DeleteAutoProcessRuleCommandExecute(object parameter)
        {
            AutoProcessRule? rule = SelectedAutoProcessRule;
            if (rule == null)
            {
                System.Windows.MessageBox.Show("自動処理ルールが選択されていません。");
                return;
            }
            if (System.Windows.MessageBox.Show($"自動処理ルール{rule.RuleName}を削除しますか？", "確認", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes)
            {
                return;
            }
            AutoProcessRules.Remove(rule);
            // LiteDBを更新
            ClipboardDatabaseController.DeleteAutoProcessRule(rule);
            OnPropertyChanged("AutoProcessRules");
        }

        // CloseCommand
        public SimpleDelegateCommand CloseCommand => new SimpleDelegateCommand(CloseCommandExecute);
        public void CloseCommandExecute(object parameter)
        {
            ListAutoProcessRuleWindow.Current?.Close();
        }


    }
}
