using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.WPF.Screen;
namespace CodeArt.WPF.Controls.Playstation
{
    public static class BootstrapUtil
    {
        /// <summary>
        /// 按照比例缩放大小
        /// </summary>
        /// <param name="proportion">原始比例</param>
        /// <param name="widthScale">所占整个屏幕宽度的比例，例如：0.1表示占屏幕宽度的10分之1</param>
        /// <returns></returns>
        public static (double Width, double Height) ZoomByWidth((double Width, double Height) proportion,double widthScale)
        {
            var logicAreaWidth = 0;
            Work.UIInvoke(()=>
            {
                logicAreaWidth = Work.Current.LogicArea.Width;
            });

            var actualWidth = logicAreaWidth * widthScale;
            if (actualWidth > proportion.Width) return proportion; //如果转换后的宽度比原始宽度大，那么还是使用原始宽度
            var height = actualWidth * proportion.Height / proportion.Width;
            return (actualWidth, height);
        }
    }
}
