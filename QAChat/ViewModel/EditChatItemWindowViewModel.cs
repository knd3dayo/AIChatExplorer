using System.Windows.Controls;
using PythonAILib.Model.Chat;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel
{
    public class EditChatItemWindowViewModel : MyWindowViewModel {

        private readonly TextSelector TextSelector = new();

        public ChatIHistorytem? ChatItem { get; set; }

        public void Initialize(ChatIHistorytem chatItem) {
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
    }
}
