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
    public enum UploadStatus : byte
    {
        /// <summary>
        /// 正在上传
        /// </summary>
        Uploading,

        /// <summary>
        /// 上传已完成
        /// </summary>
        Completed
    }
}