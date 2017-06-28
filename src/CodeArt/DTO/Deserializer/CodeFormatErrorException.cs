using System;


namespace CodeArt.DTO
{
    /// <summary>
    /// 代码格式错误
    /// </summary>
    public class CodeFormatErrorException : Exception
    {
        public CodeFormatErrorException()
        {
        }

        public CodeFormatErrorException(string message)
            : base(message)
        {
        }
    }
}
