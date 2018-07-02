using System;


namespace CodeArt.DTO
{
    /// <summary>
    /// 
    /// </summary>
    public class NotFoundDTEntityException : Exception
    {
        public NotFoundDTEntityException()
        {
        }

        public NotFoundDTEntityException(string message)
            : base(message)
        {
        }
    }
}
