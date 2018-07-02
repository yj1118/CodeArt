using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using CodeArt.DomainDriven;


namespace MenuSubsystem
{
    public interface IMenuRepository : IRepository<Menu>
    {
        Menu FindBy(string markedCode, QueryLevel level);

        /// <summary>
        /// 获取所有父亲集合（包括父级、祖父级等）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<Menu> FindParents(long id);

        /// <summary>
        /// 获取子类集合(仅获取下一级的子类)
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        IEnumerable<Menu> FindChilds(long parentId, QueryLevel level);

    }
}
