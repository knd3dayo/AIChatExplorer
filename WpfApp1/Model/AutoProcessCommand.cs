using System.IO;
using WK.Libraries.SharpClipboardNS;
using WpfApp1.Utils;

namespace WpfApp1.Model {
    public class AutoProcessCommand {

        // 自動処理でテキストを抽出」を実行するコマンド
        public static ClipboardItem ExtractTextCommandExecute(ClipboardItem clipboardItem) {

            if (clipboardItem.ContentType != SharpClipboard.ContentTypes.Files) {
                throw new ThisApplicationException("ファイル以外のコンテンツはテキストを抽出できません");
            }
            string path = clipboardItem.Content;
            string text = PythonExecutor.PythonFunctions.ExtractText(clipboardItem.Content);
            clipboardItem.Content = text;
            // タイプをテキストに変更
            clipboardItem.ContentType = SharpClipboard.ContentTypes.Text;
            MainWindowViewModel.StatusText.Text = $"{path}のテキストを抽出しました";
            return clipboardItem;

        }

        // 自動処理でデータをマスキング」を実行するコマンド
        public static ClipboardItem MaskDataCommandExecute(ClipboardItem clipboardItem) {
            if (MainWindowViewModel.Instance == null) {
                return clipboardItem;
            }
            if (clipboardItem.ContentType != SharpClipboard.ContentTypes.Text) {
                throw new ThisApplicationException("テキスト以外のコンテンツはマスキングできません");
            }
            Dictionary<string, List<string>> maskPatterns = new Dictionary<string, List<string>>();
            string result = PythonExecutor.PythonFunctions.GetMaskedString(clipboardItem.Content);
            clipboardItem.Content = result;

            MainWindowViewModel.StatusText.Text = "データをマスキングしました";
            return clipboardItem;
        }
        // 自動実行でPythonスクリプトを実行するコマンド
        public static void RunPythonScriptCommandExecute(ScriptItem scriptItem, ClipboardItem clipboardItem) {

            PythonExecutor.PythonFunctions.RunScript(scriptItem, clipboardItem);
            MainWindowViewModel.StatusText.Text = "Pythonスクリプトを実行しました";

        }
        // 自動処理でファイルパスをフォルダとファイル名に分割するコマンド
        public static void SplitFilePathCommandExecute(ClipboardItem clipboardItem) {

            if (clipboardItem.ContentType != SharpClipboard.ContentTypes.Files) {
                throw new ThisApplicationException("ファイル以外のコンテンツはファイルパスを分割できません");
            }
            string path = clipboardItem.Content;
            if (string.IsNullOrEmpty(path) == false) {
                // ファイルパスをフォルダ名とファイル名に分割
                string? folderPath = Path.GetDirectoryName(path);
                if (folderPath == null) {
                    throw new ThisApplicationException("フォルダパスが取得できません");
                }
                string? fileName = Path.GetFileName(path);
                clipboardItem.Content = folderPath + "\n" + fileName;
                // ContentTypeをTextに変更
                clipboardItem.ContentType = SharpClipboard.ContentTypes.Text;
                // StatusTextにメッセージを表示
                MainWindowViewModel.StatusText.Text = "ファイルパスをフォルダ名とファイル名に分割しました";
            }
        }

        public static void CreateAutoDescription(ClipboardItem item) {
            string updatedAtString = item.UpdatedAt.ToString("yyyy/MM/dd HH:mm:ss");
            // Textの場合
            if (item.ContentType == SharpClipboard.ContentTypes.Text) {
                item.Description = $"{updatedAtString} {item.SourceApplicationName} {item.SourceApplicationTitle}";
            }
            // Fileの場合
            else if (item.ContentType == SharpClipboard.ContentTypes.Files) {
                item.Description = $"{updatedAtString} {item.SourceApplicationName} {item.SourceApplicationTitle}";
                // Contentのサイズが50文字以上の場合は先頭20文字 + ... + 最後の30文字をDescriptionに設定
                if (item.Content.Length > 20) {
                    item.Description += " ファイル：" + item.Content.Substring(0, 20) + "..." + item.Content.Substring(item.Content.Length - 30);
                } else {
                    item.Description += " ファイル：" + item.Content;
                }
            }
        }
        // 自動でタグを付与するコマンド
        public static void CreateAutoTags(ClipboardItem item) {
            // PythonでItem.ContentからEntityを抽出
            HashSet<string> entities = PythonExecutor.PythonFunctions.ExtractEntity(item.Content);
            foreach (var entity in entities) {
                // LiteDBにタグを追加
                ClipboardDatabaseController.InsertTag(entity);
                // タグを追加
                item.Tags.Add(entity);
            }

        }

        // 自動処理でOpenAIにチャットを送信するコマンド
        public static string ChatCommandExecute(List<JSONChatItem> jSONChatItemList, bool masked = false) {
            string result = "";
            // maskedがTrueの場合
            if (masked) {
                List<string> beforeTextList = new List<string>();
                foreach (var jSONChatItem in jSONChatItemList) {
                    beforeTextList.Add(jSONChatItem.Content);
                }
                // マスキングデータを取得
                MaskedData maskedData = PythonExecutor.PythonFunctions.GetMaskedData(beforeTextList);

                for (int i = 0; i < jSONChatItemList.Count; i++) {
                    jSONChatItemList[i].Content = maskedData.AfterTextList[i];
                }
                // JsonChatItemListにマスキングデータを使用している旨のSystemMessageがない場合は追加
                JSONChatItem systemMessage = CreateMaskedDataSystemMessage();
                if (jSONChatItemList.Count == 0 || jSONChatItemList[0].Content != systemMessage.Content) {
                    jSONChatItemList.Insert(0, systemMessage);
                }

                // ステータスバーにメッセージを表示
                MainWindowViewModel.StatusText.Text = "OpenAIに送信するデータ:\n";
                foreach (var item in jSONChatItemList) {
                    MainWindowViewModel.StatusText.Text += item.Content + "\n";
                }
                // OpenAIにチャットを送信してレスポンスを受け取る
                result = PythonExecutor.PythonFunctions.OpenAIChat(jSONChatItemList);
                MainWindowViewModel.StatusText.Text = "OpenAIから受信したデータ:\n" + result + "\n";

                result = CovertMaskedDataToOriginalData(maskedData, result);

            } else {
                // ステータスバーにメッセージを表示
                MainWindowViewModel.StatusText.Text = "OpenAIに送信するデータ:\n";
                foreach (var item in jSONChatItemList) {
                    MainWindowViewModel.StatusText.Text += item.Content + "\n";
                }
                // OpenAIにチャットを送信してレスポンスを受け取る
                result = PythonExecutor.PythonFunctions.OpenAIChat(jSONChatItemList);
                MainWindowViewModel.StatusText.Text = "OpenAIから受信したデータ:\n" + result + "\n";
            }

            return result;
        }
        // OpenAIでテキストを成形するコマンド
        public static string FormatTextCommandExecute(string text) {
            string prompt = "次の文章はWindowsのクリップボードから取得した文章です。これを整形してください。重複した内容がある場合は削除してください。\n";

            // ChatCommandExecuteを実行
            prompt += "処理対象の文章\n-----------\n" + text;
            JSONChatItem chatItem = new JSONChatItem(ChatItem.UserRole, text);
            List<JSONChatItem> jSONChatItemList = [chatItem];
            string result = ChatCommandExecute(jSONChatItemList, Properties.Settings.Default.UserMaskedDataInOpenAI);

            return result;

        }

        private static string CovertMaskedDataToOriginalData(MaskedData? maskedData, string maskedText) {
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

        private static JSONChatItem CreateMaskedDataSystemMessage() {
            JSONChatItem jSONChatItem
                = new JSONChatItem(ChatItem.SystemRole,
                "このチャットではマスキングデータ(MASKED_...)を使用している場合があります。" +
                "マスキングデータの文字列はそのままにしてください");
            return jSONChatItem;
        }

        // 指定されたフォルダの全アイテムをマージするコマンド
        public static ClipboardItem MergeItemsCommandExecute(ClipboardItemFolder folder, ClipboardItem item) {
            if (folder.Items.Count == 0) {
                return item;
            }
            // マージ先のアイテムを取得
            ClipboardItem mergeToItem = folder.Items[0];

            // マージ元のアイテム
            List<ClipboardItem> mergedFromItems = new List<ClipboardItem>();
            for (int i = 1; i < folder.Items.Count; i++) {
                mergedFromItems.Add(folder.Items[i]);
            }
            // 最後に引数のアイテムを追加
            mergedFromItems.Add(item);
            // マージ元のアイテムをマージ先のアイテムにマージ
            mergeToItem.MergeItems(mergedFromItems, false);
            
            // マージしたアイテムを削除
            foreach (var mergedItem in mergedFromItems) {
                folder.DeleteItem(mergedItem);
            }
            return mergeToItem;
        }
        // 指定されたフォルダの中のSourceApplicationTitleが一致するアイテムをマージするコマンド
        public static ClipboardItem MergeItemsBySourceApplicationTitleCommandExecute(ClipboardItemFolder folder, ClipboardItem newItem) {
            if (folder.Items.Count == 0) {
                return newItem;
            }
            List<ClipboardItem> sameTitleItems = new List<ClipboardItem>();
            // マージ先のアイテムのうち、SourceApplicationTitleが一致するアイテムを取得
            foreach (var item in folder.Items) {
                if (newItem.SourceApplicationTitle == item.SourceApplicationTitle) {
                    sameTitleItems.Add(item);
                }
            }
            // mergeFromItemsが空の場合はnewItemをそのまま返す。
            if (sameTitleItems.Count == 0) {
                return newItem;
            }
            // マージ元のアイテムをUpdateAtの昇順にソート
            sameTitleItems.Sort((a, b) => a.UpdatedAt.CompareTo(b.UpdatedAt));
            // マージ元のアイテムをマージ先のアイテムにマージ
            ClipboardItem mergeToItem = folder.Items[0];
            // sameTitleItemsにnewItemを追加
            sameTitleItems.Add(newItem);
            // sameTitleItemsの1から最後までをマージ元のアイテムとする
            sameTitleItems.RemoveAt(0);
            // マージ元のアイテムをマージ先のアイテムにマージ
            mergeToItem.MergeItems(sameTitleItems, false);
            // マージしたアイテムを削除
            foreach (var mergedItem in sameTitleItems) {
                folder.DeleteItem(mergedItem);
            }
            return mergeToItem;
        }
    }
}
