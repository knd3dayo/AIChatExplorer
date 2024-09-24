using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PythonAILib.Model;
using PythonAILib.Model.Content;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.ContentItemPanel
{
    public abstract class ContentItemPanelViewModel(ContentItemBase contentItemBase) {

        public static CommonStringResources StringResources { get; } = CommonStringResources.Instance;
        public ContentItemBase ContentItem { get; set; } = contentItemBase;

        // 選択中のContentAttachedItemBase
        public ContentAttachedItem? SelectedFile { get; set; }

        // 選択中のContentItemBaseを開く
        public abstract void OpenContentItem();

        // 選択中のContentItemBaseを削除
        public abstract void RemoveContentItem();
        // OpenContentItemCommand
        public SimpleDelegateCommand<object> OpenSelectedItemCommand => new((parameter) => {
            OpenContentItem();
        });

        // RemoveSelectedItemCommand
        public SimpleDelegateCommand<object> RemoveSelectedItemCommand => new((parameter) => {
            RemoveContentItem();
        });
    }
}
