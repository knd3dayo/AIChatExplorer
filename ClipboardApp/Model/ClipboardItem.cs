using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using ClipboardApp.Utils;
using LiteDB;
using WK.Libraries.SharpClipboardNS;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace ClipboardApp.Model {
    public  enum ClipboardContentTypes {
        Text,
        Files,
        Image,
        Unknown
    }
    public class ClipboardItem {
        // コンストラクタ
        public ClipboardItem() {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        // プロパティ

        public ObjectId? Id { get; set; }

        public string? CollectionName { get; set; }

        // 生成日時
        public DateTime CreatedAt { get; set; }

        // 更新日時
        public DateTime UpdatedAt { get; set; }

        // クリップボードの内容
        public string Content { get; set; } = "";
        // クリップボードの内容の種類
        public ClipboardContentTypes ContentType { get; set; }

        //Tags
        public HashSet<string> Tags { get; set; } = new HashSet<string>();

        //説明
        public string Description { get; set; } = "";

        //　貼り付け元のアプリケーション名
        public string SourceApplicationName { get; set; } = "";
        //　貼り付け元のアプリケーションのタイトル
        public string SourceApplicationTitle { get; set; } = "";
        //　貼り付け元のアプリケーションのID
        public int? SourceApplicationID { get; set; }
        //　貼り付け元のアプリケーションのパス
        public string? SourceApplicationPath { get; set; }
        // ピン留め
        public bool IsPinned { get; set; }


        // -------------------------------------------------------------------
        // インスタンスメソッド
        // -------------------------------------------------------------------

        public ClipboardItem Copy() {
            ClipboardItem newItem = new ClipboardItem();
            CopyTo(newItem);
            return newItem;

        }
        public void CopyTo(ClipboardItem newItem) {
            newItem.UpdatedAt = UpdatedAt;
            newItem.Content = Content;
            newItem.ContentType = ContentType;
            newItem.SourceApplicationName = SourceApplicationName;
            newItem.SourceApplicationTitle = SourceApplicationTitle;
            newItem.SourceApplicationID = SourceApplicationID;
            newItem.SourceApplicationPath = SourceApplicationPath;
            newItem.Tags = new HashSet<string>(Tags);
            newItem.Description = Description;
        }

        public void MergeItems(List<ClipboardItem> items, bool mergeWithHeader, Action<ActionMessage> action) {
            if (this.ContentType != ClipboardContentTypes.Text) {
                action(ActionMessage.Error("Text以外のアイテムへのマージはできません"));
                return;
            }
            string mergeText = "\n";
            // 現在の時刻をYYYY/MM/DD HH:MM:SS形式で取得
            mergeText += "---\n";

            foreach (var item in items) {

                // Itemの種別がFileが含まれている場合はマージしない
                if (item.ContentType != ClipboardContentTypes.Files && item.ContentType != ClipboardContentTypes.Text) {
                    action(ActionMessage.Error("Fileが含まれているアイテムはマージできません"));
                    return;
                }
            }
            foreach (var item in items) {
                if (mergeWithHeader) {
                    // 説明がある場合は追加
                    if (Description != "") {
                        mergeText += item.Description + "\n";
                    }
                    // mergeTextにHeaderを追加
                    mergeText += item.HeaderText + "\n";
                }
                // Contentを追加
                mergeText += item.Content + "\n";

            }
            // mergeTextをContentに追加
            Content += mergeText;

            // Tagsのマージ。重複を除外して追加
            Tags.UnionWith(items.SelectMany(item => item.Tags));
        }

        // タグ表示用の文字列
        public string TagsString() {
            return string.Join(",", Tags);
        }

        public void SetApplicationInfo(ClipboardChangedEventArgs sender) {
            SourceApplicationName = sender.SourceApplication.Name;
            SourceApplicationTitle = sender.SourceApplication.Title;
            SourceApplicationID = sender.SourceApplication.ID;
            SourceApplicationPath = sender.SourceApplication.Path;
        }

        public string? HeaderText {
            get {
                string header1 = "";
                // 更新日時文字列を追加
                header1 += "[更新日時]" + UpdatedAt.ToString("yyyy/MM/dd HH:mm:ss") + "\n";
                // 作成日時文字列を追加
                header1 += "[作成日時]" + CreatedAt.ToString("yyyy/MM/dd HH:mm:ss") + "\n";
                // 貼り付け元のアプリケーション名を追加
                header1 += "[ソースアプリ名]" + SourceApplicationName + "\n";
                // 貼り付け元のアプリケーションのタイトルを追加
                header1 += "[ソースタイトル]" + SourceApplicationTitle + "\n";
                // Tags
                header1 += "[タグ]" + TagsString() + "\n";
                // ピン留め中かどうか
                if (IsPinned) {
                    header1 += "[ピン留めしてます]\n";
                }

                if (ContentType == ClipboardContentTypes.Text) {
                    return header1 + "[種類]Text";
                } else if (ContentType == ClipboardContentTypes.Files) {
                    return header1 + "[種類]File";
                } else if (ContentType == ClipboardContentTypes.Image) {
                    return header1 + "[種類]Image";
                } else {
                    return header1 + "[種類]Unknown";
                }
            }
        }
        //--------------------------------------------------------------------------------
        // staticメソッド
        //--------------------------------------------------------------------------------

        // ClipboardItemをJSON文字列に変換する
        public static string ToJson(ClipboardItem item) {
            var options = new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            return System.Text.Json.JsonSerializer.Serialize(item, options);
        }
        // JSON文字列をClipboardItemに変換する
        public static ClipboardItem? FromJson(string json, Action<ActionMessage> action) {
            var options = new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            ClipboardItem? item = System.Text.Json.JsonSerializer.Deserialize<ClipboardItem>(json, options);
            if (item == null) {
                action(ActionMessage.Error("JSON文字列をClipboardItemに変換できませんでした"));
                return null;
            }

            return item;

        }

        // 自分自身をDBに保存する
        public void Save() {
            ClipboardAppFactory.Instance.GetClipboardDBController().UpsertItem(this);
        }
        // 自分自身をDBから削除する
        public void Delete() {
            ClipboardAppFactory.Instance.GetClipboardDBController().DeleteItem(this);
            // DBから削除したらIdをnullにする
            Id = null;
        }
    }

}
