using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace SerialNumberSubsystem
{
    public interface ISNGeneratorRepository : IRepository<SNGenerator>
    {
        /// <summary>
        /// 根据唯一标示精确查找权限对象
        /// </summary>
        /// <param name="markedCode"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        SNGenerator FindByMarkedCode(string markedCode, QueryLevel level);
    }

}
