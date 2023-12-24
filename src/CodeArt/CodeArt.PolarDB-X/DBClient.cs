using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace CodeArt.PolarDB_X
{
    public class DBClient : IDisposable
    {
        private MySqlConnection _con;

        public void Open()
        {
            _con.Open();
        }

        public void Close()
        {
            _con.Close();
        }

        public int ExecuteNonQuery(string sql)
        {
            var cmd = new MySqlCommand(sql, _con);
            return cmd.ExecuteNonQuery();
        }
        
        public DBClient(string ip, int port, string username, string password, string database)
        {
            //创建mysql链接
            var conStr = $"server={ip};port={port};user={username};password={password};database={database};";
            _con = new MySqlConnection(conStr);
        }

        public void Dispose()
        {
            Close();
        }
    }
}
