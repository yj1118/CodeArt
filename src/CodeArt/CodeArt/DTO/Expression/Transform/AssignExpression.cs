using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.DTO
{
    /// <summary>
    /// 该方法主要用于更改成员值的类型
    /// findExp=valueFindExp
    /// 说明：
    /// valueFindExp 可以包含检索方式，默认的方式是在findExp检索出来的结果中所在的DTO对象中进行检索
    /// 带“@”前缀，表示从根级开始检索
    /// 带“*”前缀，表示返回值所在的对象
    /// </summary>
    internal class AssignExpression : TransformExpression
    {
        private string _findExp;
        private string _valueFindExp;

        public AssignExpression(string exp)
        {
            var temp = exp.Split('=');
            _findExp = temp[0].Trim();
            _valueFindExp = temp[1].Trim();
        }

        public override void Execute(DTObject dto)
        {
            Execute(dto, (v) => { return v; });
        }

        public void Execute(DTObject dto, Func<object, object> transformValue)
        {
            ChangeValue(dto, _findExp, _valueFindExp, transformValue);
        }

        #region 更改值

        /// <summary>
        /// 该方法用于更改成员的值
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="findExp"></param>
        /// <param name="valueFindExp"></param>
        /// <param name="transformValue"></param>
        public void ChangeValue(DTObject dto, string findExp, string valueFindExp, Func<object, object> transformValue)
        {
            ArgumentAssert.IsNotNullOrEmpty(findExp, "findExp");
            ArgumentAssert.IsNotNullOrEmpty(valueFindExp, "valueFindExp");

            //1.找出需要赋值的目标成员
            var targets = dto.FindEntities(findExp, false);
            if (targets.Length == 0) dto.SetValue(findExp, string.Empty); //如果没有成员，就自动生成
            targets = dto.FindEntities(findExp, false);

            var valueExpression = _getValueExpression(valueFindExp);

            foreach (var target in targets)
            {
                var parent = target.Parent as DTEObject;
                if (parent == null) throw new DTOException("预期之外的错误，" + valueExpression.FindExp);

                var parentDTO = valueExpression.StartRoot ? dto : new DTObject(parent, dto.IsReadOnly);

                //2.找出值，值是在目标成员所在的对象下进行查找的
                var entities = parentDTO.FindEntities(valueExpression.FindExp, false);
                if (entities.Length == 1)
                {
                    //获取值
                    var ve = entities[0];
                    var newValue = GetValue(ve, transformValue, dto.IsReadOnly);
                    //if (newValue == null) throw new DTOException("预期之外的数据转换，" + valueExpression.FindExp);
                    if (newValue == null) continue; //传递的值是null，就表明调用者要忽略这条数据


                    //目标值是唯一的，这个时候要进一步判断
                    var valueObjParent = ve.Parent.Parent as DTEList; //这是值所在的对象的父亲
                    //if (valueObjFather != null && ve!=target)  //如果值所在的对象处在集合中，并且不是自身对自身赋值，那么还是要以集合形式赋值
                    if (valueObjParent != null && ve.Parent != target.Parent)  //如果值所在的对象处在集合中，并且不是自身对象对自身对象赋值，那么还是要以集合形式赋值
                    {
                        //以集合赋值
                        SetValue(target, new object[] { newValue }, valueExpression.FindExp);
                    }
                    else
                    {
                        //赋单值
                        SetValue(target, newValue, valueExpression.FindExp);
                    }
                }
                else if (entities.Length > 1)
                {
                    //如果目标值是多个，那么是集合类型，这时候需要收集所有转换后的值，再赋值
                    List<object> values = new List<object>(entities.Length);
                    foreach (var e in entities)
                    {
                        var newValue = GetValue(e, transformValue, dto.IsReadOnly);
                        if (newValue == null) throw new DTOException("预期之外的数据转换，" + valueExpression.FindExp);
                        values.Add(newValue);
                    }

                    SetValue(target, values, valueExpression.FindExp);
                }
                else
                {
                    //值为0,需要判断是否为数组
                    var path = _getFindExpPath(valueExpression.FindExp);
                    bool isArray = false;
                    foreach(var exp in path)
                    {
                        var ent = dto.FindEntity(exp, false);
                        if (ent == null) break;
                        isArray = ent is DTEList;
                        if (isArray) break;
                    }
                    if (isArray)
                        SetValue(target, Array.Empty<object>(), valueExpression.FindExp);
                    else
                    {
                        var newValue = transformValue(null);
                        SetValue(target, newValue, valueExpression.FindExp);
                    }
                        
                }
            }
        }

        private void SetValue(DTEntity target, object value, string findExp)
        {
            var parent = target.Parent as DTEObject;
            if (parent == null) throw new DTOException("表达式错误" + findExp);

            var query = QueryExpression.Create(target.Name);
            parent.SetEntity(query, (name) =>
            {
                var dtoValue = value as DTObject;
                if (dtoValue != null)
                {
                    var t = dtoValue.Clone();
                    var newEntity = t.GetRoot();
                    newEntity.Name = name;
                    return newEntity;
                }
                else
                {
                    DTObject t = DTObject.Create();
                    t.SetValue(value);
                    var newEntity = t.GetRoot().GetFirstEntity();
                    if (newEntity == null) throw new DTOException("预期之外的错误，" + findExp);
                    newEntity.Name = name;
                    return newEntity;
                }
            });
        }


        private object GetValue(DTEntity e, Func<object, object> transformValue, bool isReadOnly)
        {
            var ve = e as DTEValue;
            if (ve != null) return transformValue(ve.Value);

            var le = e as DTEList;
            if (le != null)
            {
                var list = le.GetObjects();
                var value = list.Select((item) =>
                {
                    if (item.IsSingleValue) return item.GetValue();
                    return item;
                }).ToArray();
                return transformValue(value);
            }

            var oe = e as DTEObject;
            if (oe != null)
            {
                var value = new DTObject(oe, isReadOnly);
                return transformValue(value);
            }
            return null;
        }


        #endregion

        private static Func<string, string[]> _getFindExpPath = LazyIndexer.Init<string, string[]>((valueFindExp) =>
        {
            return valueFindExp.Split('.');
        });


        private static Func<string, ValueTuple> _getValueExpression = LazyIndexer.Init<string, ValueTuple>((valueFindExp) =>
         {
             bool startRoot = valueFindExp.StartsWith("@");
             if (startRoot) valueFindExp = valueFindExp.Substring(1);
             return new ValueTuple(startRoot, valueFindExp);
         });



        private class ValueTuple
        {
            public bool StartRoot { get; private set; }
            public string FindExp { get; private set; }

            public ValueTuple(bool startRoot, string findExp)
            {
                this.StartRoot = startRoot;
                this.FindExp = findExp;
            }
        }

    }
}
