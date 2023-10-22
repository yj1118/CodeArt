using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;

using CodeArt.AppSetting;
using CodeArt.EasyMQ.RPC;
using CodeArt.Util;
using CodeArt.EasyMQ;

namespace CodeArt.DomainDriven
{
    [SafeAccess]
    internal class GetRemoteObject : IRPCHandler
    {
        private GetRemoteObject() { }

        public TransferData Process(string method, DTObject arg)
        {
            //先初始化会话身份
            InitIdentity(arg);

            var tip = GetTip(arg);
            var obj = FindObject(tip, arg);
            var schemaCode = GetSchemaCode(tip, arg);
            var info = DTObject.Create(schemaCode, obj);
            return new TransferData(AppSession.Language,info);
        }

        private void InitIdentity(DTObject arg)
        {
            var identity = arg.GetObject("identity");
            AppSession.Identity = identity;
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

        private string GetSchemaCode(RemotableAttribute tip, DTObject arg)
        {
            var metadataSchemaCode = arg.GetValue("schemaCode", string.Empty);
            if (string.IsNullOrEmpty(metadataSchemaCode)) return tip.SchemaCode;//外界没有传递对象代码，那么使用默认的代码
            if (string.IsNullOrEmpty(tip.SchemaCode)) return metadataSchemaCode;//没有设置默认代码，表示不限制外界方面的数据

            return _isSafe(tip.SchemaCode)(metadataSchemaCode) ? metadataSchemaCode : tip.SchemaCode;//外界的代码超出规定，那么仅用限定的代码
        }

        private static Func<string, Func<string, bool>> _isSafe = LazyIndexer.Init<string, Func<string, bool>>((schemaCode) =>
        {
            return LazyIndexer.Init<string, bool>((metadataCode) =>
            {
                var source = DTObject.Create(schemaCode);
                return source.ContainsSchemaCode(metadataCode);
            });
        });


        public static readonly GetRemoteObject Instance = new GetRemoteObject();
    }
}