using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven.DataAccess
{
    public interface IDataMapper
    {
        IEnumerable<IDataField> GetObjectFields(Type objectType, bool isSnapshot);

        void FillInsertData(DomainObject obj, DynamicData data);

        void OnInsert(DomainObject obj);

        void FillUpdateData(DomainObject obj, DynamicData data);

        void OnUpdate(DomainObject obj);

        void OnDelete(DomainObject obj);


        /// <summary>
        /// 构建查询命令
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        string Build(QueryBuilder builder, DynamicData param);

    }
}
