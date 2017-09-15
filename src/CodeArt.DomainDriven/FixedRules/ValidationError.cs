using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CodeArt.DomainDriven
{
    public class ValidationError
    {
        /// <summary>
        /// 错误编码
        /// </summary>
        public string ErrorCode
        {
            get;
            internal set;
        }

        /// <summary>
        /// 错误的消息
        /// </summary>
        public string Message
        {
            get;
            internal set;
        }

        internal ValidationError()
        {
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.ErrorCode) && string.IsNullOrEmpty(this.Message);
        }


        public virtual void Clear()
        {
            this.ErrorCode = string.Empty;
            this.Message = string.Empty;
        }

    }
}
