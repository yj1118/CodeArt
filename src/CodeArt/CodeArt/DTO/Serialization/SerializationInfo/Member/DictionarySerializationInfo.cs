using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DTO
{
    /// <summary>
    /// 当前版本不支持DTO的键值对格式
    /// </summary>
    //[DebuggerDisplay("{Name}")]
    //internal class DictionarySerializationInfo : MemberSerializationInfo
    //{
    //    public DictionarySerializationInfo(MemberInfo memberInfo, DTOMemberAttribute memberAttribute)
    //        : base(memberInfo, memberAttribute)
    //    {
    //    }

    //    public DictionarySerializationInfo(Type classType)
    //        : base(classType)
    //    {
    //    }

    //    public override void GenerateSerializeIL(Runtime.CIL.MethodGenerator g)
    //    {
    //        g.If(() =>
    //        {
    //            LoadMemberValue(g);//加载集合到堆栈上，检查是否为null
    //            return LogicOperator.IsNull;
    //        }, () =>
    //        {
    //            //if(dic==null) writer.write(-1);
    //            SerializationMethodHelper.Write(g, typeof(int), () =>
    //            {
    //                g.Load(-1);
    //            });
    //        }, () =>
    //        {
    //            //写入长度
    //            SerializationMethodHelper.Write(g, typeof(int), () =>
    //            {
    //                LoadMemberValue(g);
    //                g.LoadMember("Count");
    //            });

    //            Type[] types = this.TargetType.ResolveGenericArguments();
    //            var keyType = types[0];
    //            var valueType = types[1];

    //            //写入每个项
    //            LoadMemberValue(g);
    //            g.ForEach(item =>
    //            {
    //                //write(key);
    //                SerializationMethodHelper.Write(g, keyType, () =>
    //                {
    //                    g.LoadVariable(item, LoadOptions.ValueAsAddress);
    //                    g.LoadMember("Key");
    //                });

    //                //write(value);
    //                SerializationMethodHelper.Write(g, valueType, () =>
    //                {
    //                    g.LoadVariable(item, LoadOptions.ValueAsAddress);
    //                    g.LoadMember("Value");
    //                });
    //            });
    //        });
    //    }

    //    public override void GenerateDeserializeIL(MethodGenerator g)
    //    {
    //        SetMember(g, () =>
    //        {
    //            var count = g.Declare<int>();
    //            g.Assign(count, () =>
    //            {
    //                SerializationMethodHelper.Read(g, typeof(int));//读取数量
    //            });

    //            var dictionary = g.Declare(this.TargetType);

    //            g.If(() =>
    //            {
    //                g.Load(count);
    //                g.Load(0);
    //                return LogicOperator.LessThan;
    //            }, () =>
    //            {
    //                //数量小于1
    //                //dictionary = null;
    //                g.Assign(dictionary, () =>
    //                {
    //                    g.LoadNull();
    //                });
    //            }, () =>
    //            {
    //                //dictionary = new Dictionary<Key,Value>();

    //                g.Assign(dictionary, () =>
    //                {
    //                    g.NewObject(this.TargetType);
    //                });

    //                Type[] types = this.TargetType.ResolveGenericArguments();
    //                var keyType = types[0];
    //                var valueType = types[1];

    //                g.For(count, (index) =>
    //                {
    //                    var key = g.Declare(keyType);
    //                    g.Assign(key, () =>
    //                    {
    //                        SerializationMethodHelper.Read(g, keyType);
    //                    });

    //                    var value = g.Declare(valueType);
    //                    g.Assign(value, () =>
    //                    {
    //                        SerializationMethodHelper.Read(g, valueType);
    //                    });
    //                    g.Load(dictionary);
    //                    g.Load(key);
    //                    g.Load(value);
    //                    g.Call(this.TargetType.ResolveMethod("Add", keyType, valueType));
    //                });
    //            });

    //            g.Load(dictionary);

    //        });
    //    }



    //}
}
