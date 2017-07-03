using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.Concurrent;
using CodeArt.DTO;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 用于动态领域对象的仓储
    /// </summary>
    public interface IDynamicRepository : IRepository
    {
        /// <summary>
        /// 从仓储中根据编号加载动态根对象
        /// </summary>
        /// <param name="rootType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        DynamicRoot Find(AggregateRootDefine define, object id, QueryLevel level);

        /// <summary>
        /// 向仓储中添加动态根对象
        /// </summary>
        /// <param name="define"></param>
        /// <param name="obj"></param>
        void Add(AggregateRootDefine define, DynamicRoot obj);

        /// <summary>
        /// 修改仓储中的根对象
        /// </summary>
        /// <param name="define"></param>
        /// <param name="obj"></param>
        void Update(AggregateRootDefine define, DynamicRoot obj);

        /// <summary>
        /// 移除仓储中的根对象
        /// </summary>
        /// <param name="define"></param>
        /// <param name="obj"></param>
        void Delete(AggregateRootDefine define, DynamicRoot obj);
    }
}
