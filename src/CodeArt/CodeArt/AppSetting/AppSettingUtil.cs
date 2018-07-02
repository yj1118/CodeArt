using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;

using CodeArt;

namespace CodeArt.AppSetting
{
    public static class AppSettingUtil
    {
        public static string GetAttributeValue(XmlNode node, string name)
        {
            var attr = node.Attributes[name];
            if (attr == null) throw new XmlException(string.Format(Strings.NodeNoAttribute, node.Name, name));
            return attr.Value;
        }

        public static string TryGetAttributeValue(XmlNode node, string name)
        {
            var attr = node.Attributes[name];
            if (attr == null) return null;
            return attr.Value;
        }

        public static InterfaceMapper ParseInterfaceMapper(XmlNode section)
        {
            if (section == null) return null;
            InterfaceMapper mapper = new InterfaceMapper();
            var implements = section.SelectNodes("implement");
            foreach (XmlNode impNode in implements)
            {
                string contractTypeName = section.Attributes["contractType"].Value;
                Type contractType = Type.GetType(contractTypeName);
                if (contractType == null)
                    throw new NoTypeDefinedException(contractTypeName);

                var imp = ParseInterfaceImplement(impNode);
                if (imp != null)
                    mapper.AddImplement(contractType, imp);
            }
            return mapper;
        }

        public static InterfaceImplementer ParseInterfaceImplement(XmlNode section)
        {
            if (section == null) return null;
            string implementName = section.Attributes["implementType"].Value;
            Type impType = Type.GetType(implementName);
            if (impType == null)
                throw new NoTypeDefinedException(implementName);

            return new InterfaceImplementer(impType, null);
        }


    }
}
