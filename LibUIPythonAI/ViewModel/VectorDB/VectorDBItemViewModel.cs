using System.Windows;
using LibPythonAI.Model.VectorDB;
using PythonAILib.Model.VectorDB;

namespace LibUIPythonAI.ViewModel.VectorDB {
    public class VectorDBItemViewModel(VectorDBItem item) : ChatViewModelBase {
        public VectorDBItem Item { get; private set; } = item;


        // ベクトルDBの種類を表す列挙型
        public VectorDBTypeEnum VectorDBType {
            get => Item.Type;
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
        // CollectionName
        public string CollectionName {
            get => Item.CollectionName;
            set {
                Item.CollectionName = value;
                OnPropertyChanged(nameof(CollectionName));
            }
        }

        // DocStoreURL
        public string DocStoreURL {
            get => Item.DocStoreURL;
            set {
                Item.DocStoreURL = value;
                OnPropertyChanged(nameof(DocStoreURL));
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

        // IsUseMultiVectorRetriever
        public bool IsUseMultiVectorRetriever {
            get => Item.IsUseMultiVectorRetriever;
            set {
                Item.IsUseMultiVectorRetriever = value;
                OnPropertyChanged(nameof(IsUseMultiVectorRetriever));
                OnPropertyChanged(nameof(DocStoreURLVisibility));
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

        // DocStoreURLを表示するか否かのVisibility
        public Visibility DocStoreURLVisibility {
            get {
                if (IsUseMultiVectorRetriever) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

    }
}
