using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using CodeArt.Util;

namespace CodeArt.Runtime.IL
{
    /// <summary>
    /// 封装了常用类型的生成方法
    /// </summary>
    [DebuggerDisplay("AssemblyName={AssemblyName}, TypeName={TypeName}")]
    public class TypeDefine
    {
        #region 常量

        private const TypeAttributes _publicClass = TypeAttributes.Public | TypeAttributes.Class;

        #endregion

        #region 类型定义

        private string _assemblyName;
        public string AssemblyName
        {
            get { return _assemblyName; }
        }

        private string _typeName;
        public string TypeName
        {
            get { return _typeName; }
        }

        private TypeAttributes _typeAttributes;
        public TypeAttributes TypeAttributes
        {
            get { return _typeAttributes; }
        }

        private Type[] _interfaceImplementations;
        /// <summary>
        /// 实现的接口
        /// </summary>
        public Type[] InterfaceImplementations
        {
            get { return _interfaceImplementations; }
            set { _interfaceImplementations = value; }
        }

        private Type _baseType;
        /// <summary>
        /// 基类
        /// </summary>
        public Type BaseType
        {
            get { return _baseType; }
            set { _baseType = value; }
        }

        #endregion

        public TypeDefine(string assemblyName,string typeName, TypeAttributes typeAttributes)
        {
            _assemblyName = assemblyName;
            _typeName = typeName;
            _typeAttributes = typeAttributes;
        }

        public TypeDefine(string assemblyName, string typeName)
            : this(assemblyName, typeName, _publicClass)
        {
        }

        internal TypeBuilder CreateTypeBuilder(string fileName)
        {
            ModuleBuilder mb = CreateModule(fileName);
            TypeBuilder tb = mb.DefineType(this.TypeName, this.TypeAttributes);
            SetBaseType(tb);
            AddInterfaceImplementations(tb);
            return tb;
        }

        private ModuleBuilder CreateModule(string fileName)
        {
            AssemblyName assemblyName = new AssemblyName(this.AssemblyName);
            if (fileName == null)
            {
                AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                return assemblyBuilder.DefineDynamicModule(assemblyName.Name);
            }
            else
            {
                AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
                return assemblyBuilder.DefineDynamicModule(assemblyName.Name, fileName);
            }
        }

        /// <summary>
        /// 为类型添加需要实现的接口
        /// </summary>
        /// <param name="tb"></param>
        private void AddInterfaceImplementations(TypeBuilder tb)
        {
            if (this.InterfaceImplementations != null)
            {
                foreach (var itf in this.InterfaceImplementations)
                {
                    tb.AddInterfaceImplementation(itf);
                }
            }
        }

        /// <summary>
        /// 设置类型的基类
        /// </summary>
        /// <param name="tb"></param>
        private void SetBaseType(TypeBuilder tb)
        {
            if (this.BaseType != null)
            {
                tb.SetParent(this.BaseType);
            }
        }



    }
}
