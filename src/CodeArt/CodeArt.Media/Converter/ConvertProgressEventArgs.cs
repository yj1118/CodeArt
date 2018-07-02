namespace CodeArt.Media.Converter
{
    using System;
    using System.Runtime.CompilerServices;

    public class ConvertProgressEventArgs : EventArgs
    {
        public ConvertProgressEventArgs(TimeSpan processed, TimeSpan totalDuration)
        {
            this.TotalDuration = totalDuration;
            this.Processed = processed;
        }

        public TimeSpan Processed { get; private set; }

        public TimeSpan TotalDuration { get; private set; }
    }
}

