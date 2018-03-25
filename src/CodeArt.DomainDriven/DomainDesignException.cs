using System;

using CodeArt.Log;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 表示领域设计错误
    /// </summary>
    [NonLog]
    public class DomainDesignException : DomainDrivenException
    {
        public DomainDesignException()
            : base()
        {
        }

        public DomainDesignException(string message)
            : base(message)
        {
        }
    }
}
