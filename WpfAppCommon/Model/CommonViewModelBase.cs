using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.Resource {
    public abstract class CommonViewModelBase : ObservableObject {

        public virtual void OnLoadedAction() { }
        public virtual void OnActivatedAction() { }

        public Window? ActiveWindow { get; set; }

        // Loaded時の処理
        public SimpleDelegateCommand<RoutedEventArgs> LoadedCommand => new((routedEventArgs) => {
            if (routedEventArgs.Source is Window) {
                Window window = (Window)routedEventArgs.Source;
                Tools.ActiveWindow = window;
                // 追加処理
                OnLoadedAction();
                return;
            }
            if (routedEventArgs.Source is UserControl) {
                UserControl userControl = (UserControl)routedEventArgs.Source;
                Window window = Window.GetWindow(userControl);
                Tools.ActiveWindow = window;
                // 追加処理
                OnLoadedAction();
                return;
            }
        });

        // Activated時の処理
        public SimpleDelegateCommand<object> ActivatedCommand => new((parameter) => {
            if (ActiveWindow == null) return;
            Tools.ActiveWindow = ActiveWindow;
            // 追加処理
            OnActivatedAction();
        });

        // CloseButtonを押した時の処理
        public SimpleDelegateCommand<Window?> CloseCommand => new((window) => {
            if (window != null) {
                window.Close();
            }
        });

    }
}
