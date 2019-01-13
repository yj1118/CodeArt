using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.DTO
{
    internal sealed class DTEValue : DTEntity
    {
        public override DTEntityType Type => DTEntityType.Value;

        private object _value;

        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                this.Changed();
            }
        }

        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
                this.Changed();
            }
        }


        public DTEValue(string name,object value)
        {
            this.Name = name;
            this.Value = value;
        }

        #region 数据

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public override DTEntity Clone()
        {
            return new DTEValue(this.Name, Clone(this.Value));
        }

        private object Clone(object value)
        {
            return ObjectSerializer.Clone(value);
        }

        public override void ClearData()
        {
            this.Value = null;
            this.Changed();
        }

        public override bool ContainsData()
        {
            return this.Value != null;
        }

        #endregion

        #region 实体控制

        public override IEnumerable<DTEntity> FindEntities(QueryExpression query)
        {
            if (query.IsSelfEntities) return GetSelfEntities();//查询自身
            if (this.Name.EqualsIgnoreCase(query.Segment)) return GetSelfEntities();
            return Array.Empty<DTEntity>();
        }

        public override void DeletEntity(DTEntity e)
        {
            throw new NotImplementedException("DTValue.DeletEntity");
        }

        public override void SetEntity(QueryExpression query, Func<string, DTEntity> createEntity)
        {
            throw new NotImplementedException("DTValue.SetEntity");
        }

        //public override void OrderEntities()
        //{
            
        //}

        #endregion


        public override void Changed()
        {
            if(this.Parent != null)
                this.Parent.Changed();
        }


        #region 代码

        public override string GetCode(bool sequential, bool outputKey)
        {
            StringBuilder code = new StringBuilder();
            if (outputKey && !string.IsNullOrEmpty(this.Name))
                code.AppendFormat("\"{0}\"", this.Name);
            if (code.Length > 0) code.Append(":");
            code.Append(GetValueCode(sequential));
            return code.ToString();
        }

        private string GetValueCode(bool sequential)
        {
            var dto = this.Value as DTObject;
            if (dto != null) return dto.GetCode(sequential, false);
            return JSON.GetCode(this.Value);
        }

        public override string GetSchemaCode(bool sequential, bool outputKey)
        {
            return this.Name;
        }

        #endregion

    }
}
