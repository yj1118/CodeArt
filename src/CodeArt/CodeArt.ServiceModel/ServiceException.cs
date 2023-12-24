using System;
using System.ServiceModel;

namespace CodeArt.ServiceModel
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceException : Exception
    {
        public ServiceException()
            : base()
        {
        }

        public ServiceException(string message)
            : base(message)
        {
        }

        public ServiceException(string message, Exception innerException)
            : base(message,innerException)
        {
        }

    }

    /// <summary>
    /// 调用服务返回的结果中出现的错误
    /// </summary>
    public class InvokeServiceException : ServiceException
    {
        public InvokeServiceException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// 调用服务返回的结果中出现的错误
    /// </summary>
    public class InvokeServiceUserException : UserUIException
    {
        public InvokeServiceUserException(string message)
            : base(message)
        {
        }
    }

}
