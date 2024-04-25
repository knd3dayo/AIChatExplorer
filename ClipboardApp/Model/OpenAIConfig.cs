
namespace ClipboardApp.Model {
    public class OpenAIConfig {

        public static  Dictionary<string, string> CreateOpenAIConfig() {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("OpenAIKey", Properties.Settings.Default.OpenAIKey);
            dict.Add("OpenAICompletionModel", Properties.Settings.Default.OpenAICompletionModel);
            dict.Add("OpenAIEmbeddingModel", Properties.Settings.Default.OpenAIEmbeddingModel);
            dict.Add("AzureOpenAI", Properties.Settings.Default.AzureOpenAI.ToString());
            if (Properties.Settings.Default.OpenAICompletionBaseURL != "") {
                dict.Add("OpenAICompletionBaseURL", Properties.Settings.Default.OpenAICompletionBaseURL);
            }
            if (Properties.Settings.Default.OpenAIEmbeddingBaseURL != "") {
                dict.Add("OpenAIEmbeddingBaseURL", Properties.Settings.Default.OpenAIEmbeddingBaseURL);
            }
            return dict;
        }

    }
}
