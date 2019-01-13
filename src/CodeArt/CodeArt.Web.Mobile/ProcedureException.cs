using System;

namespace CodeArt.Web.Mobile
{
    /// <summary>
    /// 
    /// </summary>
    public class ProcedureException : Exception
    {
        public ProcedureException()
            : base()
        {
        }

        public ProcedureException(string message)
            : base(message)
        {
        }

        public ProcedureException(string message, Exception innerException)
            : base(message,innerException)
        {
        }

    }

    /// <summary>
    /// 调用过程返回的结果中出现的错误
    /// </summary>
    public class InvokeProcedureException : ProcedureException
    {
        public InvokeProcedureException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// 调用过程返回的结果中出现的错误
    /// </summary>
    public class InvokeProcedureUserException : UserUIException
    {
        public InvokeProcedureUserException(string message)
            : base(message)
        {
        }
    }

}
