namespace CodeArt.Media.Converter
{
    using System;

    public class ConvertSettings : OutputSettings
    {
        public bool AppendSilentAudioStream;
        public string CustomInputArgs;
        public float? Seek = null;
        public string AudioDevice;
    }
}

