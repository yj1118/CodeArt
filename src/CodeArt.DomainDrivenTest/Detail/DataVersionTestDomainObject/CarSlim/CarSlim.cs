using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Data;

using Dapper;

using CodeArt.TestTools;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using CodeArt.Runtime;
using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.DomainDrivenTest.Detail
{
    [ObjectRepository(typeof(ICarSlimRepository), NoCache = true)]
    public class CarSlim : AggregateRoot<CarSlim, Guid>
    {
        #region 类型

        private static readonly DomainProperty CarStyleProperty = DomainProperty.Register<CarStyle, CarSlim>("CarStyle");

        [PropertyRepository()]
        public CarStyle CarStyle
        {
            get
            {
                return GetValue<CarStyle>(CarStyleProperty);
            }
            set
            {
                SetValue(CarStyleProperty, value);
            }
        }

        #endregion

        #region 零件数

        private static readonly DomainProperty PartNumProperty = DomainProperty.Register<byte, CarSlim>("PartNum");

        [PropertyRepository()]
        public byte PartNum
        {
            get
            {
                return GetValue<byte>(PartNumProperty);
            }
            set
            {
                SetValue(PartNumProperty, value);
            }
        }

        #endregion

        #region 车头标

        private static readonly DomainProperty StandProperty = DomainProperty.Register<char, CarSlim>("Stand");

        [PropertyRepository()]
        public char Stand
        {
            get
            {
                return GetValue<char>(StandProperty);
            }
            set
            {
                SetValue(StandProperty, value);
            }
        }

        #endregion

        #region 评价数

        private static readonly DomainProperty CommentProperty = DomainProperty.Register<short, CarSlim>("Comment");

        [PropertyRepository()]
        public short Comment
        {
            get
            {
                return GetValue<short>(CommentProperty);
            }
            set
            {
                SetValue(CommentProperty, value);
            }
        }

        #endregion

        #region 名称

        private static readonly DomainProperty NameProperty = DomainProperty.Register<string, CarSlim>("Name");

        [PropertyRepository()]
        [StringLength(1, 100)]
        public string Name
        {
            get
            {
                return GetValue<string>(NameProperty);
            }
            set
            {
                SetValue(NameProperty, value);
            }
        }

        #endregion

        #region 出厂时间

        private static readonly DomainProperty DeliveryDateProperty = DomainProperty.Register<Emptyable<DateTime>, CarSlim>("DeliveryDate");

        [PropertyRepository()]
        public Emptyable<DateTime> DeliveryDate
        {
            get
            {
                return GetValue<Emptyable<DateTime>>(DeliveryDateProperty);
            }
            set
            {
                SetValue(DeliveryDateProperty, value);
            }
        }

        #endregion

        #region 排序号

        private static readonly DomainProperty OrderIndexProperty = DomainProperty.Register<int, CarSlim>("OrderIndex");

        [PropertyRepository()]
        public int OrderIndex
        {
            get
            {
                return GetValue<int>(OrderIndexProperty);
            }
            set
            {
                SetValue(OrderIndexProperty, value);
            }
        }

        #endregion

        #region 里程数

        private static readonly DomainProperty DistanceProperty = DomainProperty.Register<float, CarSlim>("Distance");

        [PropertyRepository()]
        public float Distance
        {
            get
            {
                return GetValue<float>(DistanceProperty);
            }
            set
            {
                SetValue(DistanceProperty, value);
            }
        }

        #endregion

        #region 价格

        private static readonly DomainProperty PriceProperty = DomainProperty.Register<decimal, CarSlim>("Price");

        [PropertyRepository()]
        public decimal Price
        {
            get
            {
                return GetValue<decimal>(PriceProperty);
            }
            set
            {
                SetValue(PriceProperty, value);
            }
        }

        #endregion

        #region 新旧标识

        private static readonly DomainProperty IsNewCarProperty = DomainProperty.Register<bool, CarSlim>("IsNewCar");

        [PropertyRepository()]
        public bool IsNewCar
        {
            get
            {
                return GetValue<bool>(IsNewCarProperty);
            }
            set
            {
                SetValue(IsNewCarProperty, value);
            }
        }

        #endregion

        #region 空对象

        private class CarSlimEmpty : CarSlim
        {
            public CarSlimEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly CarSlim Empty = new CarSlimEmpty();

        #endregion

        [ConstructorRepository()]
        public CarSlim(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }
    }

    public enum CarStyle
    {
        UNKNOWN = 0,
        BUS = 1,
        SUV = 2,
        TRUCK = 3,
        JEEP = 4,
        SEDAN = 5,

    }

}
