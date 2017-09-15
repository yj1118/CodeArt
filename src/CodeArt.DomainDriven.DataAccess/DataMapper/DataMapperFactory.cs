using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return SqlContext.GetMapper(objectType) ?? DataMapper.Instance;
        });
    }
}