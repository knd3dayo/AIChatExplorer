using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using AIChatExplorer.Model.Item;
using AIChatExplorer.ViewModel.Folders.Application;
using AIChatExplorer.ViewModel.Main;
using AIChatExplorer.ViewModel.Settings;
using LibPythonAI.Model.Content;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;

namespace AIChatExplorer.ViewModel.Content {
    public partial class ApplicationItemViewModel : ContentItemViewModel {

        // コンストラクタ
        public ApplicationItemViewModel(ContentFolderViewModel folderViewModel, ContentItemWrapper applicationItem) : base(folderViewModel, applicationItem) {
            if (folderViewModel.Commands == null) {
                throw new Exception("folderViewModel.Commands is null");
            }
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

        }

        // Context Menu

        public virtual ObservableCollection<MenuItem> ContentItemMenuItems {
            get {
                ApplicationItemMenu applicationItemMenu = new(this);
                return applicationItemMenu.ContentItemMenuItems;
            }
        }

        // Copy
        public virtual ApplicationItemViewModel Copy() {
            ContentItemWrapper newItem = ContentItem.Copy();
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
