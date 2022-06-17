using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Models
{
    internal class DirectoryModel : Model
    {
        public string LastWriteDate { get; set; }

        public DirectoryModel(string name, string path, string type, string shortDate):base(name, path, type)
        {
            LastWriteDate = shortDate;
        }
    }
}
