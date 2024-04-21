using ClipboardApp.Model;
using ClipboardApp.Utils;
using QAChat.Model;

namespace ClipboardApp.PythonIF {
    public class EmptyPythonFunctions : IPythonFunctions {
        public string ExtractText(string path) {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public string GetMaskedString(string text) {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public string GetUnmaskedString(string maskedText) {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public string ExtractTextFromImage(System.Drawing.Image image) {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public MaskedData GetMaskedData(List<string> beforeTextList) {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public MaskedData GetUnMaskedData(List<string> maskedTextList) {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }

        public ChatResult LangChainChat(string prompt, IEnumerable<ChatItem> chatHistory) {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public void OpenAIEmbedding(string text) {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public void SaveFaissIndex() {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public void LoadFaissIndex() {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }

        public void RunScript(ScriptItem scriptItem, ClipboardItem clipboardItem) {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public HashSet<string> ExtractEntity(string text) {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
    }
}
