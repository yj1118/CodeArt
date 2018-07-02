using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionAnswerSubsystem
{
    public enum QuestionType : byte
    {
        /// <summary>
        /// 单选题
        /// </summary>
        SingleChoice =1,
        /// <summary>
        /// 多选题
        /// </summary>
        MultipleChoice =2,
        /// <summary>
        /// 解答题
        /// </summary>
        Answer = 3
    }
}
