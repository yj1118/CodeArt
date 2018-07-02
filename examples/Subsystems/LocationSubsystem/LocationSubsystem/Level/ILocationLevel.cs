using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CodeArt.DomainDriven;

namespace LocationSubsystem
{
    public interface ILocationLevel
    {
        /// <summary>
        /// 获取对应地理位置级别的地理对象，例如省，市，区等
        /// </summary>
        /// <param name="levelName"></param>
        /// <returns></returns>
        Location GetLevel(Location source, string levelName);
    }
}
