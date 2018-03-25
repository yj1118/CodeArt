using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Util;

namespace CodeArt.Web.WebPages.Xaml
{
    [ContentProperty("Assets")]
    public class AssetDirectory : DependencyObject
    {

        public static DependencyProperty AssetsProperty { get; private set; }

        public static DependencyProperty AssemblyNameProperty { get; private set; }

        public static DependencyProperty MapperProperty { get; private set; }

        static AssetDirectory()
        {
            var assetsMetadata = new PropertyMetadata(() => { return new DependencyCollection(); });
            AssetsProperty = DependencyProperty.Register<DependencyCollection, AssetDirectory>("Assets", assetsMetadata);

            var assemblyNameMetadata = new PropertyMetadata(() => { return string.Empty; });
            AssemblyNameProperty = DependencyProperty.Register<string, AssetDirectory>("AssemblyName", assemblyNameMetadata);

            var mapperMetadata = new PropertyMetadata(() => { return null; });
            MapperProperty = DependencyProperty.Register<AssetMapper, AssetDirectory>("Mapper", mapperMetadata);
        }

        [IgnoreBlank(true)]
        public DependencyCollection Assets
        {
            get
            {
                return GetValue(AssetsProperty) as DependencyCollection;
            }
            set
            {
                SetValue(AssetsProperty, value);
            }
        }

        /// <summary>
        /// 资产映射器，用于将.NET资源映射为XAML资产的工具
        /// </summary>
        public AssetMapper Mapper
        {
            get
            {
                return GetValue(MapperProperty) as AssetMapper;
            }
            set
            {
                SetValue(MapperProperty, value);
            }
        }

        private Func<string, AssetFile> _getFile = null;

        public AssetDirectory()
        {
            _getFile = LazyIndexer.Init<string, AssetFile>((path) =>
            {
                var assets = this.Assets;
                foreach (var item in assets)
                {
                    var file = item as AssetFile;
                    if (file != null)
                    {
                        if (file.Path == path) return file;
                    }
                    else
                    {
                        var package = item as AssetPackage;
                        if (package != null)
                        {
                            file = package.GetFile(path);
                            if (file != null) return file;
                        }
                    }
                }
                return null;
            });
        }

        public AssetFile GetFile(string key)
        {
            return _getFile(key);
        }

        /// <summary>
        /// 资产目录所在的程序集名称
        /// </summary>
        public string AssemblyName
        {
            get
            {
                return GetValue(AssemblyNameProperty) as string;
            }
            set
            {
                SetValue(AssemblyNameProperty, value);
            }
        }

        private bool _filesGenerated = false;

        private void GenerateFiles()
        {
            if (!_filesGenerated)
            {
                lock (this)
                {
                    if (!_filesGenerated)
                    {
                        _filesGenerated = true;
                        var assets = this.Assets;
                        var assemblyName = this.AssemblyName;
                        foreach (var item in assets)
                        {
                            var file = item as AssetFile;
                            if (file != null)
                            {
                                file.Generate(assemblyName, this.Mapper);
                            }
                            else
                            {
                                var assetString = item as AssetString;
                                if(assetString != null)
                                {
                                    assetString.Generate(assemblyName);
                                }
                                else
                                {
                                    var package = item as AssetPackage;
                                    if (package != null)
                                    {
                                        package.Generate(assemblyName,this.Mapper);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void OnInit()
        {
            if (this.Mapper == null) this.Mapper = new AssetMapper(this);
            GenerateFiles();
        }
    }
}
