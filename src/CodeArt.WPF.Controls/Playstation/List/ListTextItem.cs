using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.WPF.Controls.Playstation
{
    public class ListTextItem : TextItem
    {
        
        private string _imageSrc;

        public string ImageSrc
        {
            get
            {
                return _imageSrc;
            }
            set
            {
                _imageSrc = value;
                this.MarkPropertyChanged("ImageSrc");
            }
        }


        public ListTextItem(object value)
            : base(value)
        {
        }
    }
}
