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
using System.Globalization;

using CodeArt.WPF.UI;

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BootstrapValue
    {
        internal BootstrapValue(string range,int rangeIndex,string valueExpression)
        {
            this.Range = range;
            this.RangeIndex = rangeIndex;
            this.ValueExpression = valueExpression;
        }

        public string Range
        {
            get;
            private set;
        }

        /// <summary>
        /// 区间序号，xs:0,sm:1,md:2,lg:3,xl:4
        /// </summary>
        internal int RangeIndex
        {
            get;
            private set;
        }

        public string ValueExpression
        {
            get;
            private set;
        }


        #region 计算结果

        private object _result;

        public object Result
        {
            get
            {
                if(_result == null)
                {
                    _result = GetResult();
                }
                return _result;
            }
        }

        protected abstract object GetResult();


        #endregion

        public static BootstrapValue Create(BootstrapValueType type, string expression)
        {
            var temp = expression.Split(':');
            var range = temp[0];
            var rangeIndex = 0;
            switch (range)
            {
                case "xs": rangeIndex = 0; break;
                case "sm": rangeIndex = 1; break;
                case "md": rangeIndex = 2; break;
                case "lg": rangeIndex = 3; break;
                case "xl": rangeIndex = 4; break;
            }
            var valueExpression = temp[1];

            switch(type)
            {
                case BootstrapValueType.Double: return new BootstrapDoubleValue(range, rangeIndex, valueExpression);
                case BootstrapValueType.Width: return new BootstrapWidthValue(range, rangeIndex, valueExpression);
                case BootstrapValueType.Height: return new BootstrapHeightValue(range, rangeIndex, valueExpression);
            }
            return null;
        }

    }
}