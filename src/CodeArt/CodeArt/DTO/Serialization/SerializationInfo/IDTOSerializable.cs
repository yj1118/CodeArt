using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DTO
{
    /// <summary>
    /// 对象可以继承该接口，自定义序列化方法
    /// </summary>
    public interface IDTOSerializable
    {
        /// <summary>
        /// 将自身的内容序列化到<paramref name="owner"/>中
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        void Serialize(DTObject owner, string name);

        /// <summary>
        /// 获得对象的dto形式
        /// </summary>
        /// <returns></returns>
        DTObject GetData();
    }
}
