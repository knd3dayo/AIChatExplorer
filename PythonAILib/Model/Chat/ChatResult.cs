namespace PythonAILib.Model.Chat {
    public class ChatResult {

        public List<Dictionary<string, string>> ReferencedContents { get; set; } = [];
        public List<string> ReferencedFilePath { get; set; } = [];

        public string Response { get; set; } = "";

        public string Verbose { get; set; } = "";

        public long TotalTokens { get; set; } = 0;

        public ChatResult() { }

    }

}
