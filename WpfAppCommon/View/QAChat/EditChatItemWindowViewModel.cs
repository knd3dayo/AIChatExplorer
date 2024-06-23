using System.Windows;
using System.Windows.Controls;
using PythonAILib.Model;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace WpfAppCommon.View.QAChat {
    public class EditChatItemWindowViewModel: MyWindowViewModel {

        private readonly TextSelector TextSelector = new();

        public ChatItem? ChatItem { get; set; }

        public void Initialize(ChatItem chatItem) {
            ChatItem = chatItem;
            OnPropertyChanged(nameof(ChatItem));
        }

        // Ctrl + Aを一回をしたら行選択、二回をしたら全選択
        public SimpleDelegateCommand<TextBox> SelectTextCommand => new((textBox) => {

            // テキスト選択
            TextSelector.SelectText(textBox);
            return;
        });
        // 選択中のテキストをプロセスとして実行
        public SimpleDelegateCommand<TextBox> ExecuteSelectedTextCommand => new((textbox) => {

            // 選択中のテキストをプロセスとして実行
            TextSelector.ExecuteSelectedText(textbox);

        });

        // キャンセルボタンのコマンド
        public SimpleDelegateCommand<Window> CancelButtonCommand => new((window) => {
            // ウィンドウを閉じる
            window.Close();
        });



    }
}
