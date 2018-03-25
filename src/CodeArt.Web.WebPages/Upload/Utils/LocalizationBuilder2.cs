namespace CodeArt.Web.WebPages
{
    using System;
    using System.Text;
    using System.Web;
    using System.Xml;

    internal class LocalizationBuilder2
    {
        private string[] _files;
        private StringBuilder _jsBuilder = new StringBuilder();
        private string _language;
        private string _path = ".";

        public LocalizationBuilder2(string language, string path, params string[] files)
        {
            this._language = language.Replace("_", "-");
            this._path = path;
            this._files = files;
            this.CreateLocalizationObject();
        }

        private void CreateLocalizationObject()
        {
            this._jsBuilder.AppendFormat("RadUploadNameSpace.Localization.ProcessRawArray(['{0}'", this._language);
            this.ProcessLocalizationFiles(this._files);
            this._jsBuilder.Append("]);");
        }

        private XmlDocument LoadLocalizationFile(string fileName)
        {
            if (("" + fileName).Length == 0)
            {
                return null;
            }
            if (!fileName.ToLower().EndsWith(".xml"))
            {
                fileName = fileName + ".xml";
            }
            string path = string.Format("{0}Localization/{1}/{2}", this._path, this._language, fileName);
            path = HttpContext.Current.Server.MapPath(path);
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(path);
                return document;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void ProcessLocalizationFiles(string[] fileNames)
        {
            if (null != fileNames)
            {
                for (int i = 0; i < fileNames.Length; i++)
                {
                    this.ProcessLocalizationXml(this.LoadLocalizationFile(fileNames[i]));
                }
            }
        }

        private void ProcessLocalizationXml(XmlDocument xmlLocalization)
        {
            if (null != xmlLocalization)
            {
                XmlNodeList childNodes = xmlLocalization.DocumentElement.ChildNodes;
                for (int i = 0; i < childNodes.Count; i++)
                {
                    XmlNode node = childNodes[i];
                    string str = string.Empty;
                    XmlAttribute attribute = node.Attributes["id"];
                    if (null != attribute)
                    {
                        str = attribute.Value;
                    }
                    if (string.Empty != str)
                    {
                        this._jsBuilder.AppendFormat(",\"{0}\",\"{1}\"", str, node.InnerText);
                    }
                }
            }
        }

        public string ToScriptString()
        {
            return string.Format("<script type=\"text/javascript\">{0}</script>{1}", this._jsBuilder.ToString(), Environment.NewLine);
        }

        public override string ToString()
        {
            return this._jsBuilder.ToString();
        }
    }
}

