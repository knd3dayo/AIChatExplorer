using System.Windows;
using PythonAILib.Model;
using PythonAILib.Model.Abstract;
using QAChat.Abstract;
using WpfAppCommon.Utils;
using ImageChat.Model;

namespace ImageChat {
    public class ImageChatPythonAILibConfigParams : IPythonAILibConfigParams, IQAChatConfigParams {

        public string GetLang() {
            return ImageChatConfig.Instance.ActualLang;
        }
        public string GetPythonDllPath() {
            return ImageChatConfig.Instance.PythonDllPath;
        }
        public string GetPathToVirtualEnv() {
            return ImageChatConfig.Instance.PythonVenvPath;
        }
        public IDataFactory GetDataFactory() {
            return ImageChatDataFactory.Instance;
        }
        public OpenAIProperties GetOpenAIProperties() {
            return ImageChatConfig.Instance.CreateOpenAIProperties();
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
            return ImageChatConfig.Instance.TextWrapping;
        }

        public string GetDBPath() {
            throw new System.NotImplementedException();
        }

        public string GetSystemVectorDBPath() {
            throw new System.NotImplementedException();
        }

        public string GetSystemDocDBPath() {
            throw new System.NotImplementedException();
        }
    }
}
