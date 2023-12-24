using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CodeArt.Web;
using CodeArt.DTO;
using CodeArt.IO;

namespace CodeArt.Search
{
    public static class Backup
    {
        public static string Execute(string address, string indexName, string folder)
        {
            Init(address, indexName, folder);

            var data = DTObject.Create();
            data.SetValue("indices", indexName);
            int number = GetNumber(indexName, folder);

            //?wait_for_completion=true  该参数表示等待
            var reustlt = WebUtil.SendPut(string.Format("{0}/_snapshot/{1}/backup_{2}?wait_for_completion=true", address, indexName, number), data.GetCode(false, false).GetBytes(), "application/json;charset=utf-8");
            SaveNumber(indexName, folder, number += 1);
            return reustlt.GetString();
        }

        private static void Init(string address, string indexName, string folder)
        {
            if (WarehouseExist(address, indexName)) return;
            var data = DTObject.Create();
            data.SetValue("type", "fs");
            data.SetValue("settings.location", Path.Combine(folder, indexName));
            data.SetValue("settings.compress", "true");
            WebUtil.SendPut(string.Format("{0}/_snapshot/{1}", address, indexName), data.GetCode(false, false).GetBytes(), "application/json;charset=utf-8");
        }

        private static bool WarehouseExist(string address, string indexName)
        {
            try
            {
                var url = string.Format("{0}/_snapshot/{1}", address, indexName);
                var detail = DTObject.Create(WebUtil.SendGet(url).GetString());
                return detail.Exist(indexName);
            }
            catch (Exception ex)
            {
                return false;
            }     
        }

        private static int GetNumber(string indexName, string folder)
        {
            string configFileName = Path.Combine(folder, string.Format("{0}.json", indexName));
            if (!File.Exists(configFileName))
            {
                return 1;
            }
            else
            {
                DTObject config = DTObject.Create(File.ReadAllText(configFileName));
                return config.GetValue<int>("number");
            }
        }

        private static void SaveNumber(string indexName, string folder, int number)
        {
            string configFileName = Path.Combine(folder, string.Format("{0}.json", indexName));
            DTObject config = File.Exists(configFileName) ? DTObject.Create(File.ReadAllText(configFileName)) : DTObject.Create();
            config["number"] = number;
            File.WriteAllText(configFileName, config.GetCode(false, false));
        }

    }
}
