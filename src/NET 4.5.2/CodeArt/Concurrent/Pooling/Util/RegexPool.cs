using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeArt.Concurrent
{
    public class RegexPool
    {
        private string _pattern;
        private RegexOptions _options;
        private Pool<Regex> _pool;

        public RegexPool(string pattern, RegexOptions options)
        {
            _pattern = pattern;
            _options = options;

            _pool = new Pool<Regex>(() =>
            {
                return new Regex(_pattern, _options);
            }, (regex, phase) =>
            {
                return true;
            }, new PoolConfig()
            {
                MaxRemainTime = 120 //闲置时间120秒
            });
        }

        public IPoolItem<Regex> Borrow()
        {
            return _pool.Borrow();
        }

        public bool IsMatch(string input)
        {
            using (var temp = _pool.Borrow())
            {
                var regex = temp.Item;
                return regex.IsMatch(input);
            }
        }

    }
}