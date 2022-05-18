using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FileManager.Models
{
    internal class FileModel : Model
    {
        public string LastWriteDate { get; set; }
        public string Extension { get; set; }
        public ImageSource ImageSource
        {
            get { return toImageSource(); }
            set { }
        }
        private byte[] fileData;
        private byte[] imageData;

        public FileModel(string name, string type, string date, string extension, byte[] file, byte[] image) : base(name, type)
        {
            this.LastWriteDate = date;
            this.Extension = extension;
            this.fileData = file;
            this.imageData = image;
        }

        private ImageSource toImageSource()
        {
            ImageSource imageSource = null;
            MemoryStream memoryStream = new MemoryStream(imageData);
            imageSource = BitmapFrame.Create(memoryStream);
            return imageSource;
        }
    }
}
