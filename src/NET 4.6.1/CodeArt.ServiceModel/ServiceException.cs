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
}
