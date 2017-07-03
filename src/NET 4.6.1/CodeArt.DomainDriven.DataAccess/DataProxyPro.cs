using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Runtime;
using CodeArt.DomainDriven;
using CodeArt.Concurrent;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven.DataAccess
{
    internal class DataProxyPro : DataProxy
    {
        /// <summary>
        /// 从数据库中加载的数据
        /// </summary>
        public DynamicData OriginalData
        {
            get;
            private set;
        }

        /// <summary>
        /// 对象对应的数据表
        /// </summary>
        internal DataTable Table
        {
            get;
            private set;
        }

        public DataProxyPro(DynamicData originalData, DataTable table)
        {
            this.OriginalData = originalData;
            this.Table = table;
        }

        protected override object LoadData(DomainProperty property)
        {
            var tip = property.RepositoryTip;
            if (tip != null && tip.Lazy)
                return this.Table.ReadPropertyValue(this.Owner, tip, null, this.OriginalData);
            return null;
        }

        //public override void Clear()
        //{
        //    base.Clear();
        //    this.OriginalData = null;
        //    this.Table = null;
        //}

        //private static Pool<DataProxyPro> _pool = new Pool<DataProxyPro>(() =>
        //{
        //    return new DataProxyPro();
        //}, (data, phase) =>
        //{
        //    data.Clear();
        //    return true;
        //}, new PoolConfig()
        //{
        //    MaxRemainTime = 300 //闲置时间300秒
        //});


        ///// <summary>
        ///// 创建代理对象，生命周期与当前的共生器同步
        ///// </summary>
        ///// <returns></returns>
        //internal static DataProxyPro CreateWithSymbiosis()
        //{
        //    var temp = _pool.Borrow();
        //    Symbiosis.Current.Mark(temp);
        //    return temp.Item;
        //}
    }
}
