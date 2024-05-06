using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using LiteDB;

namespace WpfAppCommon.Model {
    public class ClipboardItemImage {

        public ObjectId Id { get; set; } = ObjectId.Empty;

        // 画像イメージのBase64文字列
        public string ImageBase64 { get; set; } = String.Empty;

        // 画像イメージ
        public void SetImage(Image image) {
            using MemoryStream ms = new ();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ImageBase64 = Convert.ToBase64String(ms.ToArray());
        }
        public Image? GetImage() {
            if (string.IsNullOrEmpty(ImageBase64)) {
                return null;
            }
            byte[] imageBytes = Convert.FromBase64String(ImageBase64);
            using MemoryStream ms = new (imageBytes);
            return Image.FromStream(ms);
        }   
        public BitmapImage? GetBitmapImage() {
            if (string.IsNullOrEmpty(ImageBase64)) {
                return null;
            }
            byte[] binaryData = Convert.FromBase64String(ImageBase64);
            MemoryStream ms = new (binaryData, 0, binaryData.Length);
            BitmapImage bi = new ();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
        }
        // 画像データのサムネイル
        public Image? GetThumbnailImage() {
            Image? image = GetImage();
            if (image == null) {
                return null;
            }
            return image.GetThumbnailImage(100, 100, () => false, IntPtr.Zero);
        }
        // 画像データのサムネイルのBitmapImage
        public BitmapImage? GetThumbnailBitmapImage() {
            Image? image = GetThumbnailImage();
            if (image == null) {
                return null;
            }
            MemoryStream ms = new ();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage bi = new ();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
        }

        // 削除
        public void Delete() {
            ClipboardAppFactory.Instance.GetClipboardDBController().DeleteItemImage(this);
        }
        // 保存
        public void Save() {
            ClipboardAppFactory.Instance.GetClipboardDBController().UpsertItemImage(this);
        }
        // 取得
        public static ClipboardItemImage? GetItems(ObjectId objectId) {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetItemImage(objectId);
        }
    }
}
