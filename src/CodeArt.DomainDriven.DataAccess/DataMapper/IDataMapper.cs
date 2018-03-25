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

        //void FillInsertData(DomainObject obj, DynamicData data, DataTable table);

        void OnPreInsert(DomainObject obj, DataTable table);

        void OnInserted(DomainObject obj, DataTable table);

        //void FillUpdateData(DomainObject obj, DynamicData data, DataTable table);

        void OnPreUpdate(DomainObject obj, DataTable table);

        void OnUpdated(DomainObject obj, DataTable table);

        void OnPreDelete(DomainObject obj, DataTable table);

        void OnDeleted(DomainObject obj, DataTable table);


        /// <summary>
        /// 构建查询命令
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        string Build(QueryBuilder builder, DynamicData param, DataTable table);

    }
}
