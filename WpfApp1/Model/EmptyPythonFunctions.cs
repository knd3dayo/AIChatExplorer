﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Utils;
using WpfApp1.View.OpenAIView;

namespace WpfApp1.Model {
    public class EmptyPythonFunctions : IPythonFunctions{
        public string ExtractText(string path) {
            throw new Utils.ThisApplicationException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public string MaskData(string text) {
            throw new Utils.ThisApplicationException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public string OpenAIChat(List<JSONChatItem> jSONChatItems) {
            throw new Utils.ThisApplicationException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public void RunScript(ScriptItem scriptItem, ClipboardItem clipboardItem) {
            throw new Utils.ThisApplicationException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public List<string> ExtractEntity(string text) {
            throw new Utils.ThisApplicationException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
    }
}
