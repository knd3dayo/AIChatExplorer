using System.IO;
using System.Linq;
using WK.Libraries.SharpClipboardNS;
using ClipboardApp.PythonIF;
using ClipboardApp.Utils;
using QAChat.Model;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.PythonScriptView;

namespace ClipboardApp.Model {
    public class AutoProcessCommand {

        // 自動処理でテキストを抽出」を実行するコマンド
        public static ClipboardItem ExtractTextCommandExecute(ClipboardItem clipboardItem) {

            return PythonCommands.ExtractTextCommandExecute(clipboardItem);

        }

        // 自動処理でデータをマスキング」を実行するコマンド
        public static ClipboardItem MaskDataCommandExecute(ClipboardItem clipboardItem) {
            return PythonCommands.MaskDataCommandExecute(clipboardItem);
        }
        // 自動実行でPythonスクリプトを実行するコマンド
        public static void RunPythonScriptCommandExecute(ScriptItem scriptItem, ClipboardItem clipboardItem) {
            PythonCommands.RunPythonScriptCommandExecute(scriptItem, clipboardItem);

        }
        // 自動処理でファイルパスをフォルダとファイル名に分割するコマンド
        public static void SplitFilePathCommandExecute(ClipboardItem clipboardItem) {
            ClipboardItemCommands.SplitFilePathCommandExecute(clipboardItem);
        }

        public static void CreateAutoDescription(ClipboardItem item) {
            ClipboardItemCommands.CreateAutoDescription(item);
        }
        // 自動でタグを付与するコマンド
        public static void CreateAutoTags(ClipboardItem item) {
            ClipboardItemCommands.CreateAutoTags(item);
        }
        public static string FormatTextCommandExecute(string text) {
            return PythonCommands.FormatTextCommandExecute(text);
        }

        public static string CovertMaskedDataToOriginalData(MaskedData? maskedData, string maskedText) {
            return PythonCommands.CovertMaskedDataToOriginalData(maskedData, maskedText);

        }

        // 指定されたフォルダの全アイテムをマージするコマンド
        public static void MergeItemsCommandExecute(ClipboardItemFolder folder, ClipboardItem item) {
            ClipboardFolderCommands.MergeItemsCommandExecute(folder, item);
        }
        // 指定されたフォルダの中のSourceApplicationTitleが一致するアイテムをマージするコマンド
        public static void MergeItemsBySourceApplicationTitleCommandExecute(ClipboardItemFolder folder, ClipboardItem newItem) {
            ClipboardFolderCommands.MergeItemsBySourceApplicationTitleCommandExecute(folder, newItem);
        }
    }
}
