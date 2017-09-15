using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;

using CodeArt.EasyMQ.RPC;
using CodeArt.Util;

namespace CodeArt.DomainDriven
{
    [SafeAccess]
    internal class GetRemoteObject : IRPCHandler
    {
        private GetRemoteObject() { }

        public DTObject Process(string method, DTObject args)
        {
            var tip = GetTip(args);
            var obj = FindObject(tip, args);
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