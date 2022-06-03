using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FileManager.Models
{
    public abstract class Model
    {
        public string Name { get; set; } = String.Empty;
        public string Type { get; set; } = null;
        public string Path { get; set; }
        public virtual ImageSource ImageSource { get { return createIcon(); } set { } }
        public Model()
        {

        }

        public Model(string name, string type)
        {
            this.Name = name;
            this.Type = type;
        }

        protected ImageSource createIcon()
        {
            ImageSource imageSource = null;
            if (Type is not null && Type != "file")
            {
                using (FileStream fs = new FileStream(@$"icon\{Type}Icon.png", FileMode.Open))
                {
                    var png = new MemoryStream();

                    Image image = Image.FromStream(fs);
                    image.Save(png, System.Drawing.Imaging.ImageFormat.Png);
                    imageSource = BitmapFrame.Create(png);
                }
            }
            return imageSource;
        }
    }
}
