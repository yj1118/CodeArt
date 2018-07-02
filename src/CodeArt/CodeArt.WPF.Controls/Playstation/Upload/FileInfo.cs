using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

using CodeArt.WPF.UI;
using CodeArt.DTO;

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// 待上传文件的信息
    /// </summary>
    public class FileInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 路径
        /// </summary>
        public string Path
        {
            get;
            private set;
        }

        /// <summary>
        /// 文件的扩展名
        /// </summary>
        public string Extension
        {
            get;
            private set;
        }

        /// <summary>
        /// 文件的附加信息，这常用于上传的消息传递中
        /// </summary>
        public Dictionary<string, object> Attached
        {
            get;
            private set;
        }

        /// <summary>
        /// 文件在的存储信息，请在上传完文件后更新该值
        /// </summary>
        public DTObject Storage
        {
            get;
            set;
        }

        public FileInfo(string name, string path, string extension)
        {
            this.Name = name;
            this.Path = path;
            this.Extension = extension.ToLower();
            this.Attached = new Dictionary<string, object>();
        }
    }
}