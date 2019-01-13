using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CodeArt.DTO
{
    internal abstract class DTEntity
    {
        /// <summary>
        /// dto成员的名称
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// dto成员所在的父成员的引用
        /// </summary>
        public virtual DTEntity Parent { get; set; }

        public DTEntity()
        {
        }

        #region 数据

        public abstract DTEntity Clone();

        /// <summary>
        /// 清空dto成员所拥有的数据
        /// </summary>
        public abstract void ClearData();

        /// <summary>
        /// dto成员是否含有数据
        /// </summary>
        /// <returns></returns>
        public abstract bool ContainsData();

        #endregion

        #region 实体控制

        private IEnumerable<DTEntity> _selfEntities;
        public IEnumerable<DTEntity> GetSelfEntities()
        {
            if (_selfEntities == null)
            {
                var es = new List<DTEntity>();
                es.Add(this);
                _selfEntities = es;
            }
            return _selfEntities;
        }

        /// <summary>
        /// 根据查找表达式找出dto成员
        /// </summary>
        /// <param name="query">查询表达式</param>
        /// <returns></returns>
        public abstract IEnumerable<DTEntity> FindEntities(QueryExpression query);

        /// <summary>
        /// 删除成员
        /// </summary>
        /// <param name="e"></param>
        public abstract void DeletEntity(DTEntity e);

        /// <summary>
        /// 设置表达式对应的实体成员为新的对象
        /// </summary>
        /// <param name="query">查询表达式</param>
        /// <param name="createEntity"></param>
        public abstract void SetEntity(QueryExpression query, Func<string, DTEntity> createEntity);

        #endregion

        #region 代码

        /// <summary>
        /// 获取json格式的代码
        /// </summary>
        /// <param name="sequential">是否排序输出代码</param>
        /// <returns></returns>
        public abstract string GetCode(bool sequential, bool outputKey);

        /// <summary>
        /// 获取json格式的代码，不包含值，该代码作为架构代码
        /// </summary>
        /// <param name="sequential">是否排序输出代码</param>
        /// <returns></returns>
        public abstract string GetSchemaCode(bool sequential, bool outputKey);


        #endregion

        /// <summary>
        /// 当实体发生改变时触发
        /// </summary>
        public abstract void Changed();

        public abstract DTEntityType Type
        {
            get;
        }

    }

    internal enum DTEntityType
    {
        Value,
        Object,
        List
    }

}