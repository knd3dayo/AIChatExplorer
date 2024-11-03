using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Control.Editor;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.Resource {
    public abstract class CommonViewModelBase : ObservableObject {

        private static MyTextBox.TextWrappingModeEnum _textWrappingMode = MyTextBox.TextWrappingModeEnum.Wrap;
        public static MyTextBox.TextWrappingModeEnum TextWrappingMode {
            get { return _textWrappingMode; }
            set { _textWrappingMode = value; }
        }

        public virtual void OnLoadedAction() { }
        public virtual void OnActivatedAction() { }

        public Window? ThisWindow { get; set; }

        // Loaded時の処理
        public SimpleDelegateCommand<RoutedEventArgs> LoadedCommand => new((routedEventArgs) => {
            if (routedEventArgs.Source is Window) {
                Window window = (Window)routedEventArgs.Source;
                ThisWindow = window;
                Tools.ActiveWindow = ThisWindow;
                // 追加処理
                OnLoadedAction();
                return;
            }
            if (routedEventArgs.Source is UserControl) {
                UserControl userControl = (UserControl)routedEventArgs.Source;
                Window window = Window.GetWindow(userControl);
                ThisWindow = window;
                Tools.ActiveWindow = ThisWindow;
                // 追加処理
                OnLoadedAction();
                return;
            }
        });

        // Activated時の処理
        public SimpleDelegateCommand<object> ActivatedCommand => new((parameter) => {
            if (ThisWindow == null) return;
            Tools.ActiveWindow = ThisWindow;
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
