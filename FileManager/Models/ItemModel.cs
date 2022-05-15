using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FileManager.Models
{
    public class ItemModel
    {
        public ItemModel(string name, string date, object Element)
        {
            this.name = name;
            lastWriteDate = date;
            this.Element = Element;
        }

        public ItemModel(string name, string date, object Element, ImageSource imageSource)
        {
            this.Image = imageSource;
            this.name = name;
            lastWriteDate = date;
            this.Element = Element;
        }

        public ItemModel(string name, string date, string extension, object Element, ImageSource imageSource)
        {
            this.name = name;
            this.extension = extension;
            this.Element = Element;
            this.Image = imageSource;
            lastWriteDate = date;
        }
        public string name { get; set; } = String.Empty;
        public string lastWriteDate { get; set; } = String.Empty;
        public object Element { get; set; } = null;
        public string extension { get; set; } = String.Empty;
        public ImageSource Image { get; set; } = null;
    }
}
