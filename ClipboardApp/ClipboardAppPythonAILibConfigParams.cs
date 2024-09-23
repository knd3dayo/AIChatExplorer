using System.Windows;
using ClipboardApp.Factory;
using ClipboardApp.Model;
using PythonAILib.Model;
using PythonAILib.Model.Abstract;
using QAChat.Abstract;
using WpfAppCommon.Utils;

namespace ClipboardApp {
    public class ClipboardAppPythonAILibConfigParams : IPythonAILibConfigParams, IQAChatConfigParams {

        public string GetLang() {
            return ClipboardAppConfig.Instance.ActualLang;
        }
        public string GetPythonDllPath() {
            return ClipboardAppConfig.Instance.PythonDllPath;
        }
        public string GetPathToVirtualEnv() {
            return ClipboardAppConfig.Instance.PythonVenvPath;
        }
        public IDataFactory GetDataFactory() {
            return ClipboardAppFactory.Instance.GetClipboardDBController();
        }
        public OpenAIProperties GetOpenAIProperties() {
            return ClipboardAppConfig.Instance.CreateOpenAIProperties();
        }

        public Action<string> GetInfoAction() {
            return LogWrapper.Info;
        }
        public Action<string> GetWarnAction() {
            return LogWrapper.Warn;
        }
        public Action<string> GetErrorAction() {
            return LogWrapper.Error;
        }

        public TextWrapping GetTextWrapping() {
            return ClipboardAppConfig.Instance.TextWrapping;
        }
    }
}
