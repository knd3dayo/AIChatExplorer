using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PythonAILib.Model;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel {
    public class VectorSearchResultWindowViewModel : MyWindowViewModel{


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

        public SimpleDelegateCommand<Window> CloseCommand => new((window) => {
            // ウィンドウを閉じる
            window.Close();

        });


    }
}
