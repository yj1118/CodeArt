using CodeArt.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CodeArt.CI.Setting
{
    public class Jenkins
    {
        /// <summary>
        /// Jenkins所在的路径
        /// </summary>
        public string Path
        {
            get;
            private set;
        }

        public Jenkins(string path)
        {
            this.Path = path;
        }


        internal Jenkins()
        {
        }

        public void LoadFrom(XmlNode section)
        {
            var path = section.GetAttributeValue("path",string.Empty);
            this.Path = path;
        }
    }
}
