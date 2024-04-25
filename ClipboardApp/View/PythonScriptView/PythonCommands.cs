using ClipboardApp.Model;
using ClipboardApp.Utils;
using QAChat.Model;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;

namespace ClipboardApp.View.PythonScriptView {
    public class PythonCommands {


        public static void CreatePythonScriptCommandExecute(object obj) {
            EditPythonScriptWindow editScriptWindow = new EditPythonScriptWindow();
            EditPythonScriptWindowViewModel editScriptWindowViewModel = (EditPythonScriptWindowViewModel)editScriptWindow.DataContext;
            editScriptWindowViewModel.ScriptItem = new ScriptItem("", PythonExecutor.LoadPythonScript(PythonExecutor.TemplateScript), ScriptType.Python);
            editScriptWindow.ShowDialog();
        }

        public static void EditPythonScriptCommandExecute(object obj) {
            SelectPythonScriptWindow SelectScriptWindow = new SelectPythonScriptWindow();
            SelectScriptWindow.ShowDialog();
        }

        //--------------------------------------------------------------------------------
        // Pythonスクリプトを実行するコマンド
        //--------------------------------------------------------------------------------
        public static void DeleteScriptCommandExecute(object obj) {
            if (obj is ScriptItem) {
                ScriptItem scriptItem = (ScriptItem)obj;
                ScriptItem.DeleteScriptItem(scriptItem);
            }
        }
        // 自動処理でテキストを抽出」を実行するコマンド
        public static ClipboardItem ExtractTextCommandExecute(ClipboardItem clipboardItem) {

            if (clipboardItem.ContentType != ClipboardContentTypes.Files) {
                throw new ClipboardAppException("ファイル以外のコンテンツはテキストを抽出できません");
            }
            string path = clipboardItem.Content;
            string text = PythonExecutor.PythonFunctions.ExtractText(clipboardItem.Content);
            clipboardItem.Content = text;
            // タイプをテキストに変更
            clipboardItem.ContentType = ClipboardContentTypes.Text;
            MainWindowViewModel.StatusText.Text = $"{path}のテキストを抽出しました";
            return clipboardItem;

        }

        // 自動処理でデータをマスキング」を実行するコマンド
        public static ClipboardItem MaskDataCommandExecute(ClipboardItem clipboardItem) {
            if (MainWindowViewModel.Instance == null) {
                return clipboardItem;
            }
            if (clipboardItem.ContentType != ClipboardContentTypes.Text) {
                throw new ClipboardAppException("テキスト以外のコンテンツはマスキングできません");
            }
            Dictionary<string, List<string>> maskPatterns = new Dictionary<string, List<string>>();
            string spacyModel = WpfAppCommon.Properties.Settings.Default.SpacyModel;
            string result = PythonExecutor.PythonFunctions.GetMaskedString(spacyModel, clipboardItem.Content);
            clipboardItem.Content = result;

            MainWindowViewModel.StatusText.Text = "データをマスキングしました";
            return clipboardItem;
        }
        // 自動実行でPythonスクリプトを実行するコマンド
        public static void RunPythonScriptCommandExecute(ScriptItem scriptItem, ClipboardItem clipboardItem) {
            string inputJson = ClipboardItem.ToJson(clipboardItem);

            string result = PythonExecutor.PythonFunctions.RunScript(scriptItem.Content, inputJson);
            ClipboardItem? resultItem = ClipboardItem.FromJson(result, (message) => {
                MainWindowViewModel.StatusText.Text = "Pythonスクリプトを実行しました";

            });
            resultItem?.CopyTo(clipboardItem);

        }

        public static string CovertMaskedDataToOriginalData(MaskedData? maskedData, string maskedText) {
            if (maskedData == null) {
                return maskedText;
            }
            // マスキングデータをもとに戻す
            string result = maskedText;
            foreach (var entity in maskedData.Entities) {
                // ステータスバーにメッセージを表示
                MainWindowViewModel.StatusText.Text += $"マスキングデータをもとに戻します: {entity.Before} -> {entity.After}\n";
                result = result.Replace(entity.After, entity.Before);
            }
            return result;
        }

        private static ChatItem CreateMaskedDataSystemMessage() {
            ChatItem chatItem
                = new ChatItem(ChatItem.SystemRole,
                "このチャットではマスキングデータ(MASKED_...)を使用している場合があります。" +
                "マスキングデータの文字列はそのままにしてください");
            return chatItem;
        }

        public static string FormatTextCommandExecute(string text) {
            string prompt = "次の文章はWindowsのクリップボードから取得した文章です。これを整形してください。重複した内容がある場合は削除してください。\n";

            // ChatCommandExecuteを実行
            prompt += "処理対象の文章\n-----------\n" + text;

            ChatResult result = PythonExecutor.PythonFunctions.LangChainChat(prompt, []);

            return result.Response;

        }


    }
}
