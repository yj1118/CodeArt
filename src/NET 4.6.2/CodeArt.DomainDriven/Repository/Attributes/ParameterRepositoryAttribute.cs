using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class ParameterRepositoryAttribute : RepositoryAttribute
    {
        public string LoadMethod
        {
            get;
            private set;
        }

        /// <summary>
        /// 参数有可能是抽象的，这种情况下，我们有可能需要制定参数在仓储创建时使用的类型
        /// </summary>
        public Type ImplementType
        {
            get;
            private set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadMethod">请保证加载参数数据的方法是被构造对象类型的静态成员或者数据仓储的静态成员</param>
        public ParameterRepositoryAttribute(string loadMethod)
        {
            this.LoadMethod = loadMethod;
        }

        /// <summary>
        /// 参数有可能是抽象的，这种情况下，我们有可能需要制定参数在仓储创建时使用的类型
        /// </summary>
        /// <param name="implementType"></param>
        public ParameterRepositoryAttribute(Type implementType)
        {
            this.ImplementType = implementType;
        }

    }
}
