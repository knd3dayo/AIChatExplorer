using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace ClipboardApp.Model {
    // 検索条件ルールは
    // - 検索条件
    // 検索結果の保存先フォルダ(検索フォルダ)、検索対象フォルダ、検索対象サブフォルダを含むかどうかを保持する
    // IsGlobalSearchがTrueの場合は検索フォルダ以外のどのフォルダを読み込んでも、読み込みのタイミングで検索を行う
    // IsGlobalSearchがFalseの場合は検索フォルダのみ検索を行う
    // このクラスのオブジェクトはLiteDBに保存される
    public class SearchRule {

        public enum SearchType {
            // 標準 or 検索フォルダ
            Normal,
            SearchFolder
        }

        public ObjectId? Id { get; set; }

        public SearchCondition SearchCondition { get; set; }

        public ClipboardItemFolder? SearchFolder { get; set; }

        public ClipboardItemFolder? TargetFolder { get; set; }

        public string Name { get; set; } = "";

        // Type
        // 標準 or 検索フォルダ
        public SearchType Type { get; set; }

        // コンストラクタ
        public SearchRule() {
            SearchCondition = new SearchCondition();
            Type = SearchType.Normal;
        }

        // 保存
        public void Save() {
            ClipboardAppFactory.Instance.GetClipboardDBController().UpsertSearchRule(this);
        }   

    }
}
