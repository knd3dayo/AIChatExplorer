using System.Windows;
using LibMain.Model.VectorDB;
using LibUIMain.Resource;

namespace LibUIMain.ViewModel.VectorDB {
    public class VectorDBItemViewModel(VectorDBItem item) : CommonViewModelBase {
        public VectorDBItem Item { get; private set; } = item;


        // ベクトルDBの種類を表す列挙型
        public VectorDBTypeEnum VectorDBType {
            get => Item.Type;
            set {
                Item.Type = value;
                OnPropertyChanged(nameof(VectorDBType));
            }
        }

        // VectorDBName
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
        // CollectionName
        public string CollectionName {
            get => Item.CollectionName;
            set {
                Item.CollectionName = value;
                OnPropertyChanged(nameof(CollectionName));
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

        // ChunkSize
        public int ChunkSize {
            get => Item.ChunkSize;
            set {
                Item.ChunkSize = value;
                OnPropertyChanged(nameof(ChunkSize));
            }
        }

        // DefaultSearchResultLimit
        public int DefaultSearchResultLimit {
            get => Item.DefaultSearchResultLimit;
            set {
                Item.DefaultSearchResultLimit = value;
                OnPropertyChanged(nameof(DefaultSearchResultLimit));
            }
        }
        // DefaultScoreThreshold
        public float DefaultScoreThreshold {
            get => Item.DefaultScoreThreshold;
            set {
                Item.DefaultScoreThreshold = value;
                OnPropertyChanged(nameof(DefaultScoreThreshold));
            }
        }

    }
}
