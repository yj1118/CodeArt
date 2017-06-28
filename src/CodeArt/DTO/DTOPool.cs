using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.AppSetting;

namespace CodeArt.DTO
{
    internal static class DTOPool
    {
        #region dto池

        private static Pool<DTObject> _objectPool = new Pool<DTObject>(() =>
        {
            return new DTObject();
        }, (obj, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                obj.Clear();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });


        public static DTObject CreateObject(DTEObject root, bool isReadOnly, bool isPinned)
        {
            var obj = isPinned ? new DTObject() : Symbiosis.TryMark(_objectPool, () =>
            {
                return new DTObject();
            });
            obj._root = root;
            obj.IsReadOnly = isReadOnly;
            obj.IsPinned = isPinned;
            return obj;
        }

        #endregion

        #region DTObject集合池

        private static Pool<List<DTObject>> _objectsPool = new Pool<List<DTObject>>(() =>
        {
            return new List<DTObject>();
        }, (objs, phase) =>
        {
            objs.Clear();
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //5分钟之内没有被使用，那么销毁
        });

        public static List<DTObject> CreateObjects(bool isPinned)
        {
            return isPinned ? new List<DTObject>() : Symbiosis.TryMark(_objectsPool, () =>
            {
                return new List<DTObject>();
            });
        }

        #endregion

        #region DTObjectList 池

        private static Pool<DTObjectList> _objectListPool = new Pool<DTObjectList>(() =>
        {
            return new DTObjectList();
        }, (obj, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                obj.Reset();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });

        public static DTObjectList CreateObjectList(DTEList owner, bool isPinned)
        {
            var list = isPinned ? new DTObjectList() : Symbiosis.TryMark(_objectListPool, () =>
              {
                  return new DTObjectList();
              });
            list.Init(owner, isPinned);
            return list;
        }

        #endregion

        #region DTObjects 池

        private static Pool<DTObjects> _DTObjectsPool = new Pool<DTObjects>(() =>
        {
            return new DTObjects();
        }, (obj, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                obj.Reset();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });

        public static DTObjects CreateDTOjects(IList<DTObject> items, bool isPinned)
        {
            var objs = isPinned ? new DTObjects() : Symbiosis.TryMark(_DTObjectsPool, () =>
               {
                   return new DTObjects();
               });
            objs.SetList(items);
            return objs;
        }

        #endregion

        #region DTEObject池 

        private static Pool<DTEObject> _dteObjectPool = new Pool<DTEObject>(() =>
        {
            return new DTEObject();
        }, (obj, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                obj.Reset();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });

        public static DTEObject CreateDTEObject(bool isPinned)
        {
            var dte = isPinned ? new DTEObject() : Symbiosis.TryMark(_dteObjectPool, () =>
             {
                 return new DTEObject();
             });
            dte.Init(isPinned);
            return dte;
        }

        public static DTEObject CreateDTEObject(List<DTEntity> members, bool isPinned)
        {
            var dte = CreateDTEObject(isPinned);
            dte.SetMembers(members);
            return dte;
        }

        #endregion

        #region DTEntity集合池

        private static Pool<List<DTEntity>> _entitiesPool = new Pool<List<DTEntity>>(() =>
        {
            return new List<DTEntity>();
        }, (entities, phase) =>
        {
            entities.Clear();
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //5分钟之内没有被使用，那么销毁
        });

        public static List<DTEntity> CreateDTEntities(bool isPinned)
        {
            return isPinned ? new List<DTEntity>() : Symbiosis.TryMark(_entitiesPool, () =>
            {
                return new List<DTEntity>();
            });
        }

        public static Pool<List<DTEntity>> EntitiesPool
        {
            get
            {
                return _entitiesPool;
            }
        }


        #endregion

        #region DTEList池 

        private static Pool<DTEList> _dteListPool = new Pool<DTEList>(() =>
        {
            return new DTEList();
        }, (obj, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                obj.Reset();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });

        public static DTEList CreateDTEList(bool isPinned)
        {
            return CreateDTEList(DTOPool.CreateObjects(isPinned), isPinned);
        }

        public static DTEList CreateDTEList(List<DTObject> members, bool isPinned)
        {
            var dte = isPinned ? new DTEList() : Symbiosis.TryMark(_dteListPool, () =>
            {
                return new DTEList();
            });
            dte.Init(isPinned, members);
            return dte;
        }

        public static DTEList CreateDTEList(string name, DTObject template, DTObjectList items, bool isPinned)
        {
            var dte = isPinned ? new DTEList() : Symbiosis.TryMark(_dteListPool, () =>
            {
                return new DTEList();
            });
            dte.InitByClone(name, template, items, isPinned);
            return dte;
        }

        #endregion



        #region DTEValue 池 

        private static Pool<DTEValue> _dteValuePool = new Pool<DTEValue>(() =>
        {
            return new DTEValue();
        }, (obj, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                obj.Reset();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });

        public static DTEValue CreateDTEValue(string name,object value, bool isPinned)
        {
            var dte = isPinned ? new DTEValue() : Symbiosis.TryMark(_dteValuePool, () =>
             {
                 return new DTEValue();
             });
            dte.Init(isPinned);
            dte.Name = name;
            dte.Value = value;
            return dte;
        }


        #endregion


        #region DTEValue 池 

        private static Pool<Dictionary<string,object>> _dicValuePool = new Pool<Dictionary<string, object>>(() =>
        {
            return new Dictionary<string, object>();
        }, (obj, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                obj.Clear();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });

        public static Dictionary<string, object> CreateDictionary(bool isPinned)
        {
            return isPinned ? new Dictionary<string, object>() : Symbiosis.TryMark(_dicValuePool, () =>
            {
                return new Dictionary<string, object>();
            });
        }


        #endregion
    }
}
