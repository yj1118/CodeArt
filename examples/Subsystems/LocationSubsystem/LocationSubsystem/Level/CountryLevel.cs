using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CodeArt.DomainDriven;

namespace LocationSubsystem
{
    public sealed class CountryLevel : ILocationLevel
    {
        private CountryLevel() { }

        public Location GetLevel(Location source, string levelName)
        {
            var path = source.Path;
            switch(levelName)
            {
                case "province": return path.GetItem(0);
                case "city": return path.GetItem(1);
                case "region": return path.GetItem(2);
                default:
                    {
                        throw new DomainDrivenException(typeof(CountryLevel).FullName + "不支持地理位置级别" + levelName + "的判定");
                    }
            }
        }

        /// <summary>
        /// 获取指定地理位置对应的省
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Location GetProvince(Location source)
        {
            return GetLevel(source, "province");
        }

        /// <summary>
        /// 获取指定地理位置对应的城市
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Location GetCity(Location source)
        {
            return GetLevel(source, "city");
        }

        /// <summary>
        /// 获取指定地理位置对应的区
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Location GetRegion(Location source)
        {
            return GetLevel(source, "region");
        }


        public static CountryLevel Instance = new CountryLevel();

    }
}
