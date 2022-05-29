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
        public string Path { get; set; }
        public ImageSource ImageSource { get; private set; }
        
        public byte[] fileData { get; set; }
        public byte[] imageData { get; set; }

        public FileModel(string name, string type, string date, string extension, byte[] file, byte[] image) : base(name, type)
        {
            this.LastWriteDate = date;
            this.Extension = extension;
            this.fileData = file;
            this.imageData = image;
        }

        public void createImageSource(int number)
        {
            /*
            ImageSource imageSource = null;
            MemoryStream memoryStream = new MemoryStream(imageData);
            imageSource = BitmapFrame.Create(memoryStream);
            return imageSource;
            */
            ImageSource image = null;
            string path = @"D:\FILES\new"+number+Extension;

            File.Create(path);
            if(File.Exists(path))
            {
                Icon icon = Icon.ExtractAssociatedIcon(path);
                MemoryStream stream = null;
                if (icon != null)
                {
                    using (var bmp = icon.ToBitmap())
                    {
                        stream = new MemoryStream();

                        bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        image = BitmapFrame.Create(stream);

                    }
                }
                //File.Delete(path);
            }
            ImageSource = image;
        }

        /*
         * ImageSource image = null;
            Icon icon = Icon.ExtractAssociatedIcon(path);
            MemoryStream stream=null;
            if (icon != null)
            {
                using (var bmp = icon.ToBitmap())
                {
                    stream = new MemoryStream();
                    
                    bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    image = BitmapFrame.Create(stream);
                      
                }
            }
            return image;
         * 
         * */
    }
}
