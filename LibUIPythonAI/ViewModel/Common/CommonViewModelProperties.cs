using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Common;

namespace LibUIPythonAI.ViewModel.Common {
    public class CommonViewModelProperties : ObservableObject{


        public static CommonViewModelProperties Instance { get; set; } = new CommonViewModelProperties();

        public  bool MarkdownView {
            get {
                return PythonAILibManager.Instance.ConfigParams.IsMarkdownView();
            }
            set {
                PythonAILibManager.Instance.ConfigParams.UpdateMarkdownView(value);
                OnPropertyChanged(nameof(MarkdownView));
            }
        }






    }
}
