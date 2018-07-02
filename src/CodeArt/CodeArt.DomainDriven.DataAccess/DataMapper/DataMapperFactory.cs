using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    internal static class DataMapperFactory
    {
        public static IDataMapper Create(Type objectType)
        {
            return _getMapper(objectType);
        }

        private static Func<Type, IDataMapper> _getMapper = LazyIndexer.Init<Type, IDataMapper>((objectType) =>
        {
            return GetByRepository(objectType) ?? DataMapper.Instance;
        });

        /// <summary>
        /// 从仓储的定义中得到mapper
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        private static IDataMapper GetByRepository(Type objectType)
        {
            var repository = Repository.CreateWithNoCheckUp(objectType);
            if (repository == null) return null;
            var dataMapperType = DataMapperAttribute.GetDataMapperType(repository);
            if (dataMapperType == null) return null;
            return SafeAccessAttribute.CreateSingleton<IDataMapper>(dataMapperType);
        }

    }
}