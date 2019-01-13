using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.WebPages.Xaml.Script;

namespace CodeArt.Web.XamlControls.Metronic
{
    /// <summary>
    /// 
    /// </summary>
    public class DaterangeSE : ScriptElement
    {
        public DaterangeSE() { }

        #region 发射命令

        public void Set(System.DateTime start, System.DateTime end)
        {
            DTObject value = DTObject.Create();
            value.SetValue("start", start);
            value.SetValue("end", end);
            this.View.WriteCode(string.Format("{0}.proxy().set({1});",this.Id, value.GetCode(false, false)));
        }

        public void SetToday()
        {
            this.Set(System.DateTime.Now, System.DateTime.Now);
        }


        public void SetYesterday()
        {
            this.Set(System.DateTime.Now.AddDays(-1), System.DateTime.Now);
        }

        public void SetLast7Days()
        {
            this.Set(System.DateTime.Now.AddDays(-6), System.DateTime.Now);
        }

        public void SetLast30Days()
        {
            this.Set(System.DateTime.Now.AddDays(-29), System.DateTime.Now);
        }

        public void SetThisMonth()
        {
            var now = System.DateTime.Now;
            this.Set(new System.DateTime(now.Year, now.Month, 1), now);
        }

        public void SetLastMonth()
        {
            var now = System.DateTime.Now;
            var last = now.AddMonths(-1);
            this.Set(new System.DateTime(last.Year, last.Month, 1), new System.DateTime(now.Year, now.Month, 1).AddDays(-1));
        }

        

        #endregion


    }
}
