using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CodeArt.Util;

namespace CodeArt.CI.Setting
{
    public class Workspace
    {
        public IEnumerable<Project> Projects
        {
            get;
            private set;
        }

        public Project GetProject(string name)
        {
            if (this.Projects == null) return null;
            return this.Projects.FirstOrDefault((project) => project.Name.EqualsIgnoreCase(name));
        }


        internal Workspace()
        {
        }


        public void LoadFrom(XmlNode section)
        {
            List<Project> projects = new List<Project>();
            var projectNodes = section.SelectNodes("projects/add");
            foreach (XmlNode node in projectNodes)
            {
                var name = node.GetAttributeValue("name", string.Empty);
                var path = node.GetAttributeValue("path", string.Empty);
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(path)) continue;
                projects.Add(new Project(name, path));
            }
            this.Projects = projects;
        }
    }
}
