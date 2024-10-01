using System.Collections.ObjectModel;
using PythonAILib.Model.Content;
using PythonAILib.Model.VectorDB;

namespace QAChat.Control
{
    public class QAChatStartupProps {
        public QAChatStartupProps(ContentItem clipboardItem) {
            ContentItem = clipboardItem;
        }
        public ContentItem ContentItem { get; set; }


        public Action<ObservableCollection<VectorDBItem>> SelectVectorDBItemAction { get; set; } = (folders) => { };

        public Action<Action<List<ContentItem>>> AddContentItemCommandAction { get; set; } = (items) => { };

        public Action<ContentItem> OpenSelectedItemCommand { get; set; } = (item) => { };

        public Action<ContentItem> SaveCommand { get; set; } = (item) => { };
    }


}
