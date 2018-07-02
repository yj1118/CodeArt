using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 为了避免在构造对象的时候，循环构建，所以我们引入了构造上下文的概念
    /// </summary>
    public static class ConstructContext
    {
        public static object Get(Type objectType, object id)
        {
            return Current.Get(objectType, id, id);
        }

        /// <summary>
        /// 从构造上下文中获取对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static object Get(Type objectType, object rootId, object id)
        {
            return Current.Get(objectType, rootId, id);
        }

        public static void Add(object rootId, object id, DomainObject obj)
        {
            Current.Add(rootId, id, obj);
        }

        public static void Add(object id, DomainObject obj)
        {
            Current.Add(id, id, obj);
        }


        public static void Remove(DomainObject obj)
        {
            Current.Remove(obj);
        }


        private class ConstructContextImpl
        {
            private List<Item> _items;

            public ConstructContextImpl()
            {
                _items = new List<Item>();
            }

            public void Add(object rootId, object id, DomainObject obj)
            {
                var item = _itemPool.Borrow();
                item.ObjectType = obj.ObjectType;
                item.RootId = rootId;
                item.Id = id;
                item.Target = obj;

                _items.Add(item);
            }

            public void Remove(object obj)
            {
                var item = _items.Remove((t) =>
                 {
                     return object.ReferenceEquals(t.Target, obj);
                 });
                if(item != null)
                {
                    _itemPool.Return(item);
                }
            }

            /// <summary>
            /// 从构造上下文中获取对象
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public object Get(Type objectType, object rootId, object id)
            {
                var item = _items.FirstOrDefault((t) =>
                {
                    return t.ObjectType.Equals(objectType) && t.RootId.Equals(rootId) && t.Id.Equals(id);
                });
                return item == null ? null : item.Target;
            }

            public void Clear()
            {
                _items.Clear();
            }
        }

        private class Item
        {
            public Type ObjectType { get; set; }

            public object RootId { get; set; }

            public object Id { get; set; }

            public object Target { get; set; }

            public Item()
            {
            }

            public void Clear()
            {
                this.ObjectType = null;
                this.RootId = null;
                this.Id = null;
                this.Target = null;
            }
        }


        #region 基于当前应用程序回话的数据上下文


        private const string _sessionKey = "__ConstructContext.Current";

        /// <summary>
        /// 获取或设置当前会话的数据上下文
        /// </summary>
        private static ConstructContextImpl Current
        {
            get
            {
                var context = AppSession.GetOrAddItem<ConstructContextImpl>(
                    _sessionKey,
                    () =>
                    {
                        return Symbiosis.TryMark<ConstructContextImpl>(_pool, () => { return new ConstructContextImpl(); });
                    });
                if (context == null) throw new InvalidOperationException("DataContext.Current为null,无法使用仓储对象");
                return context;
            }
        }


        #endregion

        #region 对象池

        private static Pool<ConstructContextImpl> _pool;

        private static PoolWrapper<Item> _itemPool;


        static ConstructContext()
        {
            _pool = new Pool<ConstructContextImpl>(() =>
            {
                return new ConstructContextImpl();
            }, (ctx, phase) =>
            {
                if (phase == PoolItemPhase.Returning)
                {
                    ctx.Clear();
                }
                return true;
            }, new PoolConfig()
            {
                MaxRemainTime = 300 //5分钟之内未被使用，就移除
            });


            _itemPool = new PoolWrapper<Item>(() =>
            {
                return new Item();
            }, (item, phase) =>
            {
                if (phase == PoolItemPhase.Returning)
                {
                    item.Clear();
                }
                return true;
            }, new PoolConfig()
            {
                MaxRemainTime = 300 //5分钟之内未被使用，就移除
            });
        }

        #endregion
    }
}
