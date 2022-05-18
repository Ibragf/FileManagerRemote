using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FileManager.Models
{
    internal class SerializableClass
    {
        public List<DriveModel> Drivers;
        public List<DirectoryModel> Folders;
        public List<FileModel> Files;

        #region garbage
        /*public Image Image { get; }
        public string image
        {
            get
            {
                if (Image != null) return null;
                if(Element is DriveInfo) return "drive";
                if (Element is DirectoryInfo) return "folder";
                if (Element is string) return "comp";
                return null;
            }
        }

        public DriveModel toItemModel()
        {
            DriveModel itemModel = null;
            ImageSource imageSource;
            if (extension!=String.Empty) 
            {
                imageSource = toImageSource();
                itemModel = new ItemModel(name, lastWriteDate, extension, Element, imageSource);
            }
            else
            {
                imageSource=createIcon();
                itemModel=new ItemModel(name, lastWriteDate, Element, imageSource);
            }
            return itemModel;
        }

        private ImageSource toImageSource()
        {
            ImageSource imageSource=null;
            var png = new MemoryStream();
            Image.Save(png,System.Drawing.Imaging.ImageFormat.Png);
            imageSource=BitmapFrame.Create(png);

            return imageSource;
        }

        private ImageSource createIcon()
        {
            ImageSource imageSource = null;
            if (image is not null)
            {
                using (FileStream fs = new FileStream(@$"icon\{image}Icon.png", FileMode.Open))
                {
                    var png = new MemoryStream();

                    Image image = Image.FromStream(fs);
                    image.Save(png, System.Drawing.Imaging.ImageFormat.Png);
                    imageSource = BitmapFrame.Create(png);
                }
            }
            return imageSource;
        }*/
        #endregion
    }
}
