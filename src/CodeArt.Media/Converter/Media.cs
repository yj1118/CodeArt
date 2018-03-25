namespace CodeArt.Media.Converter
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    internal class Media
    {
        public Stream DataStream { get; set; }

        public string Filename { get; set; }

        public string Format { get; set; }
    }
}

