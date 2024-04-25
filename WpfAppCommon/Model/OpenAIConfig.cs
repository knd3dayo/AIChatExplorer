using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppCommon.Model {
    public class OpenAIConfig {

        public static  Dictionary<string, string> CreateOpenAIProperties() {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("OpenAIKey", WpfAppCommon.Properties.Settings.Default.OpenAIKey);
            dict.Add("OpenAICompletionModel", WpfAppCommon.Properties.Settings.Default.OpenAICompletionModel);
            dict.Add("OpenAIEmbeddingModel", WpfAppCommon.Properties.Settings.Default.OpenAIEmbeddingModel);
            dict.Add("AzureOpenAI", WpfAppCommon.Properties.Settings.Default.AzureOpenAI.ToString());

            if (WpfAppCommon.Properties.Settings.Default.OpenAICompletionBaseURL != "") {
                dict.Add("OpenAICompletionBaseURL", WpfAppCommon.Properties.Settings.Default.OpenAICompletionBaseURL);
            }
            if (WpfAppCommon.Properties.Settings.Default.OpenAIEmbeddingBaseURL != "") {
                dict.Add("OpenAIEmbeddingBaseURL", WpfAppCommon.    Properties.Settings.Default.OpenAIEmbeddingBaseURL);
            }
            dict.Add("VectorDBURL", WpfAppCommon.Properties.Settings.Default.VectorDBURL);
            return dict;
        }

    }
}
