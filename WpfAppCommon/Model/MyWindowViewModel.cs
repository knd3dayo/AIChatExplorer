using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Model {
    public class MyWindowViewModel : ObservableObject{

        // StringResources
        public StringResources StringResources { get; } = StringResources.Instance;


        // ロード時の処理
        private Window? window;
        public SimpleDelegateCommand LoadedCommand => new((parameter) => {
            RoutedEventArgs routedEventArgs = (RoutedEventArgs)parameter;
            Window window = (Window)routedEventArgs.Source;
            this.window = window;
            Tools.ActiveWindow = window;

        });

        // Activated時の処理
        public SimpleDelegateCommand ActivatedCommand => new((parameter) => {
            if (window != null && Tools.ActiveWindow != window) {
                Tools.ActiveWindow = window;
            }
        });

    }
}
