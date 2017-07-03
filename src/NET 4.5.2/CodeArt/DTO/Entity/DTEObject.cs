using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Text;

using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.DTO
{
    /// <summary>
    /// 会对根据成员的name进行排序，以保证不同的dto只要键值相同，那么就可以通过GetCode()比较是否相等
    /// </summary>
    internal class DTEObject : DTEntity
    {
        public override DTEntityType Type => DTEntityType.Object;

        #region entities

        private List<DTEntity> _entities;

        public IEnumerable<DTEntity> GetEntities()
        {
            return _entities;
        }

        public DTEntity GetFirstEntity()
        {
            return _entities.First();
        }


        internal void SetMembers(List<DTEntity> entities)
        {
            _entities = entities;
            foreach (var e in _entities)
            {
                e.Parent = this;
            }
            //this.OrderMembers();
            Changed();
        }

        //private void OrderMembers()
        //{
        //    //先排序自身
        //    OrderSelfMembers();

        //    //再排序子项
        //    foreach (var member in _members)
        //    {
        //        var e = member as DTEObject;
        //        if (e != null) e.OrderMembers();
        //    }
        //    Changed();
        //}

        //private void OrderSelfMembers()
        //{
        //    if (_members.Count == 0) return;
        //    var orders = _members.OrderBy((m) =>
        //    {
        //        return m.Name;
        //    });
        //    using (var temp = DTOPool.EntitiesPool.Borrow())
        //    {
        //        var list = temp.Item;
        //        list.AddRange(orders);
        //        _members.Clear();
        //        _members.AddRange(list);
        //    }
               
        //    Changed();
        //}

        //public override void OrderEntities()
        //{
        //    this.OrderMembers();
        //}

        #endregion

        ///// <summary>
        ///// 出于性能考虑，我们直接传递 List集合
        ///// </summary>
        ///// <param name="members"></param>
        //public DTEObject(List<DTEntity> members)
        //{
        //    this.SetMembers(members);
        //}

        public DTEObject()
        {
        }

        public void Init(bool isPinned)
        {
            this.IsPinned = isPinned;
            this.SetMembers(DTOPool.CreateDTEntities(isPinned));
        }


        public override void Reset()
        {
            _entities.Clear();
            _entities = null;
            base.Reset();
        }


        #region 数据

        /// <summary>
        /// 仅克隆结构
        /// </summary>
        /// <returns></returns>
        public override DTEntity Clone()
        {
            var copy = DTOPool.CreateDTEntities(this.IsPinned);
            for (var i = 0; i < _entities.Count; i++)
            {
                copy.Add(_entities[i].Clone());
            }

            var dte = DTOPool.CreateDTEObject(copy, this.IsPinned);
            dte.Name = this.Name;
            return dte;
        }

        public override void ClearData()
        {
            foreach (var member in _entities)
                member.ClearData();
            Changed();
        }

        public override bool ContainsData()
        {
            foreach (var member in _entities)
                if (member.ContainsData()) return true;
            return false;
        }

        #endregion

        #region 实体管理

        public override IEnumerable<DTEntity> FindEntities(QueryExpression query)
        {
            if (query.IsSelfEntities) return GetSelfEntities();//查询自身

            var segment = query.Segment;

            var entity = _entities.FirstOrDefault((m) =>
            {
                return m.Name.Equals(segment, StringComparison.OrdinalIgnoreCase);
            });

            if (entity == null)
            {
                if (query.Next == null && _entities.Count == 1 && string.IsNullOrEmpty(_entities[0].Name))
                {
                    //例如:{[{id,name}]}  id=>id2 , {{name,sex}}  name=>name2
                    return _entities[0].FindEntities(query);
                }
                else
                {
                    if (query.IsEmpty) return _entities.ToArray();
                }
            }
            else
            {
                if (query.Next == null)
                {
                    // 没有后续查找
                    return entity.GetSelfEntities(); //等同于 new DTEntity[] { entity } 但是GetSelfEntities()可以节约内存
                }
                else
                {
                    return entity.FindEntities(query.Next);
                }
            }
            return EmptyArray<DTEntity>.Value;
        }

        /// <summary>
        /// 设置成员，当成员存在时覆盖，成员不存在时新增
        /// </summary>
        /// <param name="findExp"></param>
        /// <param name="value"></param>
        public override void SetEntity(QueryExpression query, Func<string, DTEntity> createEntity)
        {
            var segment = query.Segment;

            var entity = _entities.FirstOrDefault((m) =>
            {
                return m.Name.Equals(segment, StringComparison.OrdinalIgnoreCase);
            });

            if (entity != null)
            {
                if (query.Next == null)
                {
                    //覆盖，此处并没有改变name，所以不需要重新排序
                    var index = _entities.IndexOf(entity);
                    var e = createEntity(segment);
                    e.Parent = this;
                    _entities[index] = e;
                    Changed();
                }
                else
                {
                    //由于表达式还未完，所以在下个节点中继续执行
                    entity.SetEntity(query.Next, createEntity);
                }
            }
            else
            {
                //没有找到
                if (query.Next == null)
                {
                    //没有后续
                    var e = createEntity(segment);
                    e.Parent = this;
                    _entities.Add(e);
                    //OrderSelfMembers(); //重新排序
                    Changed();
                }
                else
                {
                    var next = DTOPool.CreateDTEObject(this.IsPinned);
                    next.Name = segment;
                    next.Parent = this;
                    _entities.Add(next);
                    //OrderSelfMembers(); //重新排序
                    Changed();

                    next.SetEntity(query.Next, createEntity);
                }
            }
        }

        public override void DeletEntity(DTEntity e)
        {
            _entities.Remove(e);
            //OrderSelfMembers();
            Changed();
        }

        #endregion

        public override void Changed()
        {
            if (this.Parent != null)
                this.Parent.Changed();
        }


        #region 代码


        public override string GetCode(bool sequential)
        {
            return InternalGetCode(sequential, (member) =>
            {
                return member.GetCode(sequential);
            });
        }

        public override string GetSchemaCode(bool sequential)
        {
            return InternalGetCode(sequential, (member) =>
            {
                return member.GetSchemaCode(sequential);
            });
        }

        private string InternalGetCode(bool sequential, Func<DTEntity, string> getMemberCode)
        {
            using (var temp = StringPool.Borrow())
            {
                StringBuilder code = temp.Item;
                if (!string.IsNullOrEmpty(this.Name))
                    code.AppendFormat("\"{0}\"", this.Name);

                if (code.Length > 0) code.Append(":");

                if (this.IsSingleValue())
                {
                    code.Append(getMemberCode(_entities[0]));
                }
                else
                {
                    code.Append("{");

                    IEnumerable<DTEntity> members = sequential ? _entities.OrderBy((m) =>
                    {
                        return m.Name;
                    }) : (IEnumerable<DTEntity>)_entities;

                    foreach (var member in members)
                    {
                        var memberCode = getMemberCode(member);
                        code.Append(memberCode);
                        code.Append(",");
                    }
                    if (_entities.Count > 0) code.Length--;
                    code.Append("}");
                }
                return code.ToString();
            }
        }

        #endregion


        /// <summary>
        /// 是否为{value}的形式
        /// </summary>
        /// <returns></returns>
        public bool IsSingleValue()
        {
            if (string.IsNullOrEmpty(this.Name) && _entities.Count == 1)
            {
                var member = _entities[0] as DTEValue;
                return member != null && string.IsNullOrEmpty(member.Name);
            }
            return false;
        }

        public object GetSingleValue()
        {
            return (_entities[0] as DTEValue).Value;
        }


    }
}