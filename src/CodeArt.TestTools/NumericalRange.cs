using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.TestTools
{
    /// <summary>
    /// 数值范围
    /// </summary>
    public class NumericalRangeInt
    {
        private int _maxValue;
        private int _minValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="centralValue"></param>
        /// <param name="floatingRatio"></param>
        public NumericalRangeInt(int centralValue, float floatingRatio)
        {
            _minValue = (int)(centralValue * (1 - floatingRatio));
            _maxValue = (int)(centralValue * (1 + floatingRatio));
        }

        public bool Contains(int value)
        {
            return value >= _minValue && value <= _maxValue;
        }
    }
}
