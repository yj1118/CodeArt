using System;
using System.Collections.Generic;
using System.Runtime;

namespace CodeArt
{
    public struct DateTimePro
    {
        private DateTime _value;

        private DateTimePro(DateTime value) : this()
        {
            _value = value;
        }

        public static implicit operator DateTime(DateTimePro pro)
        {
            return pro._value;
        }

        public static implicit operator DateTimePro(DateTime value)
        {
            return new DateTimePro(value);
        }

        public static readonly DateTimePro Default = new DateTimePro(new DateTime(1900, 1, 1));

    }
}
