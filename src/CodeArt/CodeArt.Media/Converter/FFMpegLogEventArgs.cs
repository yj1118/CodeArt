namespace CodeArt.Media.Converter
{
    using System;
    using System.Runtime.CompilerServices;

    public class FFMpegLogEventArgs : EventArgs
    {
        public FFMpegLogEventArgs(string logData)
        {
            this.Data = logData;
        }

        public string Data { get; private set; }
    }
}

