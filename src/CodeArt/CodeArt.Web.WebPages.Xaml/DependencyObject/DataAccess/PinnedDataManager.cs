using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

using CodeArt.Util;
using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    internal class PinnedDataManager
    {
        private PinnedDataManager() { }

        private Dictionary<string, StorageData> _datas = new Dictionary<string, StorageData>();

        /// <summary>
        /// 根据分类名称，获得一个本地数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StorageData GetData(string name)
        {
            StorageData data = null;
            if(!_datas.TryGetValue(name,out data))
            {
                lock(_datas)
                {
                    if (!_datas.TryGetValue(name, out data))
                    {
                        data = new StorageData();
                        _datas.Add(name, data);
                    }
                }
            }
            return data;
        }



        private static Func<string, PinnedDataManager> _getManager = LazyIndexer.Init<string, PinnedDataManager>((virtualPath)=>
        {
            return new PinnedDataManager();
        });


        /// <summary>
        /// 固化数据是记录在内存中的，每个页面一个固化管理器
        /// </summary>
        public static PinnedDataManager Current
        {
            get
            {
                return _getManager(AccessContext.Current.VirtualPath);
            }
        }

    }
}
