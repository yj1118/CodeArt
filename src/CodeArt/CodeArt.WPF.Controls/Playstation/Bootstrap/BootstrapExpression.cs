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

using CodeArt.Util;
using CodeArt.WPF.UI;
using CodeArt.WPF.Screen;

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// 如果系统的分辨率或者DPI改了，请重启软件后生效最新的判定
    /// </summary>
    public class BootstrapExpression
    {
        public BootstrapValueType Type
        {
            get;
            private set;
        }

        private List<BootstrapValue> _values;

        private BootstrapExpression(BootstrapValueType type, string expression)
        {
            this.Type = type;
            _values = Parse(expression);
        }

        #region 解析表达式

        private List<BootstrapValue> Parse(string expression)
        {
            string[] temp = expression.Split('|');
            List<BootstrapValue> values = new List<BootstrapValue>(temp.Length);
            foreach(var seg in temp)
            {
                values.Add(BootstrapValue.Create(this.Type, seg));
            }
            return values;
        }

        #endregion

        #region 求值

        private BootstrapValue _value;

        /// <summary>
        /// 根据当前系统的逻辑分辨率得到适当的值
        /// </summary>
        /// <returns></returns>
        public BootstrapValue GetValue()
        {
            if (_value != null) return _value;

            foreach(var range in _systemRanges)
            {
                var value = _values.FirstOrDefault((t) => t.Range == range);
                if (value != null)
                {
                    _value = value;
                    break;
                }
            }

            if(_value == null)
            {
                //如果系统需要的区间没有找到值，那么以最低的区间找出值
                _value = _values.OrderBy((v) => v.RangeIndex).FirstOrDefault();
            }

            return _value;
        }

        #endregion

        /// <summary>
        /// 系统应该判断的区间
        /// </summary>
        private static string[] _systemRanges;

        static BootstrapExpression()
        {
            _systemRanges = GetBootstrapRanges();
        }


        /// <summary>
        /// 获得当前系统需要遍历的区间
        /// </summary>
        /// <returns></returns>
        private static string[] GetBootstrapRanges()
        {
            List<string> ranges = new List<string>() { "xl","lg", "md", "sm", "xs" }; //从最大的找起，顺序不能颠倒
            var current = GetBootstrapRange();
            while(ranges.Count > 0)
            {
                if (current == ranges[0]) break;
                ranges.RemoveAt(0);
            }
            return ranges.ToArray();
        }
            
        /// <summary>
        /// 获得系统当前区间
        /// </summary>
        /// <returns></returns>
        private static string GetBootstrapRange()
        {
            var area = Work.Current.LogicArea;
            if (area.Width >= 3840) return "xl"; //4k以及4k以上
            if (area.Width > 1920) return "lg"; //2k-3k分辨率
            if (area.Width > 1366) return "md"; //1K分辨率
            if (area.Width > 800) return "sm"; //800-1366的分辨率
            return "xs";//800及以下 
        }

        private static Func<BootstrapValueType, Func<string, BootstrapExpression>> _getExpression = LazyIndexer.Init<BootstrapValueType, Func<string, BootstrapExpression>>((type) =>
        {
            return LazyIndexer.Init<string, BootstrapExpression>((exp) => { return new BootstrapExpression(type, exp); });
        });

        public static BootstrapExpression Create(BootstrapValueType type, string exp)
        {
            return _getExpression(type)(exp);
        }
    }
}