using System;
using System.Diagnostics;

using CodeArt.Util;

namespace CodeArt.Diagnostics
{
    public static class TimeMonitor
    {
        public static OverseeData Oversee(ActionProcedure procedure)
        {
            OverseeData data = new OverseeData();
            Stopwatch stopwatch = new Stopwatch();
            foreach (var item in procedure.Items)
            {
                OverseeTime time = new OverseeTime(item.Name);
                try
                {
                    stopwatch.Start();
                    item.Action();
                    stopwatch.Stop();
                    time.Message = "success";
                }
                catch (Exception ex)
                {
                    time.Message = string.Format("error:{0}", ex.GetCompleteMessage());
                    throw ex;
                }
                finally
                {
                    time.Elapsed = stopwatch.Elapsed;
                    stopwatch.Reset();
                }
                data.AddTime(time);
            }
            return data;
        }

        public static OverseeData Oversee(params Action[] actions)
        {
            ActionProcedure procedure = new ActionProcedure();
            foreach (Action action in actions)
            {
                procedure.AddAction(action);
            }
            return Oversee(procedure);
        }
    }
}
