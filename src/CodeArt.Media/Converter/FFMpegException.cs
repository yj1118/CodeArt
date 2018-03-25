namespace CodeArt.Media.Converter
{
    using System;
    using System.Runtime.CompilerServices;

    public class FFMpegException : Exception
    {
        public FFMpegException(int errCode, string message) : base(string.Format("{0} (exit code: {1})", message, errCode))
        {
            this.ErrorCode = errCode;
        }

        public int ErrorCode { get; private set; }
    }
}

