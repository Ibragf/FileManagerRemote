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
        public ImageSource ImageSource { get; private set; }
        
        public byte[] fileData { get; set; }
        public byte[] imageData { get; set; }

        public void createImageSource(int number)
        {
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
            }
            ImageSource = image;
        }
    }
}
