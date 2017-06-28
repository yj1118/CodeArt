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
    public class TypeGenerator
    {
        #region 构造函数

        /// <summary>
        /// 定义一个公开的构造函数
        /// </summary>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public MethodGenerator DefineConstructor(params Type[] parameterTypes)
        {
            var ctor = _typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, parameterTypes);
            MethodGenerator g = new MethodGenerator(ctor, ctor.GetILGenerator());
            return g;
        }

        public MethodGenerator DefineConstructor()
        {
            return DefineConstructor(Type.EmptyTypes);
        }

        #endregion

        #region 方法

        /// <summary>
        /// 定义一个公开的方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="returnType"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public MethodGenerator DefineMethod(string name,Type returnType,params Type[] parameterTypes)
        {
            return DefineMethod(name, MethodAttributes.Public, returnType, parameterTypes);
        }

        public MethodGenerator DefineMethod(string name, Type returnType)
        {
            return DefineMethod(name, returnType, Type.EmptyTypes);
        }

        public MethodGenerator DefineMethod(string name, MethodAttributes attributes, Type returnType, params Type[] parameterTypes)
        {
            var method = _typeBuilder.DefineMethod(name, attributes, returnType, parameterTypes);
            MethodGenerator g = new MethodGenerator(method, method.GetILGenerator());
            return g;
        }

        #endregion

        #region 属性

        private const MethodAttributes _propertyMethodAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        public PropertyDefine DefineProperty(string name, Type propertyType)
        {
            PropertyBuilder property = _typeBuilder.DefineProperty(name, PropertyAttributes.HasDefault,propertyType, null);
            var getAccessor = _typeBuilder.DefineMethod(string.Format("get_{0}", name), _propertyMethodAttr, propertyType, Type.EmptyTypes);
            var setAccessor = _typeBuilder.DefineMethod(string.Format("set_{0}", name), _propertyMethodAttr, null, new Type[] { propertyType });
            return new PropertyDefine(property, getAccessor, setAccessor);
        }

        /// <summary>
        /// 用字段包装属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="propertyType"></param>
        public void DefinePropertyWrapper(string name, Type propertyType)
        {
            PropertyDefine def = DefineProperty(name, propertyType);
            var field = DefineField(string.Format("_{0}", name), propertyType);
            InitGetProperty(def, field);
            InitSetProperty(def, field);
        }

        public void DefinePropertyReadOnlyWrapper(string name, Type propertyType)
        {
            PropertyDefine def = DefineProperty(name, propertyType);
            var field = DefineField(string.Format("_{0}", name), propertyType);
            InitGetProperty(def, field);
        }

        private void InitGetProperty(PropertyDefine def, FieldBuilder field)
        {
            //get
            var g = def.GetMethod;
            g.LoadParameter(0);/// 在一个属性实例中，参数0是实例obj
            g.LoadField(field);
            g.Return();
        }

        private void InitSetProperty(PropertyDefine def, FieldBuilder field)
        {
            //set
            var g = def.SetMethod;
            g.LoadParameter(0);
            g.Assign(field, () =>
            {
                g.LoadParameter(1);//在属性的设置方法中，第二个参数是value
            });
            g.Return();
        }

        #endregion

        #region 字段

        public FieldBuilder DefineField(string name, Type fieldType)
        {
            return _typeBuilder.DefineField(name, fieldType, FieldAttributes.Private);
        }


        #endregion

        private TypeBuilder _typeBuilder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="define"></param>
        /// <param name="assemblyFileName">类型所在的程序集路径，如果指定了该参数，那么在生成类型时，会在磁盘上保存程序集文件</param>
        public TypeGenerator(TypeDefine define,string assemblyFileName)
        {
            _typeBuilder = define.CreateTypeBuilder(assemblyFileName);
        }

        public TypeGenerator(TypeDefine define)
            : this(define, null)
        {
        }

        /// <summary>
        /// 生成类型
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Type Generate()
        {
            return _typeBuilder.CreateType();
        }
    }
}
