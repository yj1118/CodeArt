using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Util;

namespace CodeArt.Web.WebPages.Xaml.Script
{
    /// <summary>
    /// 基于数据输出的脚本视图
    /// </summary>
    public struct DataView : IScriptView
    {
        private DTObject _data;

        public DataView(DTObject data)
        {
            _data = data;
        }

        public string GetDataCode()
        {
            return _data.GetCode(false, false);
        }

        public string GetScriptCode()
        {
            throw new NotImplementedException("DataView.GetScriptCode");
        }

    }
}
