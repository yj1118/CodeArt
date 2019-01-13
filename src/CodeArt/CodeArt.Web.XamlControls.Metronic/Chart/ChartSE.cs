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
        public void SetPie(string seriesItemName, DTObjects seriesData)
        {
            SetPie(null,null,null, seriesItemName, seriesData);
        }

        public void SetPie(string titleText, string seriesItemName, DTObjects seriesData)
        {
            SetPie(titleText, null, null, seriesItemName, seriesData);
        }

        public void SetPie(string titleText, string titleSubtext, string seriesItemName, DTObjects seriesData)
        {
            SetPie(titleText, titleSubtext, null, seriesItemName, seriesData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="seriesItemName">每个饼状图区域的名称</param>
        /// <param name="seriesData">格式为:[{name,value}]</param>
        /// <param name="radius">饼状图的大小，值在1-100之间</param>
        /// <param name="centerX">饼状图的x坐标，值在1-100之间</param>
        /// <param name="centerY">饼状图的y坐标，值在1-100之间</param>
        public void SetPie(string titleText, string titleSubtext, string titleX,
                        string seriesItemName,
                        DTObjects seriesData,int radius = 65)
        {
            if (string.IsNullOrEmpty(titleX)) titleX = "center";

            var config = DTObject.Create();
            config.SetValue("title.text", titleText ?? string.Empty);
            config.SetValue("title.subtext", titleSubtext ?? string.Empty);
            config.SetValue("title.x", titleX ?? "center");

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

            var config = DTObject.Create();
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

        /// <summary>
        /// 分散图
        /// </summary>
        /// <param name="title"></param>
        /// <param name="seriesData">每一项需要有x,y,name</param>
        /// <param name="symbolSize">每个块的形状大小</param>
        /// <param name="nameFontSize">每个块上的名称的字体大小</param>
        public void SetScatter((string Text, string Subtext, string X) title, DTObjects seriesData,int symbolSize = 30,int nameFontSize=20)
        {
            if (string.IsNullOrEmpty(title.X)) title.X = "left";

            var config = DTObject.Create();
            config.SetValue("title.text", title.Text ?? string.Empty);
            config.SetValue("title.subtext", title.Subtext ?? string.Empty);
            config.SetValue("title.x", title.X);
            config.SetValue("series", seriesData);
            config.SetValue("symbolSize", symbolSize);
            config.SetValue("nameFontSize", nameFontSize);
            this.View.WriteCode(string.Format("{0}.proxy().setScatter({1});", this.Id, config.GetCode(false, false)));
        }


        public void Set(DTObject data)
        {
            this.View.WriteCode(string.Format("{0}.proxy().set({1});", this.Id, data.GetCode(false, false)));
        }

        public enum Formatter
        {
            None,//表示不格式
            hhmm,//表示时间的hh:mm的格式化
        }

    }
}
