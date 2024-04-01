using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;

namespace WpfApp1
{
    public class TagWindowViewModel : ObservableObject
    {
        public ObservableCollection<TagItem> TagList { get; set; } = new ObservableCollection<TagItem>();
        private ILiteCollection<TagItem>? tagCollection;
    
        private ClipboardItem? _clipboardItem;
        public ClipboardItem? ClipboardItem {
            get
            {
                return _clipboardItem;
            }
            set
            {
                _clipboardItem = value;
                if (value != null)
                {
                    foreach (var item in TagList)
                    {
                        var tagString = item.Tag;
                        item.IsChecked = value.Tags.Contains(tagString);
                    }
                }
            }
        }

        public TagWindowViewModel()
        {
            // タグリストの初期化
            tagCollection = ClipboardDatabaseController.GetClipboardDatabase().GetCollection<TagItem>("tag");
            foreach (var item in tagCollection.FindAll())
            {
                TagList.Add(item);
            }
        }

        //新規タグのテキスト
        private string _newTag = "";
        public string NewTag
        {
            get
            {
                return _newTag;
            }
            set
            {
                _newTag = value;
                OnPropertyChanged("NewTag");
            }
        }


        // タグを追加したときの処理
        public SimpleDelegateCommand addTagCommand => new SimpleDelegateCommand(AddTagCommandExecute);

        private void AddTagCommandExecute(object obj)
        {
            
            if (obj is string)
            {
                string tag = (string)obj;
                //tagが既に存在するかチェック
                foreach (var item in TagList)
                {
                    if (item.Tag == tag)
                    {
                        return;
                    }
                }
                TagItem tagItem = new TagItem { Tag = tag };
                TagList.Add(tagItem);
                tagCollection?.Insert(tagItem);
                NewTag = "";

            }
        }

        // タグを削除したときの処理
        public SimpleDelegateCommand deleteTagCommand => new SimpleDelegateCommand(DeleteTagCommandExecute);

        private void DeleteTagCommandExecute(object obj)
        {
            if (obj is string)
            {
                string tag = (string)obj;
                //tagが既に存在するかチェック
                foreach (var item in TagList)
                {
                    if (item.Tag == tag)
                    {
                        TagList.Remove(item);
                        tagCollection?.Delete(item.Id);
                        return;
                    }
                }
            }
        }

        // OKボタンを押したときの処理
        public SimpleDelegateCommand okCommand => new SimpleDelegateCommand(OkCommandExecute);
        private void OkCommandExecute(object obj)
        {
            if (ClipboardItem == null)
            {
                return;
            }
            // ClipboardItemのタグをクリア
            ClipboardItem.Tags.Clear();
            // TagListのチェックを反映
            foreach (var item in TagList)
            {
                if (item.IsChecked)
                {
                    ClipboardItem.Tags.Add(item.Tag);
                }
                else
                {
                    ClipboardItem.Tags.Remove(item.Tag);
                }
            }
            // DBに反映
            ClipboardController.UpsertItem(ClipboardItem);
            // ウィンドウを閉じる
            TagWindow.Current?.Close();

        }
        

    }
}
