using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PythonAILib.Model.AutoGen;

namespace LibUIPythonAI.ViewModel.AutoGen {
    internal class AutoGenWindowCommands {

        // SaveAutoGenAgentCommandExecute
        public void SaveAutoGenAgentCommandExecute(Window window, AutoGenAgent AutoGenAgent, Action AfterUpdate) {
            // Save
            AutoGenAgent.Save();
            AfterUpdate();
            window.Close();
        }
    }
}
