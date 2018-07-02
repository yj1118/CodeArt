using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.Concurrent;

namespace CodeArt.DomainDrivenTest.Detail
{
    [SafeAccess]
    [RemoteType(TypeName = "User")]
    public class CarUser : AggregateRootDefine
    {
        const string MyTypeName = "CarUser";

        const string MyMetadataCode = "{id:'int',name:'ascii,10',son:'CarUser',wife:'CarUser'}";

        public CarUser()
            : base(MyTypeName, MyMetadataCode)
        {
        }
    }
}