using System.Drawing;
using PythonAILib.Model;

namespace PythonAILib.PythonIF {
    public partial interface IPythonFunctions {


        public string ExtractText(string path);
        public string GetMaskedString(string spacyModel, string text);
        public string GetUnmaskedString(string spacyModel, string maskedText);

        public string ExtractTextFromImage(Image image, string tesseractExePath);

        public MaskedData GetMaskedData(string spacyModel, List<string> textList);

        public MaskedData GetUnMaskedData(string spacyModel, List<string> maskedTextList);


        public void OpenAIEmbedding(OpenAIProperties props, string text);

        public string RunScript(string script, string input);

        // 引数として渡した文字列をSpacyで処理してEntityを抽出する
        public HashSet<string> ExtractEntity(string SpacyModel, string text);


        public ChatResult OpenAIChat(OpenAIProperties props, ChatRequest chatController);


        public ChatResult LangChainChat(OpenAIProperties props, ChatRequest chatController);


        public List<VectorSearchResult> VectorSearch(OpenAIProperties props, VectorDBItem vectorDBItem, string content);

        public void UpdateVectorDBIndex(OpenAIProperties props, GitFileInfo gitFileInfo, VectorDBItem vectorDBItem);

        public void UpdateVectorDBIndex(OpenAIProperties props, ContentInfo contentInfo, VectorDBItem vectorDBItem);

        //テスト用
        public string HelloWorld();
    }
}
