using System.Collections.ObjectModel;
using WpfAppCommon.Utils;

namespace ClipboardApp.Views.ClipboardItemView {
    public class ClipboardAppMenuItem {
        // Title
        public string Title { get; set; }

        public ObservableCollection<ClipboardAppMenuItem> SubMenuItems { get; set; } = [];

        // Command
        public SimpleDelegateCommand<object> Command { get; set; }

        // InputGestureText
        public string? InputGestureText { get; set; }
        public ClipboardAppMenuItem(string title, SimpleDelegateCommand<object> command) {
            Title = title;
            Command = command;
        }
        public ClipboardAppMenuItem(string title, SimpleDelegateCommand<object> command, string inputGestureText) {
            Title = title;
            Command = command;
            InputGestureText = inputGestureText;
        }



    }
}
