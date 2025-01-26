using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Control.Editor {
    public class MyTextBox : TextBox {
        // TextWrappingMode : 0 = NoWrap, 1 = Wrap, 2 = 閾値より小さい場合はWrap, 閾値以上の場合はNoWrap
        public enum TextWrappingModeEnum {
            NoWrap = 1,
            Wrap = 2,
            WrapWithThreshold = 3
        }

        public MyTextBox() {
            // TextSelectorの初期化
            TextSelector = new TextSelector();

            // 各種設定
            SetSettings();
            SetInputBindings();
            UpdateTextWrapping();

        }

        // DependencyProperties
        // TextWrappingMode
        public static readonly DependencyProperty TextWrappingModeProperty
            = DependencyProperty.Register("TextWrappingMode", typeof(TextWrappingModeEnum), typeof(MyTextBox), new PropertyMetadata(TextWrappingModeEnum.Wrap));
        public TextWrappingModeEnum TextWrappingMode {
            get { return (TextWrappingModeEnum)GetValue(TextWrappingModeProperty); }
            set { SetValue(TextWrappingModeProperty, value); }
        }

        protected void UpdateTextWrapping() {
            switch (TextWrappingMode) {
                case TextWrappingModeEnum.NoWrap:
                    TextWrapping = TextWrapping.NoWrap;
                    break;
                case TextWrappingModeEnum.Wrap:
                    TextWrapping = TextWrapping.Wrap;
                    break;
                case TextWrappingModeEnum.WrapWithThreshold:
                    TextWrapping = TextWrapping.WrapWithOverflow;
                    break;
            }
        }

        // TextSelector
        public static readonly DependencyProperty TextSelectorProperty
            = DependencyProperty.Register("TextSelector", typeof(TextSelector), typeof(MyTextBox), new PropertyMetadata(new TextSelector()));
        public TextSelector TextSelector {
            get { return (TextSelector)GetValue(TextSelectorProperty); }
            set { SetValue(TextSelectorProperty, value); }
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
            // Ctrl + ;を押したら日付を挿入
            InputBindings.Add(new KeyBinding(InsertDateCommand, new KeyGesture(Key.OemPlus, ModifierKeys.Control)));
            // Ctrl + :を押したら時刻を挿入
            InputBindings.Add(new KeyBinding(InsertTimeCommand, new KeyGesture(Key.OemSemicolon, ModifierKeys.Control)));
            // Ctrl + Xを押したら切り取り
            InputBindings.Add(new KeyBinding(ApplicationCommands.Cut, new KeyGesture(Key.X, ModifierKeys.Control)));
            // Ctrl + Zを押したらUndo
            InputBindings.Add(new KeyBinding(ApplicationCommands.Undo, new KeyGesture(Key.Z, ModifierKeys.Control)));
            // Ctrl + Yを押したらRedo
            InputBindings.Add(new KeyBinding(ApplicationCommands.Redo, new KeyGesture(Key.Y, ModifierKeys.Control)));
            // TABを押したらインデント ★ Editorのテキストを直接編集するとUndoバッファが消えるので対策するまではコメントアウト
            // InputBindings.Add(new KeyBinding(AddTabCommand, new KeyGesture(Key.Tab)));
            // Shift + TABを押したらインデント解除
            // InputBindings.Add(new KeyBinding(RemoveTabCommand, new KeyGesture(Key.Tab, ModifierKeys.Shift)));
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
        // 日付を挿入
        public SimpleDelegateCommand<object> InsertDateCommand => new((parameter) => {

            // 日付を挿入
            TextSelector.InsertDate(this);

        });
        // 時刻を挿入
        public SimpleDelegateCommand<object> InsertTimeCommand => new((parameter) => {

            // 時刻を挿入
            TextSelector.InsertTime(this);

        });

        // インデント
        public SimpleDelegateCommand<object> AddTabCommand => new((parameter) => {

            // インデント
            TextSelector.AddTab(this);

        });
        // インデント解除
        public SimpleDelegateCommand<object> RemoveTabCommand => new((parameter) => {

            // インデント解除
            TextSelector.RemoveTab(this);

        });

    }
}
