using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppCommon.Utils;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace WpfAppCommon.Control.Editor {
    public  class MyTextBox: TextBox {

        public MyTextBox() {
            // TextSelectorの初期化
            TextSelector = new TextSelector();

            // 各種設定
            SetSettings();

            // Loadedイベント
            Loaded += MyTextBox_Loaded;

        }


        // DependencyProperties
        public static readonly DependencyProperty TextSelectorProperty 
            = DependencyProperty.Register("TextSelector", typeof(TextSelector), typeof(MyTextBox), new PropertyMetadata(new TextSelector()));
        public TextSelector TextSelector {
            get { return (TextSelector)GetValue(TextSelectorProperty); }
            set { SetValue(TextSelectorProperty, value); }
        }

        // MyTextBox_Loadedイベント
        private void MyTextBox_Loaded(object sender, RoutedEventArgs e) {
            // Loadedイベント時の処理
            SetInputBindings();
            SetContextMenu();

        }


        // 各種設定
        public void SetSettings() {
            // Styleを設定
            // スタイルの定義
            this.Style = new Style(typeof(MyTextBox), FindResource(typeof(TextBox)) as Style);
            // HorizontalScrollBarVisibilityをAutoにする
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            // VerticalScrollBarVisibilityをAutoにする
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            // AcceptsReturnをTrueにする
            AcceptsReturn = true;

        }

        // InputBindingsの設定
        public void SetInputBindings() {

            // Ctrl + Aを一回をしたら行選択、二回をしたら全選択
            InputBindings.Add(new KeyBinding(SelectTextCommand, new KeyGesture(Key.A, ModifierKeys.Control)));
            // 選択中のテキストをプロセスとして実行
            InputBindings.Add(new KeyBinding(ExecuteSelectedTextCommand, new KeyGesture(Key.O, ModifierKeys.Control)));
        }

        // コンテキストメニューの設定
        public void SetContextMenu() {
            // コンテキストメニューの設定
            
            ContextMenu = this.ContextMenu ?? new ContextMenu();
            // テキスト選択
            ContextMenu.Items.Add(new MenuItem() { Header = "Select", Command = SelectTextCommand });
            // 選択中のテキストをプロセスとして実行
            ContextMenu.Items.Add(new MenuItem() { Header = "Execute", Command = ExecuteSelectedTextCommand });
        }

        
        // Ctrl + Aを一回をしたら行選択、二回をしたら全選択
        public SimpleDelegateCommand<object> SelectTextCommand => new((parameter) => {

            // テキスト選択
            TextSelector.SelectText(this);
            return;
        });
        // 選択中のテキストをプロセスとして実行
        public SimpleDelegateCommand<object> ExecuteSelectedTextCommand => new((parameter) => {

            // 選択中のテキストをプロセスとして実行
            TextSelector.ExecuteSelectedText(this);

        });



    }
}
