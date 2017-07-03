using System;


namespace CodeArt.DTO
{
    /// <summary>
    /// 
    /// </summary>
    public class DTOException : Exception
    {
        public DTOException()
        {
        }

        public DTOException(string message)
            : base(message)
        {
        }
    }
}