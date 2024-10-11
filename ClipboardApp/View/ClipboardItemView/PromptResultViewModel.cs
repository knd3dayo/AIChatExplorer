using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PythonAILib.Model.Prompt;

namespace ClipboardApp.View.ClipboardItemView {
    internal class PromptResultViewModel(PromptChatResult promptChatResult) {
        public PromptChatResult PromptChatResult { get; set; } = promptChatResult;

    }
}
