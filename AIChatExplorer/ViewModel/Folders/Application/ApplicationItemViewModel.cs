using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using AIChatExplorer.ViewModel.Folders.Application;
using AIChatExplorer.ViewModel.Settings;
using LibMain.Model.Content;
using LibUIMain.ViewModel.Folder;
using LibUIMain.ViewModel.Item;

namespace AIChatExplorer.ViewModel.Content {
    public partial class ApplicationItemViewModel : ContentItemViewModel {

        // コンストラクタ
        public ApplicationItemViewModel(ContentFolderViewModel folderViewModel, ContentItem applicationItem) : base(folderViewModel, applicationItem) {
            ContentItem = applicationItem;
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
            ApplicationItemMenu applicationItemMenu = new(this);
            ContentItemMenuItems = applicationItemMenu.CreateBasicItemContextMenuItems();
        }


        // Context Menu

        public override ObservableCollection<MenuItem> ContentItemMenuItems { get; protected set; } = [];

        // Copy
        public virtual ApplicationItemViewModel Copy() {
            ContentItem newItem = ContentItem.Copy();
            return new ApplicationItemViewModel(FolderViewModel, newItem);
        }

        #region 開発中機能
        // EnableDevelopmentFeaturesがTrueの場合のみ有効
        public bool EnableDevelopmentFeatures {
            get { return AIChatExplorerConfig.Instance.EnableDevFeatures; }
        }

        // EnableDevelopmentFeaturesがTrueの場合はVisible
        public Visibility DevFeaturesVisibility {
            get {
                return EnableDevelopmentFeatures ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion



    }
}
