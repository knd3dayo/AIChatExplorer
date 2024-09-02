using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace PythonAILib.Model {
    public abstract class ImageItemBase {

        // 画像イメージのBase64文字列
        public string ImageBase64 { get; set; } = string.Empty;

        // 画像イメージ
        [BsonIgnore]
        public Image? Image {
            get {
                if (string.IsNullOrEmpty(ImageBase64)) {
                    return null;
                }
                byte[] imageBytes = Convert.FromBase64String(ImageBase64);
                using MemoryStream ms = new(imageBytes);
                return Image.FromStream(ms);
            }
            set {
                using MemoryStream ms = new();
                value?.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ImageBase64 = Convert.ToBase64String(ms.ToArray());
            }
        }
        [BsonIgnore]
        public BitmapImage? BitmapImage {
            get {
                if (string.IsNullOrEmpty(ImageBase64)) {
                    return null;
                }
                byte[] binaryData = Convert.FromBase64String(ImageBase64);
                return ContentTypes.GetBitmapImage(binaryData);
            }
        }

        // 削除
        public abstract void Delete();
        // 保存
        public abstract void Save();

    }
}
