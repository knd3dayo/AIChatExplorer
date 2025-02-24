using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.ViewModel.Folders.Clipboard;
using ClipboardApp.ViewModel.Settings;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;
using PythonAILib.Model.Content;
using PythonAILibUI.ViewModel.Item;
using ClipboardApp.Model.Item;
using ClipboardApp.ViewModel.Main;

namespace ClipboardApp.ViewModel.Content {
    public partial class ClipboardItemViewModel : ContentItemViewModel {

        // コンストラクタ
        public ClipboardItemViewModel(ContentFolderViewModel folderViewModel, ContentItemWrapper clipboardItem) : base(folderViewModel, clipboardItem) {
            if ( folderViewModel.Commands == null) {
                throw new Exception("folderViewModel.Commands is null");
            }
            ContentItem = clipboardItem;
            FolderViewModel = folderViewModel;
            Content = ContentItem.Content;
            Description = ContentItem.Description;
            Tags = ContentItem.Tags;
            SourceApplicationTitleText = ContentItem.SourceApplicationTitle;
            OnPropertyChanged(nameof(Content));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(Tags));
            OnPropertyChanged(nameof(SourceApplicationTitleText));
            OnPropertyChanged(nameof(FileTabVisibility));

        }

        // Context Menu

        public virtual ObservableCollection<MenuItem> ContentItemMenuItems {
            get {
                ClipboardItemMenu clipboardItemMenu = new(this);
                return clipboardItemMenu.ContentItemMenuItems;
            }
        }

        // Copy
        public virtual ClipboardItemViewModel Copy() {
            ContentItemWrapper newItem = ContentItem.Copy();
            return new ClipboardItemViewModel(FolderViewModel, newItem);
        }

        #region 開発中機能
        // EnableDevelopmentFeaturesがTrueの場合のみ有効
        public bool EnableDevelopmentFeatures {
            get { return ClipboardAppConfig.Instance.EnableDevFeatures; }
        }

        // EnableDevelopmentFeaturesがTrueの場合はVisible
        public Visibility DevFeaturesVisibility {
            get {
                return EnableDevelopmentFeatures ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // コンテキストメニューの「データをマスキング」の実行用コマンド
        public SimpleDelegateCommand<object> MaskDataCommand => new((parameter) => {
            if (ContentItem is not ClipboardItem clipboardItem) {
                return;
            }
            clipboardItem.MaskDataCommandExecute();
            // 保存
            ((AppItemViewModelCommands)FolderViewModel.Commands).SaveClipboardItemCommand.Execute(true);

        });
        #endregion



    }
}
