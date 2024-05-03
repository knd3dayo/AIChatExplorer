using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public SimpleDelegateCommand ClearCommand => new((parameter) => {
            TagName = "";
            ExcludeTag = false;
        });
        // 閉じるボタンの処理
        public SimpleDelegateCommand CloseCommand => new((parameter) => {
            // ウィンドウを閉じる
            if (parameter is not System.Windows.Window window) {
                return;
            }
            window.Close();
        });

        // 検索ボタンの処理
        public SimpleDelegateCommand SearchCommand => new((parameter) => {
            _afterUpdate(TagName, ExcludeTag);

            if (parameter is not System.Windows.Window window) {
                return;
            }

            window.Close();
        });

    }
}
