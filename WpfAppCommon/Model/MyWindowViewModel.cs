using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Model {
    public class MyWindowViewModel : ObservableObject{

        // StringResources
        public StringResources StringResources { get; } = StringResources.Instance;

        public virtual void OnLoadAction() { }

        public virtual void OnActivatedAction() { }
        // ロード時の処理
        private Window? window;
        public SimpleDelegateCommand LoadedCommand => new((parameter) => {
            RoutedEventArgs routedEventArgs = (RoutedEventArgs)parameter;
            if (routedEventArgs.Source is  Window) {
                Window window = (Window)routedEventArgs.Source;
                this.window = window;
                Tools.ActiveWindow = window;
                // 追加処理
                OnLoadAction();
                return;
            }
            if (routedEventArgs.Source is UserControl) {
                UserControl userControl = (UserControl)routedEventArgs.Source;
                Window window = Window.GetWindow(userControl);
                this.window = window;
                Tools.ActiveWindow = window;
                // 追加処理
                OnLoadAction();
                return;
            }

        });

        // Activated時の処理
        public SimpleDelegateCommand ActivatedCommand => new((parameter) => {
            if (window != null && Tools.ActiveWindow != window) {
                Tools.ActiveWindow = window;
                OnActivatedAction();
            }
        });

    }
}
