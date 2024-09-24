using System.Collections.ObjectModel;
using PythonAILib.Model;
using PythonAILib.Model.VectorDB;

namespace QAChat.Control
{
    public class QAChatStartupProps {
        public QAChatStartupProps(ContentItemBase clipboardItem) {
            ContentItem = clipboardItem;
        }
        public ContentItemBase ContentItem { get; set; }


        public Action<ObservableCollection<VectorDBItem>> SelectVectorDBItemAction { get; set; } = (folders) => { };

        public Action<Action<List<ContentItemBase>>> AddContentItemCommandAction { get; set; } = (items) => { };

        public Action<ContentItemBase> OpenSelectedItemCommand { get; set; } = (item) => { };

        public Action<ContentItemBase> SaveCommand { get; set; } = (item) => { };
    }


}
