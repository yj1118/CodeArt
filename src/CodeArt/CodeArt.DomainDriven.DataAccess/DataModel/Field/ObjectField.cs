using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.DomainDriven;

namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 带子字段的对象类型字段
    /// </summary>
    internal abstract class ObjectField : DataField
    {
        //private List<IDataField> _childs;

        //public IEnumerable<IDataField> Childs
        //{
        //    get
        //    {
        //        return _childs == null ? Array.Empty<IDataField>().AsEnumerable() : _childs;
        //    }
        //}

        //public void AddChilds(IEnumerable<IDataField> childs)
        //{
        //    foreach (var child in childs)
        //    {
        //        AddChild(child);
        //    }
        //}

        //private void AddChild(IDataField field)
        //{
        //    if (_childs == null) _childs = new List<IDataField>();
        //    _childs.Add(field);
        //    field.ParentMemberField = this;
        //}


        public ObjectField(PropertyRepositoryAttribute attribute)
            : base(attribute, DbType.Object, Array.Empty<DbFieldType>())
        {
        }
    }
}