using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace CodeArt.Data
{
    public sealed class SqlData
    {
        private List<DataTable> _data;
        public SqlData(DataSet data)
        {
            _data = new List<DataTable>(data.Tables.Cast<DataTable>());
        }

        private SqlData(DataTable table)
        {
            _data = new List<DataTable> { table };
        }

        public bool IsEmpty()
        {
            return _data == null
                    || _data.Count == 0
                    || (_data.Count == 1 && _data[0].Rows.Count == 0);
        }

        /// <summary>
        /// 返回结果集中的第一行第一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetScalar<T>()
        {
            if (!IsEmpty()) return GetFirstRow().Field<T>(0);
            throw new DataException("查询结果为空，无法获取值");
        }

        /// <summary>
        /// 返回结果集中的第一行第一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetScalar<T>(T defaultValue)
        {
            if (!IsEmpty()) return GetFirstRow().Field<T>(0);
            return defaultValue;
        }

        /// <summary>
        /// 获取第一行的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetTopValue<T>(string name)
        {
            if (!IsEmpty()) return GetFirstRow().Field<T>(name);
            throw new DataException("查询结果为空，无法获取值");
        }

        /// <summary>
        /// 获取第一行的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetTopValue<T>(string name, T defaultValue)
        {
            if (IsEmpty()) return defaultValue;
            object value = GetFirstRow()[name];
            if (value == null || value == DBNull.Value) return defaultValue;
            return (T)value;
        }

        private DataRow _firstRow = null;
        /// <summary>
        /// 返回结果集中的第一行
        /// </summary>
        /// <returns></returns>
        private DataRow GetFirstRow()
        {
            if (_firstRow == null && !IsEmpty())
                _firstRow = _data[0].Rows[0];
            return _firstRow;
        }

        private SqlData _topRow = null;

        public SqlData GetTopRow()
        {
            if (_topRow == null && !IsEmpty())
                _topRow = CreateDataBy(GetFirstRow());
            return _topRow;
        }

        /// <summary>
        /// 遍历第一个数据集的每一行数据
        /// </summary>
        /// <param name="action"></param>
        public void ForEachTop(Action<SqlData> action)
        {
            ForEach(0, action);
        }

        /// <summary>
        /// 遍历指定数据集的每一行数据
        /// </summary>
        /// <param name="tableIndex">数据集的索引</param>
        /// <param name="action"></param>
        public void ForEach(int tableIndex, Action<SqlData> action)
        {
            DataTable dt = GetTable(tableIndex);
            if (dt == null) return;
            foreach (DataRow dr in dt.Rows)
            {
                SqlData data = CreateDataBy(dr);
                action(data);
            }
        }


        /// <summary>
        /// 遍历所有数据集
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action<SqlData> action)
        {
            for (var i = 0; i < _data.Count; i++)
            {
                DataTable dt = GetTable(i);
                SqlData data = CreateDataBy(dt);
                action(data);
            }
        }

        private SqlData CreateDataBy(DataRow dr)
        {
            return new SqlData(CreateTableBy(dr));
        }

        private DataTable CreateTableBy(DataRow dr)
        {
            DataTable dt = dr.Table.Clone();
            DataRow row = dt.NewRow();
            for (int i = 0; i < dt.Columns.Count; i++)
                row[i] = dr[i];
            dt.Rows.Add(row);
            return dt;
        }

        private SqlData CreateDataBy(DataTable table)
        {
            return new SqlData(table);
        }


        /// <summary>
        /// 返回结果集中的第一个表
        /// </summary>
        /// <returns></returns>
        public DataTable GetTable()
        {
            return GetTable(0);
        }

        /// <summary>
        /// 返回结果集中的第index一个表
        /// </summary>
        /// <returns></returns>
        public DataTable GetTable(int index)
        {
            if (!IsEmpty() && (index < _data.Count))
                return _data[index];
            return null;
        }


        public int GetCount(int tableIndex)
        {
            int count = 0;
            if (!IsEmpty())
            {
                var dt = GetTable(tableIndex);
                count = dt == null ? 0 : dt.Rows.Count;
            }
            return count;
        }

        public static SqlData CreateEmpty()
        {
            return new SqlData(new DataSet());
        }

    }
}
