using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Data;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.DomainDriven;


namespace CodeArt.DomainDriven.DataAccess
{
    public interface IDataField
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 字段类型
        /// </summary>
        DataFieldType FieldType
        {
            get;
        }

        /// <summary>
        /// 字段对应的数据库类型
        /// </summary>
        DbType DbType
        {
            get;
        }

        /// <summary>
        /// 是否为主键
        /// </summary>
        bool IsPrimaryKey
        {
            get;
        }

        /// <summary>
        /// 是否为聚集索引
        /// </summary>
        bool IsClusteredIndex
        {
            get;
        }

        /// <summary>
        /// 是否为非聚集索引
        /// </summary>
        bool IsNonclusteredIndex
        {
            get;
        }

        /// <summary>
        /// 定义的仓储特性
        /// </summary>
        PropertyRepositoryAttribute Tip
        {
            get;
        }

        /// <summary>
        /// 所属父成员字段，例如book.category.cover
        /// 对于字段cover(BookCover表)的MemberField字段就是cover，BookCover表的ParentMemberField就是category
        /// </summary>
        IDataField ParentMemberField
        {
            get;
            set;
        }

        /// <summary>
        /// 字段所在的表
        /// </summary>
        DataTable Table
        {
            get;
            set;
        }

        /// <summary>
        /// 由于memberField会先建立，然后创建对应的表，所以再这个期间，
        /// memberField的Table为null,因此我们会记录TableName,以便别的对象使用
        /// </summary>
        string TableName
        {
            get;
            set;
        }


        string MasterTableName
        {
            get;
            set;
        }


        /// <summary>
        /// 表示该字段指示的是否为多行数据（集合）
        /// </summary>
        bool IsMultiple
        {
            get;
        }

        ///// <summary>
        ///// 由该字段衍生出来的表
        ///// </summary>
        //IList<DataTable> Derivatives
        //{
        //    get;
        //}

    }


    public enum DataFieldType
    {
        Value,
        ValueObject,
        AggregateRoot,
        EntityObject,
        EntityObjectPro,
        ValueList,
        ValueObjectList,
        EntityObjectList,
        EntityObjectProList,
        AggregateRootList,
        /// <summary>
        /// 由orm生成的键
        /// </summary>
        GeneratedField
    }

    internal enum DbFieldType
    {
        /// <summary>
        /// 主键
        /// </summary>
        PrimaryKey,
        /// <summary>
        /// 聚集索引
        /// </summary>
        ClusteredIndex,
        /// <summary>
        /// 非聚集索引
        /// </summary>
        NonclusteredIndex,
        /// <summary>
        /// 普通的键
        /// </summary>
        Common
    }

}
