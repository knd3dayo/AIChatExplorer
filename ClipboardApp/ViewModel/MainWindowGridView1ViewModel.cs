using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel {
    public  class MainWindowGridView1ViewModel : ObservableObject {
        
        public Visibility PreviewModeVisibility { get; set; } = Visibility.Visible;

        public ClipboardItemViewModel? SelectedItem { get; set; }

        public ObservableCollection<ClipboardItemViewModel> ItemsSource { get; set; } = [];


        public SimpleDelegateCommand<object> ClipboardItemSelectionChangedCommand => new ((obj) => { });

        public SimpleDelegateCommand<object> OpenSelectedItemCommand => new((obj) => { });

        public SimpleDelegateCommand<object> CopyItemCommand => new((obj) => { });

        public SimpleDelegateCommand<object> DeleteItemCommand => new((obj) => { });

        public SimpleDelegateCommand<object> SelectTextCommand => new((obj) => { });

        public SimpleDelegateCommand<object> ExecuteSelectedTextCommand => new((obj) => { });

        public CommonStringResources StringResources { get; } = CommonStringResources.Instance;


    }
}
