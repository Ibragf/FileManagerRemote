using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Models
{
    [Serializable]
    internal class DriveModel : Model
    {
        public DriveModel(string name, string path, string type) : base(name, path, type)
        {

        }
    }
}
