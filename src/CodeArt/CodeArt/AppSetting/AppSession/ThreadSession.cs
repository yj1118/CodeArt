using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CodeArt.AppSetting;


namespace CodeArt.AppSetting
{
    [AppSessionAccess]
    public class ThreadSession : IAppSession
    {
        private static ThreadLocal<ContentEntries> _local = new ThreadLocal<ContentEntries>(()=>
        {
            return new ContentEntries();
        });

        public bool Initialized
        {
            get
            {
                return _local.Value.Initialized;
            }
        }


        public ThreadSession()
        {
        }

        public void Initialize()
        {
            _local.Value.Initializ();
        }

        public void Dispose()
        {
            if(_local.IsValueCreated)
            {
                _local.Value.Clear();
            }
        }


        public object GetItem(string name)
        {
            return _local.Value.Get(name);
        }

        public void SetItem(string name, object value)
        {
            _local.Value.Set(name, value);
        }

        public bool ContainsItem(string name)
        {
            return _local.Value.Contains(name);
        }

        public static readonly ThreadSession Instance = new ThreadSession();

    }
}
