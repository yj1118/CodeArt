using System;


namespace CodeArt.DTO
{
    /// <summary>
    /// 
    /// </summary>
    public class DTOTypeErrorException : Exception
    {
        public DTOTypeErrorException()
        {
        }

        public DTOTypeErrorException(string message)
            : base(message)
        {
        }
    }
}
