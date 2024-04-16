using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Model;
using WpfApp1.Utils;

namespace WpfApp1.PythonIF
{
    public class EmptyPythonFunctions : IPythonFunctions
    {
        public string ExtractText(string path)
        {
            throw new ThisApplicationException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public string GetMaskedString(string text)
        {
            throw new ThisApplicationException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public MaskedData GetMaskedData(List<string> beforeTextList)
        {
            throw new ThisApplicationException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public string OpenAIChat(List<JSONChatItem> jSONChatItems)
        {
            throw new ThisApplicationException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public void OpenAIEmbedding(string text)
        {
            throw new ThisApplicationException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public void SaveFaissIndex()
        {
            throw new ThisApplicationException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public void LoadFaissIndex()
        {
            throw new ThisApplicationException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }

        public void RunScript(ScriptItem scriptItem, ClipboardItem clipboardItem)
        {
            throw new ThisApplicationException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public HashSet<string> ExtractEntity(string text)
        {
            throw new ThisApplicationException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
    }
}
