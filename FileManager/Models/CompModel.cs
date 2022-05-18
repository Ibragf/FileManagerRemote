using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Models
{
    internal class CompModel: Model
    {
        public string ID { get; set; }
        public CompModel(string name, string type, string ID) : base(name, type)
        {
            this.ID = ID;
        }
    }
}
