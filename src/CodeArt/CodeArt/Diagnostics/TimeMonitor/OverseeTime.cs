using System;


namespace CodeArt.Diagnostics
{
    public sealed class OverseeTime
    {
        private string _actionName;
        /// <summary>
        /// 行为名称
        /// </summary>
        public string ActionName
        {
            get { return _actionName; }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            internal set { _message = value; }
        }

        internal OverseeTime(string actionName)
        {
            _actionName = actionName;
        }

        private TimeSpan _elapsed;

        /// <summary>
        /// 时间间隔
        /// </summary>
        public TimeSpan Elapsed
        {
            get
            {
                return _elapsed;
            }
            internal set
            {
                _elapsed = value;
            }
        }

        public string ElapsedString
        {
            get
            {
                return FormattedTime(this.Elapsed);
            }
        }

        public double ElapsedMilliseconds
        {
            get
            {
                return this.Elapsed.TotalMilliseconds;
            }
        }

        public double ElapsedSeconds
        {
            get
            {
                return this.Elapsed.TotalSeconds;
            }
        }

        private static string FormattedTime(TimeSpan ts)
        {
            return String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
        }

    }
}
