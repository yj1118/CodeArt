using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 实体对象接口
    /// Entity的最基本职责是确保连续性，
    /// 以便其行为更清楚且可预测。
    /// 保持实体的简练是实现这一责任的关键。
    /// 不要将注意力集中在属性或行为上，应该摆脱这些细枝末节，抓住Entity对象定义的最基本特征，
    /// 尤其是那些用于标示、查找或匹配对象的特征。只添加那些对概念至关重要的行为和这些行为所必须的属性。
    /// 此外，应该将行为和属性转移到与核心实体关联的其他对象中
    /// 每个entity都会有一个所属的聚合根
    /// </summary>
    public interface IEntityObject : IDomainObject
    {
        /// <summary>
        /// 获取对象唯一标示
        /// 在领域驱动思想中，引用对象的标示可以是本地唯一也可以是全局唯一
        /// 在内聚根对象中一定要全局唯一
        /// </summary>
        /// <returns></returns>
        object GetIdentity();

    }
}
