using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;
using System.Timers;

[assembly: ProApplicationStartedAttribute(typeof(CodeArt.DomainDriven.Extensions.ProApplicationStarted),"Initialized")]


namespace CodeArt.DomainDriven.Extensions
{
    internal class ProApplicationStarted
    {
        private static void Initialized()
        {
            DomainMessage.Continue();
            StartDeleteExpired();
        }

        #region 消息过期检测

        private static Timer _timer;

        public static void StartDeleteExpired()
        {
            _timer = new Timer(12*60 * 60 * 1000); //每隔12小时候执行一次
            _timer.Elapsed += OnElapsed;
            _timer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
            _timer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
        }

        private static void OnElapsed(object sender, ElapsedEventArgs e)
        {
            DomainMessage.DeleteExpired();
        }

        #endregion

    }
}
