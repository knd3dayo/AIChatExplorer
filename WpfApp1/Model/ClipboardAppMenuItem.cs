﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Utils;
using WpfApp1.View.ClipboardItemView;

namespace WpfApp1.Model {
    public class ClipboardAppMenuItem {
        // Title
        public string Title { get; set; }

        public ObservableCollection<ClipboardAppMenuItem> SubMenuItems { get; set; } = new ObservableCollection<ClipboardAppMenuItem>();

        // Command
        public SimpleDelegateCommand Command { get; set; }

        public ClipboardAppMenuItem(string title, SimpleDelegateCommand command) {
            Title = title;
            Command = command;
        }

    }
}
