using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;
using AccountSubsystem;

namespace MenuSubsystem
{
    [ObjectRepository(typeof(IMenuRepository))]
    [ObjectValidator(typeof(MenuSpecification))]
    public class Menu : AggregateRoot<Menu, long>
    {

        [PropertyRepository()]
        [NotEmpty()]
        [StringLength(1, 10)]
        public static readonly DomainProperty NameProperty = DomainProperty.Register<string, Menu>("Name");

        /// <summary>
        /// 菜单名称
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

        /// <summary>
        /// 标示码
        /// </summary>
        [PropertyRepository()]
        [StringLength(0, 25)]
        [StringFormat(StringFormat.Letter | StringFormat.Number, "_")]
        public static readonly DomainProperty MarkedCodeProperty = DomainProperty.Register<string, Menu>("MarkedCode");

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
        public static readonly DomainProperty SortNumberProperty = DomainProperty.Register<int, Menu>("SortNumber", 0);

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

        [PropertyRepository()]
        [StringLength(0, 100)]
        public static readonly DomainProperty IconProperty = DomainProperty.Register<string, Menu>("Icon");

        /// <summary>
        /// 图标名称，这是字体样式的名称，不是图片路径
        /// </summary>
        public string Icon
        {
            get
            {
                return GetValue<string>(IconProperty);
            }
            set
            {
                SetValue(IconProperty, value);
            }
        }

        [PropertyRepository()]
        public static readonly DomainProperty TagsProperty = DomainProperty.RegisterCollection<string, Menu>("Tags");

        /// <summary>
        /// 菜单的标签
        /// </summary>
        public DomainCollection<string> Tags
        {
            get
            {
                return GetValue<DomainCollection<string>>(TagsProperty);
            }
            set
            {
                SetValue(TagsProperty, value);
            }
        }

        #region 父菜单


        [PropertyRepository(Lazy = true)]
        [PropertySet("SetParent")]
        public static readonly DomainProperty ParentProperty = DomainProperty.Register<Menu, Menu>("Parent", (obj) => { return Menu.Empty; });


        public Menu Parent
        {
            get
            {
                return GetValue<Menu>(ParentProperty);
            }
            set
            {
                SetValue(ParentProperty, value);
            }
        }

        protected virtual void SetParent(Menu value)
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

        #region 子菜单

        /// <summary>
        /// 子菜单
        /// </summary>
        [PropertyRepository(Lazy = true)]
        public static readonly DomainProperty ChildsProperty = DomainProperty.RegisterCollection<Menu, Menu>("Childs");

        private DomainCollection<Menu> _Childs
        {
            get
            {
                return GetValue<DomainCollection<Menu>>(ChildsProperty);
            }
            set
            {
                SetValue(ChildsProperty, value);
            }
        }

        /// <summary>
        /// 子菜单
        /// </summary>
        public IEnumerable<Menu> Childs
        {
            get
            {
                return _Childs;
            }
        }

        /// <summary>
        /// 删除子菜单
        /// </summary>
        private void Clear()
        {
            var repository = Repository.Create<IMenuRepository>();
            var childs = repository.FindChilds(this.Id, QueryLevel.Mirroring);
            foreach (var child in childs)
            {
                repository.Delete(child);
            }
        }

        #endregion

        #region 菜单行为相关

        [PropertyRepository]
        [NotEmpty()]
        [Specification()]
        private static readonly DomainProperty BehaviorProperty = DomainProperty.Register<MenuBehavior, Menu>("Behavior", MenuBehavior.Empty);

        public MenuBehavior Behavior
        {
            get
            {
                return GetValue<MenuBehavior>(BehaviorProperty);
            }
            set
            {
                SetValue(BehaviorProperty, value);
            }
        }

        public virtual string GetBehaviorCode()
        {
            return this.Behavior.GetCode();
        }

        //public bool CanRender(Account account, IMenuRender render)
        //{
        //    return this.Behavior.CanRender(account, render);
        //}

        #endregion

        #region 权限

        [PropertyRepository]
        private static readonly DomainProperty ItemsProperty = DomainProperty.RegisterCollection<Permission, Menu>("Items");

        private DomainCollection<Permission> ItemsImpl
        {
            get
            {
                return GetValue<DomainCollection<Permission>>(ItemsProperty);
            }
            set
            {
                SetValue(ItemsProperty, value);
            }
        }

        /// <summary>
        /// 为该菜单分配的访问权限项
        /// </summary>
        public IEnumerable<Permission> Items
        {
            get
            {
                return this.ItemsImpl;
            }
        }

        public void AddItems(IEnumerable<Permission> items)
        {
            ItemsImpl = new DomainCollection<Permission>(ItemsProperty, items);
        }

        internal bool ContainsItem(Permission item)
        {
            return this.ItemsImpl.Contains(item);
        }

        /// <summary>
        /// 菜单没有设置相关的功能
        /// </summary>
        /// <returns></returns>
        public bool IsNonePermission()
        {
            return this.ItemsImpl.Count == 0;
        }


        #endregion


        [ConstructorRepository()]
        internal Menu(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }

        #region 空对象

        private class MenuEmpty : Menu
        {
            public MenuEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public new static readonly Menu Empty = new MenuEmpty();

        #endregion
    }
}
