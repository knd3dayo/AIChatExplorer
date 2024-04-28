using System.Drawing;
using QAChat.Model;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;

namespace WpfAppCommon.PythonIF {
    public class EmptyPythonFunctions : IPythonFunctions {
        public string ExtractText(string path) {
            throw new NotImplementedException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public string GetMaskedString(string spacyModel, string text) {
            throw new NotImplementedException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public string GetUnmaskedString(string spacyModel, string maskedText) {
            throw new NotImplementedException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public string ExtractTextFromImage(System.Drawing.Image image, string tesseractExePath) {
            throw new NotImplementedException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public MaskedData GetMaskedData(string spacyModel, List<string> beforeTextList) {
            throw new NotImplementedException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public MaskedData GetUnMaskedData(string spacyModel, List<string> maskedTextList) {
            throw new NotImplementedException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }

        public ChatResult LangChainChat(string prompt, IEnumerable<ChatItem> chatHistory) {
            throw new NotImplementedException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public ChatResult LangChainChat(string prompt, IEnumerable<ChatItem> chatHistory, Dictionary<string, string> props) {
            throw new NotImplementedException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public void OpenAIEmbedding(string text) {
            throw new NotImplementedException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public void SaveFaissIndex() {
            throw new NotImplementedException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public void LoadFaissIndex() {
            throw new NotImplementedException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }

        public string RunScript(string script, string inputJson) {
            throw new NotImplementedException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public HashSet<string> ExtractEntity(string SpacyModel, string text) {
            throw new NotImplementedException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public ChatResult OpenAIChat(string prompt, IEnumerable<ChatItem> chatHistory) {
            throw new NotImplementedException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public ChatResult OpenAIChat(string prompt, IEnumerable<ChatItem> chatHistory, Dictionary<string, string> props) {
            throw new NotImplementedException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public string HelloWorld() {
            throw new NotImplementedException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
    }
}
