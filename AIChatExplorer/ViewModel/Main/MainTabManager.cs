using System.Collections.ObjectModel;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;

namespace AIChatExplorer.ViewModel.Main {

    /// <summary>
    /// メイン画面のタブを管理するためのViewModel
    /// </summary>
    public class MainTabManager : CommonViewModelBase {


        public ObservableCollection<MainTabContent> TabItems { get; set; } = [];


        // メインウィンドウにアイテムのタブを追加
        public void AddTabItem(MainTabContent tabItem) {

            if (ThisWindow == null) {
                return;
            }
            // HeaderWidthを設定. 現在のタブ数 * AIChatExplorerTabContainerのHeaderWidth > ThisWindow.Widthの場合はThisWindow.Widthを超えないようにする
            double tabControlWidth = ThisWindow.ActualWidth - 500;
            if ((TabItems.Count + 1) * MainTabContent.HeaderWidthStatic > tabControlWidth) {
                MainTabContent.HeaderWidthStatic = tabControlWidth / (TabItems.Count + 1);
                for (int i = 1; i < TabItems.Count; i++) {
                    TabItems[i].HeaderWidth = MainTabContent.HeaderWidthStatic;
                }
            }

            TabItems.Add(tabItem);
            SelectedTabIndex = TabItems.Count - 1;
            OnPropertyChanged(nameof(TabItems));

        }
        // メインウィンドウからアイテムのタブを削除

        public void RemoveTabItem(MainTabContent tabItem) {

            TabItems.Remove(tabItem);
            OnPropertyChanged(nameof(TabItems));
        }

        // SelectedTabIndex
        private int _selectedTabIndex = -1;
        public int SelectedTabIndex {
            get { return _selectedTabIndex; }
            set {
                _selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
            }
        }

        public SimpleDelegateCommand<MainTabContent> CloseTabCommand => new((tabItem) => {
            if (tabItem == null || TabItems.Count == 1) {
                return;
            }
            TabItems.Remove(tabItem);
            OnPropertyChanged(nameof(TabItems));
        });


    }
}
