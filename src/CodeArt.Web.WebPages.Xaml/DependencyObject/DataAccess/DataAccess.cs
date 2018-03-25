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
    internal class DataAccess
    {
        public Guid InstanceId
        {
            get;
            private set;
        }

        /// <summary>
        /// 数据访问器的名称
        /// </summary>
        public string Name
        {
            get;
            private set;
        }


        /// <summary>
        /// 本地数据，本地数据是当前线程访问时的唯一数据
        /// </summary>
        private StorageData LocalData
        {
            get
            {
                return LocalDataManager.Current.GetData(this.InstanceId, this.Name);
            }
        }

        /// <summary>
        /// 固化数据，固化数据是所有线程都共享的数据，由于每个控件在一个页面路径下是唯一的，因此直接将
        /// DataContext与空间引用即可达到线程共享的目的
        /// </summary>
        private StorageData _pinnedData = new StorageData();

        /// <summary>
        /// 
        /// </summary>
        public DataAccess(Guid instanceId, string name)
        {
            this.InstanceId = instanceId;
            this.Name = name;
            //_pinnedData = PinnedDataManager.Current.GetData(this.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="getDefaultValue"></param>
        /// <param name="forcePinned">指示是否强制从固化数据中获取值</param>
        /// <returns></returns>
        public object Load(Guid dataId, Func<object> getDefaultValue)
        {
            var data = this.LocalData;
            if (!data.Contains(dataId))
            {
                var value = getDefaultValue == null ? null : getDefaultValue();
                Save(dataId, value);
                return value;
            }
            return data.Load(dataId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="value"></param>
        public void Save(Guid dataId, object value)
        {
            var localData = this.LocalData;
            if (LoadContext.IsLoading)
            {
                //在加载状态下赋的值，我们认为是需要固化的值
                this.SavePinned(dataId, value);
            }
            localData.Save(dataId, value);
        }

        private void SavePinned(Guid dataId, object value)
        {
            _pinnedData.Save(dataId, value);
        }

        /// <summary>
        /// 用固化值恢复数据
        /// </summary>
        public void Restore(Type objType)
        {
            var localData = this.LocalData;
            localData.Clear();
            foreach (var p in _pinnedData)
            {
                var id = p.Key;
                var value = p.Value;
                var curable = value as ICurable;
                if (curable != null) curable.LoadPinned();
                localData.Save(id, value);
            }
        }

    }
}
