using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Model {
    public class MyWindowViewModel : ObservableObject{

        // CommonStringResources
        public CommonStringResources StringResources {
            get {
                // 文字列リソースの言語設定
                CommonStringResources.Lang = ClipboardAppConfig.ActualLang;
                return CommonStringResources.Instance;
            }
        }

        public virtual void OnLoadAction() { }

        public virtual void OnActivatedAction() { }
        // ロード時の処理
        private Window? window;
        public SimpleDelegateCommand<RoutedEventArgs> LoadedCommand => new((routedEventArgs) => {

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
        public SimpleDelegateCommand<object> ActivatedCommand => new((parameter) => {
            if (window != null) {
                Tools.ActiveWindow = window;
                OnActivatedAction();
            }
        });

        // CloseButtonを押した時の処理
        public SimpleDelegateCommand<Window?> CloseCommand => new((window) => {
            if (window != null) {
                window.Close();
            }
        });

    }
}
