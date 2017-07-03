using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.ServiceModel
{
    internal class DirectTable
    {
        private MultiDictionary<string, string> _data = new MultiDictionary<string, string>(false, StringComparer.OrdinalIgnoreCase);

        private Func<string, string[]> _getAddresses;

        public DirectTable()
        {
            _getAddresses = LazyIndexer.Init<string, string[]>((fullName) =>
            {
                return _data.GetValues(fullName).ToArray();
            });
        }

        public void Add(string serviceName, string address)
        {
            _data.Add(serviceName, address);
        }

        public string GetAddress(string serviceFullName)
        {
            var addresses = _getAddresses(serviceFullName);
            return Algorithm.Balance<string>(addresses); //如果有多个地址，那么均衡使用某一个地址
        }

        


    }
}
