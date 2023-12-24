using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.WPF.Controls.Playstation
{
    public interface IInput
    {
        string MemberName { get; set; }

        /// <summary>
        /// 向数据中写入输入项的值
        /// </summary>
        /// <param name="data"></param>
        void Write(DTObject data);

        /// <summary>
        /// 从数据<paramref name="data"/>中读取值
        /// </summary>
        /// <param name="data"></param>
        void Read(DTObject data);


        void Validate(ValidationResult result);
    }
}
