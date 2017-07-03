using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Collections.Concurrent;

using System.Threading;
using CodeArt.IO;


namespace CodeArt.Concurrent.Pattern.Eat
{
    internal class Rubbish : IDisposable
    {
        private ConcurrentQueue<object> _rubbishs;
        private AutoResetPipeline _destroyer;

        public Rubbish()
        {
            _rubbishs = new ConcurrentQueue<object>();
            _destroyer = new AutoResetPipeline(_DisposeRubbish);
        }

        private void _DisposeRubbish()
        {
            object rubbish;
            if (_rubbishs.TryDequeue(out rubbish))
            {
                DisposeItem(rubbish);
            }
        }

        /// <summary>
        /// 添加一个垃圾，稍后会被销毁
        /// </summary>
        /// <param name="rubbish"></param>
        public void Add(object rubbish)
        {
            _rubbishs.Enqueue(rubbish);
            _destroyer.AllowOne();
        }

        public void Dispose()
        {
            object rubbish;
            while (_rubbishs.TryDequeue(out rubbish))
            {
                DisposeItem(rubbish);
            }
        }


        #region 静态成员

        /// <summary>
        /// 销毁项
        /// </summary>
        /// <param name="item"></param>
        public static void DisposeItem(object item)
        {
            var disposable = item as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }


        #endregion

    }
}
