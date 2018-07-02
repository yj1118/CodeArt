using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.WPF.Controls
{
    public class HistogramItem : ObservableObject
    {
        private double _value;

        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                this.MarkPropertyChanged("Value");
            }
        }

        private string _text;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                this.MarkPropertyChanged("Text");
            }
        }


        private string _valueText;

        public string ValueText
        {
            get
            {
                return _valueText;
            }
            set
            {
                _valueText = value;
                this.MarkPropertyChanged("ValueText");
            }
        }


        private double _percentageValue;

        /// <summary>
        /// value的百分比计算结果，范围0-1
        /// </summary>
        public double PercentageValue
        {
            get
            {
                return _percentageValue;
            }
            internal set
            {
                _percentageValue = value;
                this.MarkPropertyChanged("PercentageValue");
            }
        }

        /// <summary>
        /// 状态对象，每个柱状图可以设置一个额外的状态对象来处理定制化的需要
        /// </summary>
        public object Status
        {
            get;
            set;
        }



        public HistogramItem()
        {
            this.Value = 0;
            this.ValueText = string.Empty;
            this.Text = string.Empty;
        }
    }
}
