using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClipboardApp.Utils;
using ClipboardApp.View.ClipboardItemView;
using WpfAppCommon.Utils;

namespace ClipboardApp.Model {
    public class ClipboardAppMenuItem {
        // Title
        public string Title { get; set; }

        public ObservableCollection<ClipboardAppMenuItem> SubMenuItems { get; set; } = new ObservableCollection<ClipboardAppMenuItem>();

        // Command
        public SimpleDelegateCommand Command { get; set; }

        // InputGestureText
        public string? InputGestureText { get; set; }
        public ClipboardAppMenuItem(string title, SimpleDelegateCommand command) {
            Title = title;
            Command = command;
        }
        public ClipboardAppMenuItem(string title, SimpleDelegateCommand command, string inputGestureText) {
            Title = title;
            Command = command;
            InputGestureText = inputGestureText;
        }



    }
}
