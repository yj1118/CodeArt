using System;
using System.Collections.Generic;
using System.Runtime;

namespace CodeArt.DomainDriven
{
    [ObjectValidator()]
    public class TimeRange : ValueObject
    {
        [PropertyRepository()]
        public static readonly DomainProperty StartProperty = DomainProperty.Register<Emptyable<DateTime>, TimeRange>("Start");


        public Emptyable<DateTime> Start
        {
            get
            {
                return GetValue<Emptyable<DateTime>>(StartProperty);
            }
            private set
            {
                SetValue(StartProperty, value);
            }
        }

        [PropertyRepository()]
        public static readonly DomainProperty EndProperty = DomainProperty.Register<Emptyable<DateTime>, TimeRange>("End");


        public Emptyable<DateTime> End
        {
            get
            {
                return GetValue<Emptyable<DateTime>>(EndProperty);
            }
            private set
            {
                SetValue(EndProperty, value);
            }
        }

        public string StartText
        {
            get
            {
                return this.Start.IsEmpty() ? string.Empty : this.Start.Value.ToString("yyyy-MM-dd");
            }
        }

        public string EndText
        {
            get
            {
                return this.End.IsEmpty() ? string.Empty : this.End.Value.ToString("yyyy-MM-dd");
            }
        }

        public override ValidationResult Validate()
        {
            if (this.Start.IsEmpty() || this.End.IsEmpty()) return base.Validate();
            if (this.Start.Value > this.End.Value) throw new DomainDrivenException(Extensions.Strings.StartTimeLaterEndTime);
            return base.Validate();
        }

        [ConstructorRepository]
        public TimeRange(Emptyable<DateTime> start, Emptyable<DateTime> end)
        {
            this.Start = start;
            this.End = end;
            this.OnConstructed();
        }

        public bool IsSameDay()
        {
            if (this.Start.IsEmpty() || this.End.IsEmpty()) return false;
            return this.Start.Value.Year == this.End.Value.Year && this.Start.Value.DayOfYear == this.End.Value.DayOfYear;
        }

        public void Each(Action<DateTime> action)
        {
            if (this.Start.IsEmpty() || this.End.IsEmpty()) return;
            var max = this.End.Value.AddDays(1);
            for (var current = this.Start; current < max; current = current.Value.AddDays(1))
            {
                action(current);
            }
        }

        public bool InRange(DateTime value)
        {
            return WithinRange(value, this.Start, this.End);
        }

        #region 工具方法

        public static bool WithinRange(DateTime value, DateTime start, DateTime end)
        {
            if (value.Year < start.Year || value.Year > end.Year) return false;
            if ((value.Year == start.Year && value.Month < start.Month) || (value.Year == end.Year && value.Month > end.Month)) return false;
            if ((value.Year == start.Year && value.Month == start.Month && value.Day < start.Day)
                    || (value.Year == end.Year && value.Month == end.Month && value.Day > end.Day)) return false;
            return true;
        }

        /// <summary>
        /// 获得当天的时间段
        /// </summary>
        /// <returns></returns>
        public static TimeRange GetThatDay()
        {
            //var yesterday = DateTime.Now.AddDays(-1);
            var now = DateTime.Now;
            var start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            var end = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            return new TimeRange(start, end);
        }

        #endregion


        public override bool IsEmpty()
        {
            return false;
        }

        private class TimeRangeEmpty : TimeRange
        {
            public TimeRangeEmpty()
                : base(Emptyable<DateTime>.CreateEmpty(), Emptyable<DateTime>.CreateEmpty())
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly TimeRange Empty = new TimeRangeEmpty();
    }
}
