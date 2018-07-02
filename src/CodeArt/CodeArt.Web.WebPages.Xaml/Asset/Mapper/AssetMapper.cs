using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 该对象主要用于解决.Net程序集资源的缺陷导致的站点文件的生成BUG，这包括
    /// 1.资源文件的目录不能有-，否则.net会自动转换为_
    /// 2.资源文件的路径是 a.b.c,我们无法从中推断出文件名是c,还是b.c
    /// 综上所述，我们需要根据实际情况引入配置文件来描述
    /// </summary>
    public class AssetMapper : IAssetMapper
    {
        private AssetDirectory _directory;

        #region 加载配置

        /// <summary>
        /// 资产片段名《=》资源片段名的双向映射表
        /// </summary>
        private Dictionary<string, string> _segments = new Dictionary<string, string>();

        /// <summary>
        /// 资产文件的短名称
        /// </summary>
        private List<string> _files = new List<string>();

        private static Func<string, (List<string> Files, Dictionary<string, string> Segments)> _loadFromConfig = LazyIndexer.Init<string, (List<string> Files, Dictionary<string, string> Segments)>(LoadFromConfig);

        private static (List<string> Files, Dictionary<string, string> Segments) LoadFromConfig(string assemblyName)
        {
            var _files = new List<string>();
            var _segments = new Dictionary<string, string>();

            var configName = string.Format("{0}.assets.xml,{0}", assemblyName);
            var xml = AssemblyResource.LoadText(configName);

            if(!string.IsNullOrEmpty(xml))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                var directories = doc.SelectNodes("config/directories/add");
                foreach(XmlNode directory in directories)
                {
                    var assetName = directory.GetAttributeValue("name");
                    var resourceName = assetName.Replace("-","_");

                    //双向添加
                    _segments.Add(assetName, resourceName);
                    _segments.Add(resourceName, assetName);
                }

                var files = doc.SelectNodes("config/files/add");
                foreach (XmlNode file in files)
                {
                    var name = file.GetAttributeValue("name");
                    _files.Add(name);
                }
            }

            return (_files, _segments);
        }

        #endregion


        public AssetMapper(AssetDirectory directory)
        {
            _directory = directory;
            var config = _loadFromConfig(directory.AssemblyName);
            _files = config.Files;
            _segments = config.Segments;
        }

        public string Correct(string pathSegment)
        {
            var result = string.Empty;
            if (_segments.TryGetValue(pathSegment, out result)) return result;
            return pathSegment;
        }

        public string GetAssetName(string resuorceName)
        {
            //以下代码是在匹配列表中找出最符合目标的修正名
            var result = _files.Where((assetName) =>
            {
                return resuorceName.EndsWith(assetName);
            }).OrderByDescending((t) =>
            {
                return t.Length;
            }).ToArray();

            if (result.Length > 0) return result[0];

            //不能简单的用以下代码匹配，因为会出现 velocity.min.js;animation.velocity.min.js 这种误判
            //也就是两个js文件，有部分后缀重叠了
            //foreach(var assetName in _files)
            //{
            //    if (resuorceName.EndsWith(assetName)) return assetName; //如果有修正名，那么直接使用
            //}

            var temp = resuorceName.Split('.');
            if (temp.Length == 0) return resuorceName;
            if (temp.Length == 1) return temp[0];
            return string.Format("{0}.{1}", temp[temp.Length - 2], temp[temp.Length - 1]);
        }

        /// <summary>
        /// 将资源路径转换为资产路径
        /// 或将资产路径转换为资源路径
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <returns></returns>
        public string MapPath(string resourcePath)
        {
            if (resourcePath.IndexOf('/') == -1) return Correct(resourcePath);
            string[] temp = resourcePath.Split('/');
            for (var i = 0; i < temp.Length; i++)
            {
                temp[i] = Correct(temp[i]);
            }
            return string.Join("/", temp);
        }
    }
}
