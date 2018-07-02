namespace CodeArt.Media.Converter
{
    using System;
    using System.Text.RegularExpressions;

    internal class FFMpegProgress
    {
        private static Regex DurationRegex = new Regex(@"Duration:\s(?<duration>[0-9:.]+)([,]|$)", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private bool Enabled = true;
        private ConvertProgressEventArgs lastProgressArgs;
        internal float? MaxDuration = null;
        private Action<ConvertProgressEventArgs> ProgressCallback;
        private int progressEventCount;
        private static Regex ProgressRegex = new Regex(@"time=(?<progress>[0-9:.]+)\s", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
        internal float? Seek = null;

        internal FFMpegProgress(Action<ConvertProgressEventArgs> progressCallback, bool enabled)
        {
            this.ProgressCallback = progressCallback;
            this.Enabled = enabled;
        }

        internal void Complete()
        {
            if ((this.Enabled && (this.lastProgressArgs != null)) && (this.lastProgressArgs.Processed < this.lastProgressArgs.TotalDuration))
            {
                this.ProgressCallback(new ConvertProgressEventArgs(this.lastProgressArgs.TotalDuration, this.lastProgressArgs.TotalDuration));
            }
        }

        private TimeSpan CorrectDuration(TimeSpan totalDuration)
        {
            if (totalDuration != TimeSpan.Zero)
            {
                if (this.Seek.HasValue)
                {
                    TimeSpan ts = TimeSpan.FromSeconds((double) this.Seek.Value);
                    totalDuration = (totalDuration > ts) ? totalDuration.Subtract(ts) : TimeSpan.Zero;
                }
                if (this.MaxDuration.HasValue)
                {
                    TimeSpan span2 = TimeSpan.FromSeconds((double) this.MaxDuration.Value);
                    if (totalDuration > span2)
                    {
                        totalDuration = span2;
                    }
                }
            }
            return totalDuration;
        }

        internal void ParseLine(string line)
        {
            if (this.Enabled)
            {
                TimeSpan totalDuration = (this.lastProgressArgs != null) ? this.lastProgressArgs.TotalDuration : TimeSpan.Zero;
                Match match = DurationRegex.Match(line);
                if (match.Success)
                {
                    TimeSpan zero = TimeSpan.Zero;
                    if (TimeSpan.TryParse(match.Groups["duration"].Value, out zero))
                    {
                        TimeSpan span3 = totalDuration.Add(zero);
                        this.lastProgressArgs = new ConvertProgressEventArgs(TimeSpan.Zero, span3);
                    }
                }
                Match match2 = ProgressRegex.Match(line);
                if (match2.Success)
                {
                    TimeSpan result = TimeSpan.Zero;
                    if (TimeSpan.TryParse(match2.Groups["progress"].Value, out result))
                    {
                        if (this.progressEventCount == 0)
                        {
                            totalDuration = this.CorrectDuration(totalDuration);
                        }
                        this.lastProgressArgs = new ConvertProgressEventArgs(result, (totalDuration != TimeSpan.Zero) ? totalDuration : result);
                        this.ProgressCallback(this.lastProgressArgs);
                        this.progressEventCount++;
                    }
                }
            }
        }

        internal void Reset()
        {
            this.progressEventCount = 0;
            this.lastProgressArgs = null;
        }
    }
}

