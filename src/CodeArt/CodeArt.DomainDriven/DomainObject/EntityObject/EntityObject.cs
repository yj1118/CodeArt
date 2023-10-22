using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    [MergeDomain]
    [FrameworkDomain]
    public abstract class EntityObject : DomainObject, IEntityObject
    {
        /// <summary>
        /// 统一领域对象中的标识符名称，这样在ORM处理等操作中会比较方便
        /// </summary>
        public const string IdPropertyName = "Id";

        public abstract object GetIdentity();

        protected EntityObject()
        {
            this.OnConstructed();
        }

        public override bool Equals(object obj)
        {
            var target = obj as EntityObject;
            if (target == null) return false;
            return this.GetIdentity() == target.GetIdentity();
        }

        public override int GetHashCode()
        {
            var key = this.GetIdentity();
            if (key != null) return key.GetHashCode();
            return 0;
        }

        public static bool operator ==(EntityObject entity0, EntityObject entity1)
        {
            if ((object)entity0 == null && (object)entity1 == null) return true;
            if ((object)entity0 == null || (object)entity1 == null) return false;
            return entity0.GetIdentity() == entity1.GetIdentity();
        }

        public static bool operator !=(EntityObject entity0, EntityObject entity1)
        {
            return !(entity0 == entity1);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="OT">对象类型</typeparam>
    /// <typeparam name="KT">识别对象的唯一编号的类型</typeparam>
    [MergeDomain]
    [FrameworkDomain]
    public abstract class EntityObject<TObject, TIdentity> : EntityObject
        where TObject : EntityObject<TObject, TIdentity>
        where TIdentity : struct
    {
        /// <summary>
        /// 引用对象的唯一标示
        /// 引用对象的标示可以是本地(内聚范围内)唯一也可以是全局唯一
        /// </summary>
        [PropertyRepository(Snapshot = true)]                                               //TObject是子类的类型，EntityObject<TObject, TIdentity>才是当前类的类型
        public static readonly DomainProperty IdProperty = DomainProperty.Register<TIdentity, EntityObject<TObject, TIdentity>>(IdPropertyName, default(TIdentity));

        //private TIdentity _id;

        ///// <summary>
        ///// 引用对象的唯一标示
        ///// 引用对象的标示可以是本地(内聚范围内)唯一也可以是全局唯一
        ///// </summary>
        //[PropertyRepository(Snapshot = true)]
        //public TIdentity Id
        //{
        //    get { return GetValue<TIdentity>(IdProperty, _id); }
        //    private set
        //    {
        //        SetValue(IdProperty, ref _id, value);
        //    }
        //}


        public TIdentity Id
        {
            get { return GetValue<TIdentity>(IdProperty); }
            private set
            {
                SetValue(IdProperty, value);
            }
        }

        public EntityObject(TIdentity id)
        {
            this.Id = id;
            this.OnConstructed();
        }

        public override bool Equals(object obj)
        {
            TObject target = obj as TObject;
            if (target == null) return false;
            return target.Id.Equals(this.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override object GetIdentity()
        {
            return this.Id;
        }

        public static bool operator ==(EntityObject<TObject, TIdentity> entity0, EntityObject<TObject, TIdentity> entity1)
        {
            if ((object)entity0 == null && (object)entity1 == null) return true;
            if ((object)entity0 == null || (object)entity1 == null) return false;
            return entity0.Equals(entity1);
        }

        public static bool operator !=(EntityObject<TObject, TIdentity> entity0, EntityObject<TObject, TIdentity> entity1)
        {
            return !(entity0 == entity1);
        }
    }

    [MergeDomain]
    [FrameworkDomain]
    public abstract class EntityObject<TObject, TIdentity, TObjectEmpty> : EntityObject<TObject, TIdentity>
            where TObject : EntityObject<TObject, TIdentity>
            where TIdentity : struct
            where TObjectEmpty : TObject, new()
    {
        public EntityObject(TIdentity id)
            : base(id)
        {
            this.OnConstructed();
        }

        private static TObject _empty;
        private static object _syncObject = new object();

        public static TObject Empty
        {
            get
            {
                if (_empty == null)
                {
                    lock (_syncObject)
                    {
                        if (_empty == null)
                        {
                            _empty = new TObjectEmpty();
                        }
                    }
                }
                return _empty;
            }
        }
    }

}