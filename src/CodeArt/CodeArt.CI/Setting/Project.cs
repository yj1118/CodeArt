using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CodeArt.CI.Setting
{
    public class Project
    {
        public string Name
        {
            get;
            private set;
        }

        public string Path
        {
            get;
            private set;
        }

        public Project(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }
    }
}
