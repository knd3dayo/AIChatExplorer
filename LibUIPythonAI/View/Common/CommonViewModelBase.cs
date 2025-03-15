using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LibUIPythonAI.Control.Editor;
using LibUIPythonAI.Utils;

namespace LibUIPythonAI.Resource {
    public abstract class CommonViewModelBase : ObservableObject {

        private static MyTextBox.TextWrappingModeEnum _textWrappingMode = MyTextBox.TextWrappingModeEnum.Wrap;
        public static MyTextBox.TextWrappingModeEnum TextWrappingMode {
            get { return _textWrappingMode; }
            set { _textWrappingMode = value; }
        }


        public virtual void OnLoadedAction() { }
        public virtual void OnActivatedAction() { }

        public Window? ThisWindow { get; set; }

        public UserControl? ThisUserControl { get; set; }

        // Loaded時の処理
        public virtual SimpleDelegateCommand<RoutedEventArgs> LoadedCommand => new((routedEventArgs) => {
            if (routedEventArgs.Source is Window) {
                Window window = (Window)routedEventArgs.Source;
                ThisWindow = window;
                LibUIPythonAI.Utils.Tools.ActiveWindow = ThisWindow;
                // 追加処理
                OnLoadedAction();
                return;
            }
            if (routedEventArgs.Source is UserControl) {
                UserControl userControl = (UserControl)routedEventArgs.Source;
                ThisUserControl = userControl;

                Window window = Window.GetWindow(userControl);
                ThisWindow = window;
                LibUIPythonAI.Utils.Tools.ActiveWindow = ThisWindow;
                // 追加処理
                OnLoadedAction();
                return;
            }
        });

        // Activated時の処理
        public virtual SimpleDelegateCommand<object> ActivatedCommand => new((parameter) => {
            if (ThisWindow == null) return;
            Tools.ActiveWindow = ThisWindow;
            // 追加処理
            OnActivatedAction();
        });

        // CloseButtonを押した時の処理
        public virtual SimpleDelegateCommand<object> CloseCommand => new((parameter) => {
            // parameterがWindowの場合
            if (parameter is Window window) {
                // ウィンドウを閉じる
                window.Close();
            } else if (ThisWindow != null) {
                ThisWindow.Close();
            }
        });

    }
}
