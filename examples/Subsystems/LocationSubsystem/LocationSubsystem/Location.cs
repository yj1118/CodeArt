using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CodeArt.DomainDriven;

namespace LocationSubsystem
{
    [ObjectRepository(typeof(ILocationRepository))]
    [ObjectValidator(typeof(LocationValidator))]
    public class Location : AggregateRoot<Location, long>
    {
        [PropertyRepository()]
        [NotEmpty]
        [StringLength(1,100)]
        public static readonly DomainProperty NameProperty = DomainProperty.Register<string, Location>("Name");

        /// <summary>
        /// 地理位置的名称
        /// </summary>
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

        [PropertyRepository()]
        [StringLength(0, 50)]
        [StringFormat(StringFormat.Letter | StringFormat.Number)]
        public static readonly DomainProperty MarkedCodeProperty = DomainProperty.Register<string, Location>("MarkedCode");

        /// <summary>
        /// 引用标识
        /// </summary>
        public string MarkedCode
        {
            get
            {
                return GetValue<string>(MarkedCodeProperty);
            }
            set
            {
                SetValue(MarkedCodeProperty, value);
            }
        }

        /// <summary>
        /// 是否定义了标示符
        /// </summary>
        public bool DeclareMarkedCode
        {
            get
            {
                return !string.IsNullOrEmpty(this.MarkedCode);
            }
        }

        [PropertyRepository()]
        [IntRange()]
        public static readonly DomainProperty SortNumberProperty = DomainProperty.Register<int, Location>("SortNumber", 0);

        /// <summary>
        /// 排序的序号
        /// </summary>
        public int SortNumber
        {
            get
            {
                return GetValue<int>(SortNumberProperty);
            }
            set
            {
                SetValue(SortNumberProperty, value);
            }
        }

        #region 路径

        /// <summary>
        /// 路径，该属性的值自动生成，不用存储
        /// </summary>
        [PropertyGet("GetPath")]
        public static readonly DomainProperty PathProperty = DomainProperty.Register<LocationPath, Location>("Path", (directory) => { return LocationPath.Empty; });

        public LocationPath Path
        {
            get
            {
                return GetValue<LocationPath>(PathProperty);
            }
        }

        protected virtual LocationPath GetPath()
        {
            ILocationRepository repository = Repository.Create<ILocationRepository>();
            var parents = repository.FindParents(this.Id);
            return new LocationPath(parents);
        }

        #endregion

        #region 父位置


        [PropertyRepository(Lazy = true)]
        [PropertySet("SetParent")]
        public static readonly DomainProperty ParentProperty = DomainProperty.Register<Location, Location>("Parent", (obj) => { return Location.Empty; });


        public Location Parent
        {
            get
            {
                return GetValue<Location>(ParentProperty);
            }
            set
            {
                SetValue(ParentProperty, value);
            }
        }

        protected virtual void SetParent(Location value)
        {
            if (value == null) return;

            if (value.IsEmpty()) this.Parent = value;
            else
            {
                bool existChild = value.Childs.Contains(this);

                if (!existChild)
                {
                    value._Childs.Add(this);
                    this.Parent = value;
                }
            }
        }

        #endregion 

        #region 子地理位置

        /// <summary>
        /// 子地理位置
        /// </summary>
        [PropertyRepository(Lazy = true)]
        public static readonly DomainProperty ChildsProperty = DomainProperty.RegisterCollection<Location, Location>("Childs");

        private DomainCollection<Location> _Childs
        {
            get
            {
                return GetValue<DomainCollection<Location>>(ChildsProperty);
            }
            set
            {
                SetValue(ChildsProperty, value);
            }
        }

        /// <summary>
        /// 子地理位置
        /// </summary>
        public IEnumerable<Location> Childs
        {
            get
            {
                return _Childs;
            }
        }

        /// <summary>
        /// 删除子地理位置
        /// </summary>
        private void Clear()
        {
            var repository = Repository.Create<ILocationRepository>();
            var childs = repository.FindChilds(this.Id, QueryLevel.Mirroring);
            foreach (var child in childs)
            {
                repository.Delete(child);
            }
        }

        #endregion


        [ConstructorRepository]
        public Location(long id)
            : base(id)
        {
            this.OnConstructed();
        }

        public override void OnPreDelete()
        {
            this.Clear(); //删除该地理位置之前，删除子位置
            base.OnPreDelete();
        }


        private class LocationEmpty : Location
        {
            public LocationEmpty()
                : base(0)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly Location Empty = new LocationEmpty();
    }
}
