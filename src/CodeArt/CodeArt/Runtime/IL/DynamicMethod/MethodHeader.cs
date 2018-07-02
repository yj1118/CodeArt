using System;
using System.Reflection;

namespace CodeArt.Runtime.IL
{
    /// <summary>
    /// 封装了方法的基本信息
    /// </summary>
    public class MethodHeader
    {
        /// <summary>
        /// 调用约定,例如:静态方法、虚方法等
        /// </summary>
        public CallingConventions Attributes { get; set; }

        /// <summary>
        /// 方法在哪个类型的类上声明的
        /// </summary>
        public Type DeclaringType { get; set; }

        private Type _returnType = typeof(void);
        /// <summary>
        /// 返回值类型
        /// </summary>
        public Type ReturnType
        {
            get { return _returnType; }
            set { _returnType = value ?? typeof(void); }
        }

        private Type[] _parameters = Type.EmptyTypes;
        /// <summary>
        /// 参数类型
        /// </summary>
        public Type[] ParameterTypes
        {
            get { return _parameters; }
            set { _parameters = value ?? Type.EmptyTypes; }
        }
    }
}
