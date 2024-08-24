using System.Windows;
using PythonAILib.Model;
using WpfAppCommon.Model;

namespace QAChat.ViewModel {
    public class VectorDBItemViewModel : MyWindowViewModel {

        private readonly VectorDBItem item;
        public VectorDBItemViewModel(VectorDBItem item) {
            this.item = item;
        }
        public VectorDBItem Item {
            get {
                return item;
            }
        }

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
        // MultiVectorDocChunkSize
        public int MultiVectorDocChunkSize {
            get => Item.MultiVectorDocChunkSize;
            set {
                Item.MultiVectorDocChunkSize = value;
                OnPropertyChanged(nameof(MultiVectorDocChunkSize));
            }
        }

        // MaxSearchResults
        public int MaxSearchResults {
            get => Item.MaxSearchResults;
            set {
                Item.MaxSearchResults = value;
                OnPropertyChanged(nameof(MaxSearchResults));
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
