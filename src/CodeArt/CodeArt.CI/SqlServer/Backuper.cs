using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.DTO;
using CodeArt.IO;

namespace CodeArt.CI.SqlServer
{
    public class Backuper
    {
        private string _directory;
        private string _database;

        public Backuper(string directory, string database)
        {
            _directory = directory;
            _database = database;
        }

        private DTObject LoadConfig()
        {
            var configFile = Path.Combine(_directory, "config.json");
           
            DTObject config = null;
            if (!File.Exists(configFile))
            {
                IOUtil.CreateFileDirectory(configFile);
                config = DTObject.Create();
            }
            else
            {
                var code = File.ReadAllText(configFile);
                config = DTObject.Create(code);
            }
            return config;
        }

        private void SaveConfig(DTObject config)
        {
            var configFile = Path.Combine(_directory, "config.json");
            File.WriteAllText(configFile, config.GetCode(false, false));
        }

        /// <summary>
        /// 执行备份
        /// </summary>
        public (string Log, int Code) Execute(string server, string user, string password)
        {
            DTObject config = LoadConfig();

            (string Log, int Code) result = (string.Empty,0);
            if (!config.Exist("origin"))
            {
                //没有数据库原点
                var file = string.Format(@"{0}\{1}.bak",_directory,_database);
                result= SqlCMD.FullBackup(_database, file, server, user, password);
                config["origin"] = true;
            }
            else
            {
                var index = config.GetValue<int>("inc", 1);
                var file = string.Format(@"{0}\{1}_Inc_{2}.bak", _directory, _database, index);
                result = SqlCMD.IncBackup(_database, file, server, user, password);
                config.SetValue("inc", index + 1);
            }

            SaveConfig(config);

            return result;
        }
    }
}
