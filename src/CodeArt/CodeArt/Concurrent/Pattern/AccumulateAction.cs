using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Threading;

using CodeArt.DTO;
using CodeArt.Concurrent.Pattern;

namespace CodeArt.Concurrent.Pattern
{
    /// <summary>
    /// 积累行为，只有当数据达到某个数量时，才会触发行为
    /// </summary>
    public class AccumulateAction<T>
    {
        public int Count
        {
            get;
            private set;
        }

        private List<T> _datas;
        private Action<IEnumerable<T>> _action;

        public AccumulateAction(int count, Action<IEnumerable<T>> action)
        {
            this.Count = count;
            _datas = new List<T>();
            _action = action;
        }

        public void AddData(T data)
        {
            lock(_datas)
            {
                _datas.Add(data);
                if (_datas.Count == this.Count)
                {
                    _action(_datas);
                    _datas.Clear();
                }  
            }
        }


    }
}