using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DTO
{
    /// <summary>
    /// 序列化类型的动态方法生成器（自动序列化）
    /// </summary>
    internal static class DTOSerializeMethodGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        public static SerializeMethod GenerateMethod(TypeSerializationInfo typeInfo)
        {
            DynamicMethod method = new DynamicMethod(string.Format("DTOSerialize_{0}", Guid.NewGuid().ToString("n"))
                                                    , null
                                                    , new Type[] { typeof(object),typeof(IDTOWriter) }
                                                    , true);

            MethodGenerator g = new MethodGenerator(method);

            DeclareInstance(g, typeInfo);
            WriteMembers(g, typeInfo);

            g.Return();

            return (SerializeMethod)method.CreateDelegate(typeof(SerializeMethod));
        }


        /// <summary>
        /// 声明强类型的instance变量留待后续代码使用，避免频繁类型转换
        /// </summary>
        private static void DeclareInstance(MethodGenerator g, TypeSerializationInfo typeInfo)
        {
            //TypeClassName instance = (TypeClassName)instance;
            var instance = g.Declare(typeInfo.ClassType, SerializationArgs.InstanceName);
            g.Assign(instance, () =>
            {
                g.LoadParameter(SerializationArgs.InstanceIndex);
                g.UnboxAny(typeInfo.ClassType);
            });
        }

        private static void WriteMembers(MethodGenerator g, TypeSerializationInfo typeInfo)
        {
            if (typeInfo.ClassAttribute.Mode == DTOSerializableMode.General)
            {
                foreach (var member in typeInfo.MemberInfos)
                {
                    if(member.CanRead)
                    {
                        g.BeginScope();
                        member.GenerateSerializeIL(g);
                        g.EndScope();
                    }
                }
            }
            else
            {
                //在函数模式,只有标记了ReturnValue的成员才会被写入到dto中
                foreach (var member in typeInfo.MemberInfos)
                {
                    if(member.MemberAttribute.Type == DTOMemberType.ReturnValue && member.CanRead)
                    {
                        g.BeginScope();
                        member.GenerateSerializeIL(g);
                        g.EndScope();
                    }
                }
            }
        }


    }
}
