using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.ServiceModel
{
    public interface IServiceRecorder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">服务名称</param>
        /// <param name="input">输入参数</param>
        /// <param name="output">返回结果</param>
        void Write(string name, DTObject input, DTObject output);
    }
}
