using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using WpfApp1.Utils;
using WpfApp1.View.ClipboardItemView;
using WpfApp1.View.ScriptView;

namespace WpfApp1.Model
{
    public enum ScriptType
    {
        Python,
        PowerShell,
        Batch,
        Shell,
        JavaScript,
        VBScript,
        CSharp,
        Other
    }
    public class ScriptItem
    {
        public ObjectId? Id { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }

        public ScriptType Type { get; set; }

        public ScriptItem(string description, string content, ScriptType type)
        {
            Description = description;
            Content = content;
            Type = type;
        }
        // コンテキストメニューの「削除」の実行用コマンド
        public static SimpleDelegateCommand DeleteScriptCommand = SelectScriptWindowViewModel.DeleteScriptCommandExecute;
        // コンテキストメニューの「スクリプト」の実行用コマンド
        public static SimpleDelegateCommand RunPythonScriptCommand => new SimpleDelegateCommand(ClipboardItemCommands.MenuItemRunPythonScriptCommandExecute);

        // スクリプト選択画面でスクリプトをダブルクリックしたときの処理
        public static SimpleDelegateCommand SelectScriptCommand => new SimpleDelegateCommand(SelectScriptWindowViewModel.SelectScriptCommandExecute);

    }
}
