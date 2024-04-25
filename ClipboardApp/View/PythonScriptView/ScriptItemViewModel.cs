using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClipboardApp.View.ClipboardItemView;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.PythonScriptView {
    public class ScriptItemViewModel {


        // コンテキストメニューの「削除」の実行用コマンド
        public static SimpleDelegateCommand DeleteScriptCommand = SelectPythonScriptWindowViewModel.DeleteScriptCommandExecute;
        // コンテキストメニューの「スクリプト」の実行用コマンド
        public static SimpleDelegateCommand RunPythonScriptCommand => new SimpleDelegateCommand(ClipboardItemCommands.MenuItemRunPythonScriptCommandExecute);

        // スクリプト選択画面でスクリプトをダブルクリックしたときの処理
        public static SimpleDelegateCommand SelectScriptCommand => new SimpleDelegateCommand(SelectPythonScriptWindowViewModel.SelectScriptCommandExecute);

    }
}
