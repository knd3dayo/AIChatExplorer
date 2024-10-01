using System.Collections.ObjectModel;
using PythonAILib.Model.VectorDB;
using QAChat.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.VectorDBWindow {
    public class VectorSearchResultWindowViewModel : QAChatViewModelBase {


        public void Initialize(List<VectorSearchResult> vectorSearches) {
            VectorSearchResults = [.. vectorSearches];
            OnPropertyChanged(nameof(VectorSearchResults));
        }

        public ObservableCollection<VectorSearchResult> VectorSearchResults { get; set; } = [];

        // クリアボタンのコマンド
        public SimpleDelegateCommand<object> ClearCommand => new((parameter) => {
            // VectorSearchResultsをクリア
            VectorSearchResults.Clear();
            OnPropertyChanged(nameof(VectorSearchResults));
        });

    }
}
