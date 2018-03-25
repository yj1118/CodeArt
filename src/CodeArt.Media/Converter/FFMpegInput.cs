namespace CodeArt.Media.Converter
{
    using System;
    using System.Runtime.CompilerServices;

    public class FFMpegInput
    {
        public FFMpegInput(string input) : this(input, null)
        {
        }

        public FFMpegInput(string input, string format)
        {
            this.Input = input;
            this.Format = format;
        }

        public string CustomInputArgs { get; set; }

        public string Format { get; set; }

        public string Input { get; set; }
    }
}

