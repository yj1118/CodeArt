using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    public abstract class Repositoryable<TObject, TIdentity> : EntityObject<TObject, TIdentity>
        where TObject : Repositoryable<TObject, TIdentity>
        where TIdentity : struct
    {
        public Repositoryable(TIdentity id)
            : base(id)
        {
            this.OnConstructed();
        }

        private string _uniqueKey;

        public string UniqueKey
        {
            get
            {
                if (_uniqueKey == null)
                {
                    _uniqueKey = UniqueKeyCalculator.GetUniqueKey(this);
                }
                return _uniqueKey;
            }
        }
    }
}