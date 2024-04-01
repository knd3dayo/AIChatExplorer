using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
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
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? Content { get; set; }

        public ScriptType Type { get; set; }

        public ScriptItem(string? description, string? content, ScriptType type)
        {
            Description = description;
            Content = content;
            Type = type;
        }
        // コンテキストメニューの「削除」の実行用コマンド
        public static SimpleDelegateCommand DeleteScriptCommand => new SimpleDelegateCommand(SelectScriptWindowViewModel.DeleteScriptCommandExecute);
        // コンテキストメニューの「スクリプト」の実行用コマンド
        public static SimpleDelegateCommand RunPythonScriptCommand => new SimpleDelegateCommand(PythonCommands.RunPythonScriptCommandExecute);

        // スクリプト選択画面でスクリプトをダブルクリックしたときの処理
        public static SimpleDelegateCommand SelectScriptCommand => new SimpleDelegateCommand(SelectScriptWindowViewModel.SelectScriptCommandExecute);

    }
}
