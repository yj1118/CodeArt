using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using CodeArt.DTO;
using CodeArt.IO;

namespace CodeArt.CI.Setting
{
    /// <summary>
    /// 额外的配置项
    /// </summary>
    public class Extra
    {
        private DTObject _data;
        private string _fileName;

        public Extra(string fileName)
        {
            _fileName = fileName;
            _data = Load(fileName);
        }

        public string Project
        {
            get
            {
                return _data.GetValue("project", string.Empty);
            }
            set
            {
                _data.SetValue("project", value);
            }
        }

        /// <summary>
        /// 保存更新的配置
        /// </summary>
        public void Save()
        {
            Save(_data, _fileName);
        }

        public void Save(string fileName)
        {
            Save(_data, fileName);
        }


        #region 静态方法

        static Extra()
        {
            var fileName = Configuration.Current.Extra;
            Current = new Extra(fileName);
        }

        public static Extra Current
        {
            get;
            private set;
        }

        private static DTObject Load(string fileName)
        {
            if (!File.Exists(fileName)) return DTObject.Create();
            var code = File.ReadAllText(fileName);
            return DTObject.Create(code);
        }


        private static void Save(DTObject config,string fileName)
        {
            IOUtil.CreateFileDirectory(fileName);
            File.WriteAllText(fileName, config.GetCode(false, false));
        }

        public static void Update(Action<Extra> set)
        {
            set(Current);
            Current.Save();
        }

        #endregion

    }
}
