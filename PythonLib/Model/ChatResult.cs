
namespace PythonLib.Model {
    public class ChatResult {

        public List<Dictionary<string,string>> ReferencedContents { get; set; } = [];
        public List<string> ReferencedFilePath { get; set; } = [];

        public string Response { get; set; } = "";

        public string Verbose { get; set; } = "";

        public ChatResult() { }

    }

}
