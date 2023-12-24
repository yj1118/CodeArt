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
using System.Threading;
using System.Collections.Concurrent;

using CodeArt.WPF.UI;
using CodeArt.Util;
using CodeArt.Runtime;
using System.Diagnostics;
using CodeArt.Concurrent.Sync;

namespace CodeArt.WPF.Controls.Playstation
{
    public class WaitingToken
    {
        public Guid Id
        {
            get;
            private set;
        }

        /// <summary>
        /// 等待是否超时
        /// </summary>
        public bool IsTimedOut
        {
            get;
            internal set;
        }

        public bool IsCompleted
        {
            get;
            internal set;
        }

        public Action Complete
        {
            get;
            private set;
        }

        public WaitingToken(Action complete)
        {
            this.Id = Guid.NewGuid();
            this.IsTimedOut = false;
            this.IsCompleted = false;
            this.Complete = ()=>
            {
                this.IsCompleted = true;
                complete();
            };
        }
    }
}