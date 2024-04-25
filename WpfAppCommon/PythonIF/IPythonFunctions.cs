using System.Drawing;
using QAChat.Model;
using WpfAppCommon.Model;

namespace WpfAppCommon.PythonIF {

    public class MaskedData {
        public HashSet<MaskedEntity> Entities { get; set; } = new HashSet<MaskedEntity>();
        public List<string> BeforeTextList { get; set; } = new List<string>();
        public List<string> AfterTextList { get; set; } = new List<string>();

        public MaskedData(List<string> beforeList) {

            foreach (var before in BeforeTextList) {
                BeforeTextList.Add(before);
            }
        }
    }
    public class MaskedEntity {
        public string Before { get; set; } = "";
        public string After { get; set; } = "";
        public string Label { get; set; } = "";

    }
    public interface IPythonFunctions {

        public string ExtractText(string path);
        public string GetMaskedString(string spacyModel, string text);
        public string GetUnmaskedString(string spacyModel, string maskedText);

        public string ExtractTextFromImage(Image image);

        public MaskedData GetMaskedData(string spacyModel, List<string> textList);

        public MaskedData GetUnMaskedData(string spacyModel, List<string> maskedTextList);


        public ChatResult LangChainChat(string prompt, IEnumerable<ChatItem> chatHistory);

        public void OpenAIEmbedding(string text);

        public void SaveFaissIndex();

        public void LoadFaissIndex();

        public string RunScript(string script, string input);

        // 引数として渡した文字列をSpacyで処理してEntityを抽出する
        public HashSet<string> ExtractEntity(string SpacyModel, string text);

    }
}
