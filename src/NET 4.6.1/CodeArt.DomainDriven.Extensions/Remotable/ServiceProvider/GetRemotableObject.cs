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
    public class GetRemoteObject : RemoteServiceProvider
    {
        protected override DTObject InvokeImpl(DTObject arg)
        {
            var tip = GetTip(arg);
            var obj = FindObject(tip, arg);
            return DTObject.CreateReusable(tip.SchemaCode, obj);
        }

        private RemotableAttribute GetTip(DTObject arg)
        {
            var typeName = arg.GetValue<string>("typeName"); //这是远程类型的全称
            return RemotableAttribute.GetTip(typeName);
        }

        private object FindObject(RemotableAttribute tip, DTObject arg)
        {
            var idProperty = DomainProperty.GetProperty(tip.ObjectType, EntityObject.IdPropertyName);
            var id = DataUtil.ToValue(arg.GetValue("id"), idProperty.PropertyType);

            var repository = Repository.Create(tip.ObjectType);
            return repository.Find(id, QueryLevel.None);
        }

        public static readonly GetRemoteObject Instance = new GetRemoteObject();
    }
}