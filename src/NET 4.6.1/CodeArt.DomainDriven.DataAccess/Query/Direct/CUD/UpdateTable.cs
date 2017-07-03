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
    /// 插入数据
    /// </summary>
    internal class UpdateTable : SingleTableOperation
    {
        private LazyIndexer<int, string> _cache;

        private UpdateTable(DataTable target)
            : base(target)
        {
            _cache = new LazyIndexer<int, string>();
        }

        protected override string GetName()
        {
            return string.Format("Update {0}", this.Target.Name);
        }

        protected override string Process(DynamicData param)
        {
            var fieldsCode = HashCoder<string>.GetCode(param.Keys); //data.Keys 是参与修改的字段名称
            string sql = _cache.GetOrCreate(fieldsCode, (t) =>
            {
                return GetUpdateSql(this.Target, param);
            });
            return sql;
        }

        public static UpdateTable Create(DataTable target)
        {
            return _getInstance(target);
        }

        private static Func<DataTable, UpdateTable> _getInstance = LazyIndexer.Init<DataTable, UpdateTable>((target) =>
        {
            return new UpdateTable(target);
        });


        private static string GetUpdateSql(DataTable table, DynamicData data)
        {
            var sql = new SqlUpdateBuilder();
            sql.SetTable(table.Name);

            foreach (var p in data)
            {
                var field = p.Key;
                if (field.EqualsIgnoreCase(EntityObject.IdPropertyName)) continue; //不修改id和rooid
                if (table.Type != DataTableType.AggregateRoot)
                {
                    if (field.EqualsIgnoreCase(table.Root.TableIdName)) continue; //不修改rooid
                }
                sql.AddField(field);
            }

            //if (!table.IsDerived)
            //{
            //    //更新数据版本号
            //    sql.Set(string.Format("[{0}]=[{0}]+1", GeneratedField.DataVersionName));
            //}

            foreach (var field in table.PrimaryKeys)
            {
                sql.Where(field.Name);
            }

            return sql.GetCommandText();
        }
    }
}