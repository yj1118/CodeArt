using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace CodeArt.DomainDrivenTest.Detail
{
    [DerivedClass(typeof(GoldenDog), "{206DB9AD-0ECF-42b9-968D-44389BAC3752}")]
    [ObjectRepository(typeof(IGoldenDogRepository))]
    public class GoldenDog : Dog
    {
        private static readonly DomainProperty LegCountsProperty = DomainProperty.RegisterCollection<int, GoldenDog>("LegCounts");

        [PropertyRepository()]
        public DomainCollection<int> LegCounts
        {
            get
            {
                return GetValue<DomainCollection<int>>(LegCountsProperty);
            }
            set
            {
                SetValue(LegCountsProperty, value);
            }
        }


        private static readonly DomainProperty LegColorProperty = DomainProperty.Register<AnimalColor, GoldenDog>("LegColor", AnimalColor.Empty);

        [PropertyRepository()]
        public AnimalColor LegColor
        {
            get
            {
                return GetValue<AnimalColor>(LegColorProperty);
            }
            set
            {
                SetValue(LegColorProperty, value);
            }
        }


        [PropertyRepository()]
        private static readonly DomainProperty HeadColorsProperty = DomainProperty.RegisterCollection<AnimalColor, GoldenDog>("HeadColors");

        private DomainCollection<AnimalColor> _HeadColors
        {
            get
            {
                return GetValue<DomainCollection<AnimalColor>>(HeadColorsProperty);
            }
            set
            {
                SetValue(HeadColorsProperty, value);
            }
        }

        public IEnumerable<AnimalColor> HeadColors
        {
            get
            {
                return _HeadColors;
            }
            set
            {
                _HeadColors = new DomainCollection<AnimalColor>(HeadColorsProperty, value);
            }
        }

        public void AddHeadColor(AnimalColor color)
        {
            _HeadColors.Add(color);
        }

        public void RemoveHeadColor(AnimalColor color)
        {
            _HeadColors.Remove(color);
        }

        private static readonly DomainProperty GoldenWheelProperty = DomainProperty.Register<AnimalWheel, GoldenDog>("GoldenWheel", AnimalWheel.Empty);

        [PropertyRepository()]
        public AnimalWheel GoldenWheel
        {
            get
            {
                return GetValue<AnimalWheel>(GoldenWheelProperty);
            }
            set
            {
                SetValue(GoldenWheelProperty, value);
            }
        }


        private static readonly DomainProperty GoldenBreakProperty = DomainProperty.Register<AnimalBreak, GoldenDog>("GoldenBreak", AnimalBreak.Empty);

        [PropertyRepository(Lazy = true)]
        public AnimalBreak GoldenBreak
        {
            get
            {
                return GetValue<AnimalBreak>(GoldenBreakProperty);
            }
            set
            {
                SetValue(GoldenBreakProperty, value);
            }
        }


        private static readonly DomainProperty GoldenWheelsProperty = DomainProperty.RegisterCollection<AnimalWheel, GoldenDog>("GoldenWheels");

        private DomainCollection<AnimalWheel> _GoldenWheels
        {
            get
            {
                return GetValue<DomainCollection<AnimalWheel>>(GoldenWheelsProperty);
            }
            set
            {
                SetValue(GoldenWheelsProperty, value);
            }
        }

        [PropertyRepository(Lazy = true)]
        public IEnumerable<AnimalWheel> GoldenWheels
        {
            get
            {
                return _GoldenWheels;
            }
        }

        public void AddGoldenWheel(AnimalWheel wheel)
        {
            _GoldenWheels.Add(wheel);
        }

        public void RemoveGoldenWheel(int wheelId)
        {
            var wheel = _GoldenWheels.FirstOrDefault((t) => t.Id == wheelId);
            _GoldenWheels.Remove(wheel);
        }


        private static readonly DomainProperty GoldenBreaksProperty = DomainProperty.RegisterCollection<AnimalBreak, GoldenDog>("GoldenBreaks");

        private DomainCollection<AnimalBreak> _GoldenBreaks
        {
            get
            {
                return GetValue<DomainCollection<AnimalBreak>>(GoldenBreaksProperty);
            }
            set
            {
                SetValue(GoldenBreaksProperty, value);
            }
        }

        [PropertyRepository(Lazy = true)]
        public IEnumerable<AnimalBreak> GoldenBreaks
        {
            get
            {
                return _GoldenBreaks;
            }
        }

        public void AddGoldenBreak(AnimalBreak breaker)
        {
            _GoldenBreaks.Add(breaker);
        }

        public void RemoveGoldenBreak(long breakId)
        {
            var breaker = _GoldenBreaks.FirstOrDefault((t) => t.Id == breakId);
            _GoldenBreaks.Remove(breaker);
        }

        #region 空对象

        private class GoldenDogEmpty : GoldenDog
        {
            public GoldenDogEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly new GoldenDog Empty = new GoldenDogEmpty();

        #endregion

        [ConstructorRepository()]
        public GoldenDog(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }

    }
}
