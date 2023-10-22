using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public static class DataExtensions
    {
        public static long GetInt64(this IDataReader reader, string name)
        {
            var index = reader.GetOrdinal(name);
            return reader.GetInt64(index);
        }

        public static int GetInt32(this IDataReader reader, string name)
        {
            var index = reader.GetOrdinal(name);
            return reader.GetInt32(index);
        }


        public static bool GetBoolean(this IDataReader reader, string name)
        {
            var index = reader.GetOrdinal(name);
            return reader.GetBoolean(index);
        }

        public static Guid GetGuid(this IDataReader reader, string name)
        {
            var index = reader.GetOrdinal(name);
            return reader.GetGuid(index);
        }

        public static DateTime GetDateTime(this IDataReader reader,string name)
        {
            var index = reader.GetOrdinal(name);
            return reader.GetDateTime(index);
        }

        public static string GetString(this IDataReader reader, string name)
        {
            var index = reader.GetOrdinal(name);
            return reader.GetString(index);
        }

        public static byte GetByte(this IDataReader reader, string name)
        {
            var index = reader.GetOrdinal(name);
            return reader.GetByte(index);
        }

        public static float GetFloat(this IDataReader reader, string name)
        {
            var index = reader.GetOrdinal(name);
            return reader.GetFloat(index);
        }

        public static double GetDouble(this IDataReader reader, string name)
        {
            var index = reader.GetOrdinal(name);
            return reader.GetDouble(index);
        }

        public static bool IsDBNull(this IDataReader reader, string name)
        {
            var index = reader.GetOrdinal(name);
            return reader.IsDBNull(index);
        }

        public static bool Exist(this IDataReader reader, string name)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                };
            }
            return false;
        }

    }
}
