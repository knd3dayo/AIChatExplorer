using System.Windows;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel;
using PythonAILib.Model.AutoGen;
using WpfAppCommon.Utils;

namespace LibUIPythonAI.ViewModel.AutoGen {
    public class EditAutoGenLLMConfigViewModel : ChatViewModelBase {
        public EditAutoGenLLMConfigViewModel(AutoGenLLMConfig autoGenLLMConfig, Action afterUpdate) {
            AutoGenLLMConfig = autoGenLLMConfig;
            AfterUpdate = afterUpdate;
        }

        public AutoGenLLMConfig AutoGenLLMConfig { get; set; }

        public Action AfterUpdate { get; set; }

        // Name
        public string Name {
            get { return AutoGenLLMConfig.Name; }
            set {
                AutoGenLLMConfig.Name = value;
                OnPropertyChanged();
            }
        }
        // ApiType
        public string ApiType {
            get { return AutoGenLLMConfig.ApiType; }
            set {
                AutoGenLLMConfig.ApiType = value;
                OnPropertyChanged();
            }
        }

        // ApiVersion
        public string ApiVersion {
            get { return AutoGenLLMConfig.ApiVersion; }
            set {
                AutoGenLLMConfig.ApiVersion = value;
                OnPropertyChanged();
            }
        }
        // Model

        // BaseURL
        public string BaseURL {
            get { return AutoGenLLMConfig.BaseURL; }
            set {
                AutoGenLLMConfig.BaseURL = value;
                OnPropertyChanged();
            }
        }
        // SaveCommand
        public SimpleDelegateCommand<Window> SaveCommand => new((window) => {
            // Save
            AutoGenLLMConfig.Save();
            AfterUpdate();
            window.Close();
        });
    }
}
