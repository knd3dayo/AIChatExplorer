using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Model;

namespace QAChat.View.VectorDBWindow {
    public class VectorDBItemViewModel : MyWindowViewModel {

        private readonly VectorDBItem item;
        public VectorDBItemViewModel(VectorDBItem item) {
            this.item = item;
        }
        private VectorDBItem Item {
            get {
                return item;
            }
        }
        // VectorDBTypeEnum
        public VectorDBTypeEnum VectorDBType {
            get =>  Item.Type;
            set {
                Item.Type = value;
                OnPropertyChanged(nameof(VectorDBType));
            }
        }
        
        // Name
        public string Name {
            get => Item.Name;
            set {
                Item.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        // Description
        public string Description {
            get => Item.Description;
            set {
                Item.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        // VectorDBURL
        public string VectorDBURL {
            get => Item.VectorDBURL;
            set {
                Item.VectorDBURL = value;
                OnPropertyChanged(nameof(VectorDBURL));
            }
        }
        // IsEnabled
        public bool IsEnabled {
            get => Item.IsEnabled;
            set {
                Item.IsEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        // VectorDBTypeString
        public string VectorDBTypeString {
            get {
                return Item.VectorDBTypeString;

            }
        }
        // VectorDBType
        public VectorDBTypeEnum SelectedVectorDBType {
            get {
                return Item.Type;
            }
            set {
                Item.Type = value;
                OnPropertyChanged(nameof(SelectedVectorDBType));
            }
        }
        // VectorDBTypeList
        public static List<VectorDBTypeEnum> VectorDBTypeList {
            get {
                return [.. Enum.GetValues<VectorDBTypeEnum>()];
            }
        }



        // Save
        public void Save() {
            Item.Save();
        }
        // Delete
        public void Delete() {
            Item.Delete();
        }
        

    }
}
