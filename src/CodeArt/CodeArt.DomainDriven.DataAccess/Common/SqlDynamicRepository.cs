using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;

using CodeArt.AppSetting;
using CodeArt.Util;
using CodeArt.DTO;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven.DataAccess
{
    [SafeAccess]
    public class SqlDynamicRepository : IDynamicRepository
    {
        private SqlDynamicRepository() { }

        public DynamicRoot Find(AggregateRootDefine define, object id, QueryLevel level)
        {
            var metadataType = define.MetadataType;
            var model = DataModel.Create(metadataType);
            return model.QuerySingle<DynamicRoot>(id, level);
        }

        /// <summary>
        /// 向仓储中添加动态根对象
        /// </summary>
        /// <param name="define"></param>
        /// <param name="obj"></param>
        public void Add(AggregateRootDefine define, DynamicRoot obj)
        {
            var metadataType = define.MetadataType;
            var model = DataModel.Create(metadataType);
            model.Insert(obj);
        }

        /// <summary>
        /// 修改仓储中的根对象
        /// </summary>
        /// <param name="define"></param>
        /// <param name="obj"></param>
        public void Update(AggregateRootDefine define, DynamicRoot obj)
        {
            var metadataType = define.MetadataType;
            var model = DataModel.Create(metadataType);
            model.Update(obj);
        }


        /// <summary>
        /// 移除仓储中的根对象
        /// </summary>
        /// <param name="define"></param>
        /// <param name="obj"></param>
        public void Delete(AggregateRootDefine define, DynamicRoot obj)
        {
            var metadataType = define.MetadataType;
            var model = DataModel.Create(metadataType);
            model.Delete(obj);
        }


        public IAggregateRoot Find(object id, QueryLevel level)
        {
            throw new NotImplementedException("SqlDynamicRepository.Find(object id, QueryLevel level)");
        }

        public void Add(IAggregateRoot obj)
        {
            throw new NotImplementedException("SqlDynamicRepository.Add(IAggregateRoot obj)");
        }

        public void Update(IAggregateRoot obj)
        {
            throw new NotImplementedException("SqlDynamicRepository.Update(IAggregateRoot obj)");
        }

        public void Delete(IAggregateRoot obj)
        {
            throw new NotImplementedException("SqlDynamicRepository.Delete(IAggregateRoot obj)");
        }

        public static readonly SqlDynamicRepository Instance = new SqlDynamicRepository();
    }
}
