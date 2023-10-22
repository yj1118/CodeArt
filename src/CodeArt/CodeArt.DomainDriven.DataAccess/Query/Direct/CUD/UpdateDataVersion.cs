using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 更新数据版本号
    /// </summary>
    internal class UpdateDataVersion : SingleTableOperation
    {
        private string _sql;

        private UpdateDataVersion(DataTable target,bool isEnabledMultiTenancy)
            : base(target)
        {
            _sql = GetSql(isEnabledMultiTenancy);
        }

        protected override string GetName()
        {
            return string.Format("UpdateDataVersion {0}", this.Target.Name);
        }

        protected override string Process(DynamicData param)
        {
            return _sql;
        }

        public static UpdateDataVersion Create(DataTable target,bool isEnabledMultiTenancy)
        {
            return _getInstance(target)(isEnabledMultiTenancy);
        }

        private static Func<DataTable, Func<bool, UpdateDataVersion>> _getInstance = LazyIndexer.Init<DataTable, Func<bool, UpdateDataVersion>>((target) =>
        {
            return LazyIndexer.Init<bool, UpdateDataVersion>((isEnabledMultiTenancy) =>
            {
                return new UpdateDataVersion(target, isEnabledMultiTenancy);
            });
        });


        private string GetSql(bool isEnabledMultiTenancy)
        {
            var table = this.Target;
            var sql = new SqlUpdateBuilder();
            sql.SetTable(table.Name);

            sql.Set(string.Format("{0}={0}+1", SqlStatement.Qualifier(GeneratedField.DataVersionName)));                

            foreach (var field in table.PrimaryKeys)
            {
                sql.Where(field.Name);
            }

            if (isEnabledMultiTenancy)
            {
                sql.Where(GeneratedField.TenantIdName);
            }

            return sql.GetCommandText();
        }
    }
}