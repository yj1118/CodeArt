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
        /// 我们并不在数据代理中的原始数据里包含附加字段的值，因为这些值由程序员额外维护，我们不能保证对象缓冲区的对象中包含的数据和附加字段的数据是同步的
        /// 例如：在树状结构中，批量更改了LR的值，这时候数据缓冲区并不知道
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

        public DataProxyPro(DynamicData originalData, DataTable table, bool isMirror)
        {
            this.OriginalData = originalData;
            this.Table = table;
            _isMirror = isMirror;
        }

        protected override object LoadData(DomainProperty property)
        {
            var tip = property.RepositoryTip;
            if (tip != null)
            {
                var level = IsLoadByMirroring() ? QueryLevel.Mirroring : QueryLevel.None;
                return this.Table.ReadPropertyValue(this.Owner, tip, null, this.OriginalData, level);
            }
            return null;
        }

        /// <summary>
        /// 镜像对象的属性加载优先用镜像模式
        /// 如果处于提交阶段，那么使用无锁模式，在提交阶段加载数据一般是由于验证器的验证操作引起的
        /// </summary>
        /// <returns></returns>
        private bool IsLoadByMirroring()
        {
            if (!DataContext.ExistCurrent()) return false;
            var context = DataContext.Current;
            return !context.IsCommiting && this.Owner.IsMirror;
        }

        public override bool IsSnapshot
        {
            get
            {
                if (this.IsFromSnapshot) return true;
                if (this.Table.IsDynamic && this.OriginalData.ContainsKey(DataTable.Snapshot) && (bool)this.OriginalData.Get(DataTable.Snapshot)) return true;
                //通过对比数据版本号判定数据是否为快照
                var current = this.Version;
                var latest = this.Table.GetDataVersion(this.OriginalData);
                return current != latest; //当对象已经被删除，对象版本号大于数据库版本号，当对象被修改，当前对象版本号小于数据库版本号
            }
        }

        private bool _isMirror;

        public override bool IsMirror
        {
            get
            {
                return _isMirror;
            }
        }

        public override bool IsFromSnapshot
        {
            get
            {
                //如果这个对象来自快照表，那么它就是来自于仓储的快照存储区
                return this.Table.IsSnapshot;
            }
        }
        
        public override int Version
        {
            get
            {
                return (int)this.OriginalData.Get(GeneratedField.DataVersionName);
            }
            set
            {
                this.OriginalData.Set(GeneratedField.DataVersionName, value);
            }
        }

        public override void SyncVersion()
        {
            this.Version = this.Table.GetDataVersion(this.OriginalData);
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
