using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.Views.ClipboardItemView;

namespace ClipboardApp.Control {
    /// <summary>
    /// MainWindowListView1.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindowListView1 : UserControl {
        public MainWindowListView1() {
            InitializeComponent();
        }

        // SelectedItem
        public ClipboardItemViewModel SelectedItem {
            get { return (ClipboardItemViewModel)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(ClipboardItemViewModel), typeof(MainWindowListView1));

        public ObservableCollection<ClipboardItemViewModel> ItemsSource {
            get { return (ObservableCollection<ClipboardItemViewModel>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(ObservableCollection<ClipboardItemViewModel>), typeof(MainWindowListView1));

        public ICommand ClipboardItemSelectionChangedCommand {
            get { return (ICommand)GetValue(ClipboardItemSelectionChangedCommandProperty); }
            set { SetValue(ClipboardItemSelectionChangedCommandProperty, value); }
        }
        public static readonly DependencyProperty ClipboardItemSelectionChangedCommandProperty =
            DependencyProperty.Register(nameof(ClipboardItemSelectionChangedCommand), typeof(ICommand), typeof(MainWindowListView1));

        public ICommand OpenSelectedItemCommand {
            get { return (ICommand)GetValue(OpenSelectedItemCommandProperty); }
            set { SetValue(OpenSelectedItemCommandProperty, value); }
        }
        public static readonly DependencyProperty OpenSelectedItemCommandProperty =
            DependencyProperty.Register(nameof(OpenSelectedItemCommand), typeof(ICommand), typeof(MainWindowListView1));


        // SelectTextCommand
        public ICommand SelectTextCommand {
            get { return (ICommand)GetValue(SelectTextCommandProperty); }
            set { SetValue(SelectTextCommandProperty, value); }
        }
        public static readonly DependencyProperty SelectTextCommandProperty =
            DependencyProperty.Register(nameof(SelectTextCommand), typeof(ICommand), typeof(MainWindowListView1));

        // ExecuteSelectedTextCommand
        public ICommand ExecuteSelectedTextCommand {
            get { return (ICommand)GetValue(ExecuteSelectedTextCommandProperty); }
            set { SetValue(ExecuteSelectedTextCommandProperty, value); }
        }
        public static readonly DependencyProperty ExecuteSelectedTextCommandProperty =
            DependencyProperty.Register(nameof(ExecuteSelectedTextCommand), typeof(ICommand), typeof(MainWindowListView1));

    }


}
