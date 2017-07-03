using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.ServiceModel;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;

using CodeArt.Runtime;
using CodeArt.Util;

namespace CodeArt.DomainDriven.Extensions
{
    [SafeAccess]
    public class RemoteObjectDeleted : RemoteServiceProvider
    {
        protected override DTObject InvokeImpl(DTObject arg)
        {
            AggregateRootDefine define = null;
            object id = null;
            ParseDefineAndId(arg, out define, out id);

            RemotePortal.DeleteObject(define, id);
            return DTObject.Empty;
        }


        public static readonly RemoteObjectDeleted Instance = new RemoteObjectDeleted();
    }
}