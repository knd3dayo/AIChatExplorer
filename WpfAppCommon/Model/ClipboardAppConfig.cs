using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppCommon.PythonIF;

namespace WpfAppCommon.Model {
    public class ClipboardAppConfig {

        // WpfCommon.Properties.Settingsの値をプロパティとして宣言する。

        // MonitorTargetAppNames
        public static string MonitorTargetAppNames { get; set; } = WpfAppCommon.Properties.Settings.Default.MonitorTargetAppNames;

        // PythonExecution
        public static int PythonExecute { get; set; } = WpfAppCommon.Properties.Settings.Default.PythonExecution;
        // PythonDllPath
        public static string PythonDllPath { get; set; } = WpfAppCommon.Properties.Settings.Default.PythonDllPath;

        // UseOpenAI
        public static bool UseOpenAI { get; set; } = WpfAppCommon.Properties.Settings.Default.UseOpenAI;

        // AzureOpenAI
        public static bool AzureOpenAI { get; set; } = WpfAppCommon.Properties.Settings.Default.AzureOpenAI;
        // AzureOpenAIEndpoint
        public static string AzureOpenAIEndpoint { get; set; } = WpfAppCommon.Properties.Settings.Default.AzureOpenAIEndpoint;

        // OpenAIKey
        public static string OpenAIKey { get; set; } = WpfAppCommon.Properties.Settings.Default.OpenAIKey;
        // OpenAICompletionModel
        public static string OpenAICompletionModel { get; set; } = WpfAppCommon.Properties.Settings.Default.OpenAICompletionModel;
        // OpenAIEmbeddingModel
        public static string OpenAIEmbeddingModel { get; set; } = WpfAppCommon.Properties.Settings.Default.OpenAIEmbeddingModel;
        // OpenAICompletionBaseURL
        public static string OpenAICompletionBaseURL { get; set; } = WpfAppCommon.Properties.Settings.Default.OpenAICompletionBaseURL;
        // OpenAIEmbeddingBaseURL
        public static string OpenAIEmbeddingBaseURL { get; set; } = WpfAppCommon.Properties.Settings.Default.OpenAIEmbeddingBaseURL;

        // VectorDBURL
        public static string VectorDBURL { get; set; } = WpfAppCommon.Properties.Settings.Default.VectorDBURL;
        // UseOCR
        public static bool UseOCR { get; set; } = WpfAppCommon.Properties.Settings.Default.UseOCR;

        // UseSpacy
        public static bool UseSpacy { get; set; } = WpfAppCommon.Properties.Settings.Default.UseSpacy;
        // SpacyModel
        public static string SpacyModel { get; set; } = WpfAppCommon.Properties.Settings.Default.SpacyModel;

        // AutoMergeItemsBySourceApplicationTitle
        public static bool AutoMergeItemsBySourceApplicationTitle { get; set; } = WpfAppCommon.Properties.Settings.Default.AutoMergeItemsBySourceApplicationTitle;

        // AutoTag
        public static bool AutoTag { get; set; } = WpfAppCommon.Properties.Settings.Default.AutoTag;

        // AutoDescription
        public static bool AutoDescription { get; set; } = WpfAppCommon.Properties.Settings.Default.AutoDescription;

        // UserMaskedDataInOpenAI
        public static bool UserMaskedDataInOpenAI { get; set; } = WpfAppCommon.Properties.Settings.Default.UserMaskedDataInOpenAI;


        // BackupGeneration
        public static int BackupGeneration { get; set; } = WpfAppCommon.Properties.Settings.Default.BackupGeneration;

        // TesseractExePath
        public static string TesseractExePath { get; set; } = WpfAppCommon.Properties.Settings.Default.TesseractExePath;



        public static  Dictionary<string, string> CreateOpenAIProperties() {
            Dictionary<string, string> dict = new() {
                { "OpenAIKey", WpfAppCommon.Properties.Settings.Default.OpenAIKey },
                { "OpenAICompletionModel", WpfAppCommon.Properties.Settings.Default.OpenAICompletionModel },
                { "OpenAIEmbeddingModel", WpfAppCommon.Properties.Settings.Default.OpenAIEmbeddingModel },
                { "AzureOpenAI", WpfAppCommon.Properties.Settings.Default.AzureOpenAI.ToString() }
            };

            if (string.IsNullOrEmpty(WpfAppCommon.Properties.Settings.Default.AzureOpenAIEndpoint)) {
                dict.Add("AzureOpenAIEndpoint", WpfAppCommon.Properties.Settings.Default.AzureOpenAIEndpoint);
            }

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
