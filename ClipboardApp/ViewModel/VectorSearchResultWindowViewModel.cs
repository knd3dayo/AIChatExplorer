using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ClipboardApp.Model;
using PythonAILib.Model.VectorDB;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel
{
    public class VectorSearchResultWindowViewModel : ClipboardAppViewModelBase{


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
