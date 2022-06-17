using System;
using System.Drawing;


namespace ClientApp.Models
{
    [Serializable]
    public abstract class Model
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }

        public Model()
        {

        }

        public Model(string name, string path, string type)
        {
            this.Name = name;
            this.Type = type;
            this.Path = path;
        }
    }
}

