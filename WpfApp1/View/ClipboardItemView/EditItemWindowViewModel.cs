﻿using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Model;
using WpfApp1.Utils;
using WpfApp1.View.TagView;

namespace WpfApp1.View.ClipboardItemView
{
    class EditItemWindowViewModel : ObservableObject
    {
        private bool SingleLineSelected = false;
        private ClipboardItemViewModel? itemViewModel;
        public ClipboardItemViewModel? ItemViewModel {
            get {
                return itemViewModel;
            }
            set {
                itemViewModel = value;
                TagsString = string.Join(",", itemViewModel?.ClipboardItem?.Tags ?? new HashSet<string>());
                Description = itemViewModel?.ClipboardItem?.Description ?? "";
                Content = itemViewModel?.ClipboardItem?.Content ?? "";

                OnPropertyChanged("ClipboardItemViewModel");
            }
        }

        private string _description = "";
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }
        private string _content = "";
        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged("Content");
            }
        }

        private string title = "";
        public string Title {
            get {
                return title;
            }
            set {
                title = value;
                OnPropertyChanged("Title");
            }
        }


        //Tagを文字列に変換したもの
        private string _tagsString = "";
        public string TagsString {
            get {
                return _tagsString;
            }
            set {
                _tagsString = value;
                OnPropertyChanged("TagsString");
            }
        }

        private Action? _afterUpdate;

        public void Initialize(ClipboardItemViewModel? itemViewModel, Action afterUpdate)
        {
            if (itemViewModel == null) {
                ClipboardItem clipboardItem = new ClipboardItem();
                clipboardItem.CollectionName = MainWindowViewModel.Instance?.SelectedFolder?.AbsoluteCollectionName;
                ItemViewModel = new ClipboardItemViewModel(clipboardItem);
                title = "新規アイテム";
            } else {
                title = "アイテム編集";
                ItemViewModel = itemViewModel;
            }
            _afterUpdate = afterUpdate;

        }

        // タグ追加ボタンのコマンド
        public SimpleDelegateCommand AddTagButtonCommand => new SimpleDelegateCommand(EditTagCommandExecute);

        /// <summary>
        /// コンテキストメニューのタグをクリックしたときの処理
        /// 更新後にフォルダ内のアイテムを再読み込みする
        /// </summary>
        /// <param name="obj"></param>
        public  void EditTagCommandExecute(object obj) {

            if (ItemViewModel == null) {
                Tools.Error("クリップボードアイテムが選択されていません");
                return;
            }
            TagWindow tagWindow = new TagWindow();
            TagWindowViewModel tagWindowViewModel = (TagWindowViewModel)tagWindow.DataContext;
            tagWindowViewModel.Initialize(ItemViewModel.ClipboardItem, () => {
                // TagsStringを更新
                TagsString = string.Join(",", ItemViewModel.ClipboardItem.Tags);
            });

            tagWindow.ShowDialog();

        }
        // Ctrl + Aを一回をしたら行選択、二回をしたら全選択
        public SimpleDelegateCommand SelectTextCommand => new SimpleDelegateCommand((parameter) => {
            object? editorObject = EditItemWindow.Current?.FindName("Editor");
            if (editorObject == null) {
                return;
            }
            TextBox editor = (TextBox)editorObject;
            // 選択テキストに改行が含まれていない場合は行選択
            if (! editor.SelectedText.Contains('\n')) {
                int pos = editor.SelectionStart;
                // posがTextの長さを超える場合はTextの最後を指定
                if (pos >= editor.Text.Length) {
                    pos = editor.Text.Length - 1;
                }
                int lineStart = editor.Text.LastIndexOf('\n', pos) + 1;
                
                int lineEnd = editor.Text.IndexOf('\n', pos);
                if (lineEnd == -1) {
                    lineEnd = editor.Text.Length;
                }
                // EditorTextSelectionを更新
                SingleLineSelected = true;

                // lineEnd - lineStartが0以下の場合は何もしない
                if (lineEnd - lineStart <= 0) {
                    return;
                }
                editor.Select(lineStart, lineEnd - lineStart);

                return;
            } else {
                // 1行選択の場合は全選択
                if (SingleLineSelected) {
                    editor.SelectAll();
                    SingleLineSelected = false;
                    return;
                }
            }
        });


        // OKボタンのコマンド
        public SimpleDelegateCommand OKButtonCommand => new SimpleDelegateCommand(OKButtonCommandExecute);

        private void OKButtonCommandExecute(object parameter)
        {

            // TitleとContentの更新を反映
            if (ItemViewModel == null) {
                return;
            }
            ItemViewModel.ClipboardItem.Description = Description;
            ItemViewModel.ClipboardItem.Content = Content;
            // ClipboardItemを更新
            ClipboardDatabaseController.UpsertItem(ItemViewModel.ClipboardItem);
            // 更新後の処理を実行
            _afterUpdate?.Invoke();
            // ウィンドウを閉じる
            EditItemWindow.Current?.Close();
        }
        // キャンセルボタンのコマンド
        public SimpleDelegateCommand CancelButtonCommand => new SimpleDelegateCommand(CancelButtonCommandExecute);
        private void CancelButtonCommandExecute(object parameter)
        {
            // ウィンドウを閉じる
            EditItemWindow.Current?.Close();
        }

    }
}
