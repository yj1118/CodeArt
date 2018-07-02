using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.Concurrent;

namespace CodeArt.DomainDrivenTest.Demo
{
    [SafeAccess]
    [RemoteType()]
    public class User : AggregateRootDefine
    {
        const string UserTypeName = "User";

        //const string UserMetadataCode = "{id:'int',name:'ascii,10',person:{name:'string,10',sex:'byte'},menu:{name:'string,10',parent:'menu',childs:[menu]}}";

        const string UserMetadataCode = "{id:'int',name:'ascii,10',son:'User',wife:'User'}";

        public User()
            : base(UserTypeName, UserMetadataCode)
        {
        }
    }
}