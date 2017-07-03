using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.Runtime;

namespace CodeArt.DomainDriven
{
    public abstract class AggregateRoot<TObject, TIdentity> : Repositoryable<TObject, TIdentity>, IAggregateRoot
        where TObject : AggregateRoot<TObject, TIdentity>
        where TIdentity : struct
    {
        public AggregateRoot(TIdentity id)
            : base(id)
        {
            InitRemotable();
            this.OnConstructed();
        }


        #region 内聚根可以具有远程能力

        public RemotableAttribute RemotableTip
        {
            get
            {
                return _remotableTip;
            }
        }

        public RemoteType RemoteType
        {
            get
            {
                return _remotableTip?.RemoteType;
            }
        }

        /// <summary>
        /// 初始化对象的远程能力
        /// </summary>
        private void InitRemotable()
        {
            if(this.RemotableTip != null)
            {
                //指示了对象具备远程能力
                this.Updated += NotifyUpdated;
                this.Deleted += NotifyDeleted;
            }
        }

        private void NotifyUpdated(object sender, RepositoryEventArgs e)
        {
            RemotePortal.NotifyUpdated(this.RemoteType, e.Target.GetIdentity());
        }

        private void NotifyDeleted(object sender, RepositoryEventArgs e)
        {
            RemotePortal.NotifyDeleted(this.RemoteType, e.Target.GetIdentity());
        }

        #endregion


        private static RemotableAttribute _remotableTip;

        static AggregateRoot()
        {
            var objectType = typeof(TObject);
            _remotableTip = RemotableAttribute.GetTip(objectType);
        }

    }
}