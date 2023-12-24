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
    internal class LocalDataManager
    {
        private LocalDataManager() { }

        //private Dictionary<string, StorageData> _datas = new Dictionary<string, StorageData>();

        private Func<Guid, Func<string, StorageData>> _getData = LazyIndexer.Init<Guid, Func<string, StorageData>>((instanceId)=>
        {
            return LazyIndexer.Init<string, StorageData>((name)=>
            {
                return new StorageData();
            });
        });

        /// <summary>
        /// 根据分类名称，获得一个本地数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StorageData GetData(Guid instanceId, string name)
        {
            return _getData(instanceId)(name);
        }



        #region 基于当前应用程序回话的数据上下文


        private const string _sessionKey = "XamlLocalDataManager.Current";

        /// <summary>
        /// 获取或设置当前会话的数据上下文
        /// <para>如果用户没有手工指定数据上下文，那么使用配置文件配置的数据上下文 </para>
        /// <para>如果用户手工指定了数据上下文，那么使用用户指定的数据上下文</para>
        /// </summary>
        public static LocalDataManager Current
        {
            get
            {
                var localDataManager = AppSession.GetOrAddItem<LocalDataManager>(
                    _sessionKey,
                    () =>
                    {
                        return new LocalDataManager();
                    });
                if (localDataManager == null) throw new InvalidOperationException("XamlLocalDataManager.Current为null,无法使用xaml组件");
                return localDataManager;
            }
        }

        #endregion


    }
}
