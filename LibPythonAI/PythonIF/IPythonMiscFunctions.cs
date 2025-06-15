using System.Drawing;

namespace LibPythonAI.PythonIF {
    public interface IPythonMiscFunctions {

        public string GetMaskedString(string spacyModel, string text);
        public string GetUnmaskedString(string spacyModel, string maskedText);

        public string ExtractTextFromImage(Image image, string tesseractExePath);


        public string RunScript(string script, string input);

        // 引数として渡した文字列をSpacyで処理してEntityを抽出する
        public HashSet<string> ExtractEntity(string SpacyModel, string text);

    }
}
