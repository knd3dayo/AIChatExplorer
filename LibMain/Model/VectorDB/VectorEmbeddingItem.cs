using LibMain.Common;
using LibMain.Model.Chat;
using LibMain.Model.Content;
using LibMain.PythonIF;
using LibMain.PythonIF.Request;
using LibMain.Resources;
using LibMain.Utils.Common;

namespace LibMain.Model.VectorDB {
    public class VectorEmbeddingItem {

        public VectorEmbeddingItem() { }

        public VectorEmbeddingItem(string source_id, string folderPath, string vectorDBName) {
            SourceId = source_id;
            FolderPath = folderPath;
            VectorDBName = vectorDBName;
        }

        public string VectorDBName { get; set; } = "";
        public string Content { get; set; } = "";
        public string SourceId { get; set; } = "";

        // metadata

        public string? FolderPath { get; set; } = null;

        public VectorSourceType SourceType { get; set; } = VectorSourceType.None;

        public string Description { get; set; } = "";
        public string SourcePath { get; set; } = "";

        public string ImageUrl { get; set; } = "";

        public Dictionary<string, string> Tags { get; set; } = [];

        public string DocId { get; set; } = string.Empty;

        public double Score { get; set; } = 0.0;

        public List<VectorEmbeddingItem> SubDocs { get; set; } = [];

        public void SetMetadata(string folder_path, string description, string content, VectorSourceType sourceType, string source_path, Dictionary<string, string> tags) {
            Description = description;
            Content = content;
            FolderPath = folder_path;
            SourceType = sourceType;
            SourcePath = source_path;
            Tags = tags;
        }

        public async Task SetMetadata(ContentItem item) {
            ContentFolderWrapper folder = await item.GetFolderAsync();
            var contentFolderPath = await folder.GetContentFolderPath();
            // タイトルとHeaderTextを追加
            var hederText = await item.GetHeaderTextAsync();
            string description = item.Description + "\n" + hederText;
            // タグを取得
            Dictionary<string, string> tags = item.Tags.ToDictionary(tag => tag, tag => tag);
            if (item.ContentType == ContentItemTypes.ContentItemTypeEnum.Text) {
                SetMetadata(contentFolderPath, description, item.Content, VectorSourceType.Clipboard, item.SourcePath, tags);
            } else {
                SetMetadata(contentFolderPath, description, item.Content, VectorSourceType.File, item.SourcePath, tags);
            }
        }

    }
}
