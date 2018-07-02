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
    /// 为指定类型上的属性存储序列化信息
    /// </summary>
    [DebuggerDisplay("{Name}")]
    internal class ArraySerializationInfo : MemberSerializationInfo
    {
        public ArraySerializationInfo(MemberInfo memberInfo
                                                , DTOMemberAttribute memberAttribute)
            : base(memberInfo, memberAttribute)
        {
        }

        public ArraySerializationInfo(Type classType)
            : base(classType)
        {
        }

        public override void GenerateSerializeIL(MethodGenerator g)
        {
            g.If(() =>
            {
                LoadMemberValue(g);//加载集合到堆栈上，检查是否为null
                return LogicOperator.IsNull;
            }, () =>
            {
                var elementType = this.TargetType.ResolveElementType();
                SerializationMethodHelper.WriteArray(g, this.DTOMemberName);
            }, () =>
            {
                //先写入空数组
                SerializationMethodHelper.WriteArray(g, this.DTOMemberName);

                //写入每个项
                if (this.MemberAttribute.IsBlob)
                {
                    SerializationMethodHelper.WriteBlob(g, this.DTOMemberName, () =>
                    {
                        LoadMemberValue(g);
                    });
                }
                else
                {
                    var elementType = this.TargetType.ResolveElementType();
                    LoadMemberValue(g);
                    g.ForEach(item =>
                    {
                        SerializationMethodHelper.WriteElement(g, this.DTOMemberName, elementType, () =>
                        {
                            g.Load(item);
                        });
                    });
                }
            });
        }

        public override void GenerateDeserializeIL(MethodGenerator g)
        {
            SetMember(g, () =>
            {
                var array = g.Declare(this.TargetType);

                if(this.MemberAttribute.IsBlob)
                {
                    g.Assign(array, () =>
                    {
                        SerializationMethodHelper.ReadBlob(g, this.DTOMemberName);//读取数量
                    });
                }
                else
                {
                    var length = g.Declare<int>();
                    g.Assign(length, () =>
                    {
                        SerializationMethodHelper.ReadLength(g, this.DTOMemberName);//读取数量
                    });

                    g.If(() =>
                    {
                        g.Load(length);
                        g.Load(0);
                        return LogicOperator.LessThan;
                    }, () =>
                    {
                        //数量小于1
                        //array = new array[];
                        var elementType = this.TargetType.ResolveElementType();
                        g.Assign(array, () =>
                        {
                            g.NewArray(elementType, () =>
                            {
                                g.Load(length);
                            });
                        });
                    }, () =>
                    {

                        var elementType = this.TargetType.ResolveElementType();

                        //int[] = new int[c];
                        g.Assign(array, () =>
                        {
                            g.NewArray(elementType, () =>
                            {
                                g.Load(length);
                            });
                        });

                        g.For(length, (index) =>
                        {
                            var item = g.Declare(elementType);

                            g.Assign(item, () =>
                            {
                                SerializationMethodHelper.ReadElement(g, this.DTOMemberName, elementType, index);
                            });

                            g.StoreElement(array, index, item);
                        });

                    });
                }

                g.Load(array);

            });
        }

        //public override string GetDTOSchemaCode()
        //{
        //    var elementType = this.TargetType.ResolveElementType();
        //    var elementTypeInfo = TypeSerializationInfo.GetTypeInfo(elementType);

        //    if (string.IsNullOrEmpty(this.DTOMemberName))
        //    {
        //        return "[" + elementTypeInfo.DTOSchemaCode + "]}";
        //    }
        //    else
        //    {
        //        return this.DTOMemberName + ":[" + elementTypeInfo.DTOSchemaCode + "]";
        //    }
        //}


    }
}
