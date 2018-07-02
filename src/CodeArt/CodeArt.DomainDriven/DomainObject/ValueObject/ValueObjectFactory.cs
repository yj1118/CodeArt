using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    [Obsolete]
    public abstract class ValueObjectFactory<TObject> : ValueObject
    {
        private static Dictionary<int, TObject> _instance = new Dictionary<int, TObject>();

        public static TObject Create(Func<int> getHashCode, Func<TObject> construct)
        {
            var id = getHashCode();
            TObject obj = default(TObject);
            if (_instance.TryGetValue(id, out obj)) return obj;
            lock(_instance)
            {
                if (_instance.TryGetValue(id, out obj)) return obj;
                else
                {
                    obj = construct();
                    _instance.Add(id, obj);
                }
            }
            return obj;
        }
    }
}