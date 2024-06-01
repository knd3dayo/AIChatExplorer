using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.TagView {
    public class TagSearchWindowViewModel: MyWindowViewModel{
        private Action<string, bool> _afterUpdate = (tag, exclude) => { };

        public void Initialize(Action<string, bool> afterUpdate) {
            _afterUpdate = afterUpdate;
        }
        private bool _excludeTag = false;
        public bool ExcludeTag {
            get {
                return _excludeTag;
            }
            set {
                _excludeTag = value;
                OnPropertyChanged(nameof(ExcludeTag));
            }
        }

        private string _tagName = "";
        public string TagName {
            get {
                return _tagName;
            }
            set {
                _tagName = value;
                OnPropertyChanged(nameof(TagName));
            }
        }

        // クリアボタンの処理
        public SimpleDelegateCommand<object> ClearCommand => new((parameter) => {
            TagName = "";
            ExcludeTag = false;
        });
        // 閉じるボタンの処理
        public SimpleDelegateCommand<Window> CloseCommand => new((window) => {
            // ウィンドウを閉じる
            window.Close();
        });

        // 検索ボタンの処理
        public SimpleDelegateCommand<Window> SearchCommand => new((window) => {
            _afterUpdate(TagName, ExcludeTag);
            window.Close();
        });

    }
}
