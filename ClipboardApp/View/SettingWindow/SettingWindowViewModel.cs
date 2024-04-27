
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClipboardApp.View.SettingWindow {
    public class SettingWindowViewModel : ObservableObject{
        public enum PythonExecModeEnum {
            None,
            PythonNet,
        }

        // PythonDLLのパス
        public string PythonDllPath {
            get {
                return WpfAppCommon.Properties.Settings.Default.PythonDllPath;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.PythonDllPath = value;
                OnPropertyChanged(nameof(PythonDllPath));
            }
        }
        public PythonExecModeEnum PythonExecMode {
            get {
                int mode = WpfAppCommon.Properties.Settings.Default.PythonExecution;
                if (mode == 0) {
                    return PythonExecModeEnum.None;
                } else {
                    return PythonExecModeEnum.PythonNet;
                }
            }
            set {
                int mode = (int)value;
                WpfAppCommon.Properties.Settings.Default.PythonExecution = mode;
                OnPropertyChanged(nameof(PythonExecMode));
            }
        }
        // OpenAIを使用するかどうか
        public bool UseOpenAI {
            get {
                return WpfAppCommon.Properties.Settings.Default.UseOpenAI;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.UseOpenAI = value;
                OnPropertyChanged(nameof(UseOpenAI));
            }
        }



    }
}
