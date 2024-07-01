using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ClipboardApp.View.ClipboardItemFolderView;
using WpfAppCommon.Utils;
using WpfAppCommon.Model;
using System.Windows;
using WpfAppCommon.Control.Settings;

namespace ClipboardApp.View.AutoProcessRuleView
{
    public partial class ListAutoProcessRuleWindowViewModel : MyWindowViewModel {


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

    }
}
