using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Data;


using CodeArt.Util;
using CodeArt.DomainDriven;


namespace CodeArt.DomainDriven.DataAccess
{
    [DebuggerDisplay("Name={Name} PropertyName = {PropertyName}")]
    internal abstract class DataField : IDataField
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        public bool IsAdditional
        {
            get;
            set;
        }

        public string PropertyName
        {
            get
            {
                return this.Tip.Property.Name;
            }
        }


        public abstract DataFieldType FieldType
        {
            get;
        }

        public abstract bool IsMultiple
        {
            get;
        }


        public DbType DbType
        {
            get;
            private set;
        }

        private IList<DbFieldType> _dbFieldTypes;

        public IEnumerable<DbFieldType> DbFieldTypes
        {
            get
            {
                return _dbFieldTypes;
            }
        }

        public void AddDbFieldType(DbFieldType fieldType)
        {
            if (!_dbFieldTypes.Contains(fieldType)) _dbFieldTypes.Add(fieldType);
        }

        public bool IsPrimaryKey
        {
            get
            {
                return this.DbFieldTypes.Where((t) => { return t == DbFieldType.PrimaryKey; }).Count() > 0;
            }
        }

        public bool IsClusteredIndex
        {
            get
            {
                return this.DbFieldTypes.Where((t) => { return t == DbFieldType.ClusteredIndex; }).Count() > 0;
            }
        }

        public bool IsNonclusteredIndex
        {
            get
            {
                return this.DbFieldTypes.Where((t) => { return t == DbFieldType.NonclusteredIndex; }).Count() > 0;
            }
        }

        public PropertyRepositoryAttribute Tip
        {
            get;
            private set;
        }

        public IDataField ParentMemberField
        {
            get;
            set;
        }

        public DataTable Table
        {
            get;
            set;
        }

        public string TableName
        {
            get;
            set;
        }

        public string MasterTableName
        {
            get;
            set;
        }

        public IList<DataTable> Derivatives
        {
            get;
            private set;
        }

        public DataField(PropertyRepositoryAttribute tip, DbType dbType, DbFieldType[] dbFieldTypes)
        {
            this.IsAdditional = false;//默认情况下不是附加字段
            this.Tip = tip;
            this.DbType = dbType;
            _dbFieldTypes = dbFieldTypes.ToList();
            this.Derivatives = new List<DataTable>();
        }
    }
}
