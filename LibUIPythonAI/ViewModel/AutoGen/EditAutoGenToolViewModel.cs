using System.Windows;
using LibPythonAI.Model.AutoGen;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace LibUIPythonAI.ViewModel.AutoGen {
    public class EditAutoGenToolViewModel : CommonViewModelBase {

        public EditAutoGenToolViewModel(AutoGenTool autoGenTool, Action afterUpdate) {
            AutoGenTool = autoGenTool;
            AfterUpdate = afterUpdate;
        }

        public Action AfterUpdate { get; set; }
        public AutoGenTool AutoGenTool { get; set; }

        public string Name {
            get { return AutoGenTool.Name; }
            set {
                AutoGenTool.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Description {
            get { return AutoGenTool.Description; }
            set {
                AutoGenTool.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        //　Path
        public string Path {
            get { return AutoGenTool.Path; }
            set {
                AutoGenTool.Path = value;
                OnPropertyChanged(nameof(Path));
            }
        }

        // SaveCommand
        public SimpleDelegateCommand<Window> SaveCommand => new((window) => {
            AutoGenTool.Save();
            AfterUpdate();
            window.Close();
        }, null, null);

        public SimpleDelegateCommand<object> SelectFileCommand => new((obj) => {
            //ファイルダイアログを表示
            using var dialog = new CommonOpenFileDialog() {
                Title = CommonStringResources.Instance.SelectFilePlease,
                InitialDirectory = @".",
                DefaultExtension = ".py",
            };
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            if (dialog.ShowDialog(window) != CommonFileDialogResult.Ok) {
                return;
            } else {
                Path = dialog.FileName;
            }
        }, null, null);

    }
}
