using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.ServiceModel
{
    internal class EmptyRecorder: IServiceRecorder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">服务名称</param>
        /// <param name="input">输入参数</param>
        /// <param name="output">返回结果</param>
        public void Write(string name, DTObject input, DTObject output)
        {
            //不记录
        }

        public static readonly IServiceRecorder Instance = new EmptyRecorder();
    }
}
