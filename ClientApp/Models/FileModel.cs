using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Models
{
    internal class FileModel : Model
    {
        public string LastWriteDate { get; set; }
        public string Extension { get; set; }
        public byte[] fileData { get; set; }
        public byte[] imageData { get; set; }


        public FileModel(string name, string path, string type, string date, string extension):base(name, path, type)
        {
            LastWriteDate = date;
            Extension = extension;
            Icon extractedIcon=Icon.ExtractAssociatedIcon(path);
            MemoryStream stream = null;
            if(extractedIcon!=null)
            {
                using (var bmp = extractedIcon.ToBitmap())
                {
                    stream = new MemoryStream();
                    bmp.Save(stream,System.Drawing.Imaging.ImageFormat.Png);
                    imageData=new byte[stream.Length];
                    stream.Write(imageData,0,imageData.Length);
                    stream.Flush();
                }
            }
        }
    }
}
