using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.Web.WebPages.Xaml.Script;


namespace CodeArt.Web.XamlControls.Metronic
{
    public class ChartSE : ScriptElement
    {
        public ChartSE() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seriesItemName">每个饼状图区域的名称</param>
        /// <param name="seriesData">格式为:[{name,value}]</param>
        public void SetPie(string seriesItemName,
                        DTObjects seriesData)
        {
            SetPie(default((string, string, string)), seriesItemName, seriesData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="seriesItemName">每个饼状图区域的名称</param>
        /// <param name="seriesData">格式为:[{name,value}]</param>
        public void SetPie((string Text,string Subtext,string X) title,
                        string seriesItemName,
                        DTObjects seriesData)
        {
            if (string.IsNullOrEmpty(title.X)) title.X = "center";

            var config = DTObject.CreateReusable();
            config.SetValue("title.text", title.Text ?? string.Empty);
            config.SetValue("title.subtext", title.Subtext ?? string.Empty);
            config.SetValue("title.x", title.X);

            config.SetValue("series.name", seriesItemName);
            config.SetValue("series.data", seriesData);

            this.View.WriteCode(string.Format("{0}.proxy().setPie({1});", this.Id, config.GetCode(false, false)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xAxisData">x轴的数据</param>
        /// <param name="seriesData">格式为:{name,data:[value0,value1,value2]}</param>
        public void SetLine(IEnumerable<object> xAxisData,DTObjects seriesData, Formatter formatter)
        {
            SetLine((null, null, null), xAxisData, seriesData, formatter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="xAxisData">x轴的数据</param>
        /// <param name="seriesData">格式为:{name,data:[value0,value1,value2]}</param>
        public void SetLine((string Text, string Subtext, string X) title,
                        IEnumerable<object> xAxisData,
                        DTObjects seriesData, Formatter formatter)
        {
            if (string.IsNullOrEmpty(title.X)) title.X = "left";

            var config = DTObject.CreateReusable();
            config.SetValue("title.text", title.Text ?? string.Empty);
            config.SetValue("title.subtext", title.Subtext ?? string.Empty);
            config.SetValue("title.x", title.X);

            config.SetValue("xAxis.data", xAxisData);


            config.SetValue("series", seriesData);

            if(formatter != Formatter.None)
            {
                config.SetValue("formatter", formatter.ToString());
            }

            this.View.WriteCode(string.Format("{0}.proxy().setLine({1});", this.Id, config.GetCode(false, false)));
        }

        public enum Formatter
        {
            None,//表示不格式
            hhmm,//表示时间的hh:mm的格式化
        }

    }
}
