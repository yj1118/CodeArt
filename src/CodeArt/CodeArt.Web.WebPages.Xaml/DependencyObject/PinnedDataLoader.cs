using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 固化数据加载器
    /// 该对象是线程安全的
    /// </summary>
    internal class PinnedDataLoader
    {
        private Guid _objectId;

        public PinnedDataLoader(Guid objectId)
        {
            _objectId = objectId;
        }

        /// <summary>
        /// 占位值
        /// </summary>
        private static object _placeholderValue = new object();

        /// <summary>
        /// 正在加载固化数据的标示的数据编号
        /// </summary>
        private static Guid _loadingPinnedDataId = new Guid("{1060303F-3466-4FF5-9DE0-DA1D04A665BC}");

        /// <summary>
        /// 已加载过固化数据的标示的数据编号
        /// </summary>
        private static Guid _loadedPinnedDataId = new Guid("{EA1D452F-9D3E-417E-A6BF-9EF5C2515014}");

        private bool IsLoading(StorageData local)
        {
            return local.Load(_loadingPinnedDataId) == null ? false : true;
        }


        private void IsLoading(StorageData local,bool value)
        {
            local.Save(_loadingPinnedDataId, value ? _placeholderValue : null);
        }


        private bool IsLoaded(StorageData local)
        {
            return local.Load(_loadedPinnedDataId) == null ? false : true;
        }

        private void IsLoaded(StorageData local, bool value)
        {
            local.Save(_loadedPinnedDataId, value ? _placeholderValue : null);
        }

        /// <summary>
        /// 加载固化数据（将固化数据加载到本地数据中）
        /// </summary>
        public void Load(Action loadAction)
        {
            var local = LocalDataManager.Current.GetData(_objectId, "loadPinned"); //加载固化值的本地数据包
           
            bool loadedPinned = local.Load(_loadedPinnedDataId) == null ? false : true;

            if (this.IsLoaded(local) || this.IsLoading(local)) return;
            this.IsLoading(local, true);

            loadAction(); //执行加载固化数据的行为

            this.IsLoading(local, false);
            this.IsLoaded(local, true);
        }


    }
}
