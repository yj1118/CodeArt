namespace CodeArt.Media.Converter
{
    using System;

    public class OutputSettings
    {
        public string AudioCodec;
        public int? AudioSampleRate = null;
        public string CustomOutputArgs;
        public float? MaxDuration = null;
        public string VideoCodec;
        public int? VideoFrameCount = null;
        public int? VideoFrameRate = null;
        public string VideoFrameSize;

        internal void CopyTo(OutputSettings outputSettings)
        {
            outputSettings.AudioSampleRate = this.AudioSampleRate;
            outputSettings.AudioCodec = this.AudioCodec;
            outputSettings.VideoFrameRate = this.VideoFrameRate;
            outputSettings.VideoFrameCount = this.VideoFrameCount;
            outputSettings.VideoFrameSize = this.VideoFrameSize;
            outputSettings.VideoCodec = this.VideoCodec;
            outputSettings.MaxDuration = this.MaxDuration;
            outputSettings.CustomOutputArgs = this.CustomOutputArgs;
        }

        public void SetVideoFrameSize(int width, int height)
        {
            this.VideoFrameSize = string.Format("{0}x{1}", width, height);
        }
    }
}

