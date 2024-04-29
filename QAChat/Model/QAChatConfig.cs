using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppCommon.Model {
    public class QAChatConfig {

        public static  Dictionary<string, string> CreateOpenAIProperties() {
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
            dict.Add("VectorDBURL", Properties.Settings.Default.VectorDBURL);
            return dict;
        }

    }
}
