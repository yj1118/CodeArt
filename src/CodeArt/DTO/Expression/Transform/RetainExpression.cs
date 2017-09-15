using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.DTO
{
    /// <summary>
    /// 
    /// </summary>
    internal class RetainExpression : TransformExpression
    {
        private string[] _findExps;

        public RetainExpression(string exp)
        {
            _findExps = exp.Substring(1).Split(',').Select((temp) =>
            {
                return temp.Trim();
            }).ToArray();
        }

        public override void Execute(DTObject dto)
        {
            RetainEntities(dto, _findExps);
        }

        #region 保留成员

        private struct ReservedInfo
        {
            public DTEntity Entity
            {
                get;
                private set;
            }

            /// <summary>
            /// 是否为完整保留
            /// </summary>
            public bool IsComplete
            {
                get;
                private set;
            }

            public ReservedInfo(DTEntity entity, bool isComplete)
            {
                this.Entity = entity;
                this.IsComplete = isComplete;
            }

            public bool IsEmpty()
            {
                return this.Entity == null;
            }

        }

        private void RetainEntities(DTObject dto, string[] findExps)
        {
            if (findExps.Length == 0) return;

            //收集需要保留的实体
            var targets = new List<ReservedInfo>();
            foreach (var findExp in findExps)
            {
                var items = dto.FindEntities(findExp, false);
                foreach (var item in items)
                {
                    targets.Add(new ReservedInfo(item, true));
                    //加入自身
                    //再加入父亲，由于自身需要保留，所以父亲也得保留
                    var parent = item.Parent;
                    while (parent != null)
                    {
                        targets.Add(new ReservedInfo(parent, false));
                        parent = parent.Parent;
                    }
                }
            }
            targets = targets.Distinct().ToList(); //过滤重复

            var removes = new List<DTEntity>();
            CollectNeedRemove(dto.GetRoot().GetEntities(), targets, removes);

            foreach (var t in removes)
            {
                var parent = t.Parent;
                if (parent == null) throw new DTOException("预期之外的错误，" + string.Join(";", findExps));
                parent.DeletEntity(t);
            }

        }

        private void CollectNeedRemove(IEnumerable<DTEntity> members, List<ReservedInfo> reservedRemoves, List<DTEntity> removes)
        {
            foreach (var member in members)
            {
                var findItem = reservedRemoves.FirstOrDefault((t) =>
                {
                    return t.Entity == member;
                });

                if (findItem.IsEmpty())
                {
                    //不在保留列表中，那么加入删除列表
                    removes.Add(member);
                }
                else
                {
                    if (findItem.IsComplete) continue; //如果是完整匹配，那么子项也会保留

                    //在保留列表中，继续判断子项是否保留
                    var obj = member as DTEObject;
                    if (obj != null)
                    {
                        CollectNeedRemove(obj.GetEntities(), reservedRemoves, removes);
                    }
                    else
                    {
                        var list = member as DTEList;
                        if (list != null)
                        {
                            var childs = list.GetObjects().Select((t) =>
                            {
                                return t.GetRoot();
                            }).ToArray();
                            CollectNeedRemove(childs, reservedRemoves, removes);
                        }
                    }
                }
            }
        }

        #endregion


    }
}
