using System.Windows;
using System.Windows.Controls;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel;
using PythonAILib.Model.AutoGen;
using Windows.Devices.Spi;
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

        // Model
        public string Model {
            get { return AutoGenLLMConfig.Model; }
            set {
                AutoGenLLMConfig.Model = value;
                OnPropertyChanged();
            }
        }

        // SelectedAITypeIndex
        public int SelectedAITypeIndex {
            get {
                if (AutoGenLLMConfig.ApiType == AutoGenLLMConfig.API_TYPE_AZURE) {
                    return 0;
                } else if (AutoGenLLMConfig.ApiType == AutoGenLLMConfig.API_TYPE_OPENAI) {
                    return 1;
                }
                return 0;
            }
            set {
                if (value == 0) {
                    AutoGenLLMConfig.ApiType = AutoGenLLMConfig.API_TYPE_AZURE;
                } else if (value == 1) {
                    AutoGenLLMConfig.ApiType = AutoGenLLMConfig.API_TYPE_OPENAI;
                }
                OnPropertyChanged();
            }
        }

        public SimpleDelegateCommand<RoutedEventArgs> APITypeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            SelectedAITypeIndex = comboBox.SelectedIndex;

        }, null, null);

        // SaveCommand
        public SimpleDelegateCommand<Window> SaveCommand => new((window) => {
            // Save
            AutoGenLLMConfig.Save();
            AfterUpdate();
            window.Close();
        }, null, null);
    }
}
