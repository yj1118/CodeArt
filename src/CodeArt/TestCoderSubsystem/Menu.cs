using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;


namespace TestCoderSubsystem
{
    [ObjectRepository(typeof(IMenuRepository))]
    [ObjectValidator()]
    public class Menu : AggregateRoot<Menu, Guid>
    {

        private static readonly DomainProperty MarkedCodeProperty = DomainProperty.Register<string, Menu>("MarkedCode");

        [PropertyRepository()]
        [StringLength(0,50)]
        [ASCIIString]
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


        private static readonly DomainProperty NameProperty = DomainProperty.Register<string, Menu>("Name");
        /// <summary>
        /// 菜单名称
        /// </summary>
        [PropertyRepository()]
        [StringLength(0, 20)]
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



        private static readonly DomainProperty OrderIndexProperty = DomainProperty.Register<int, Menu>("OrderIndex");

        /// <summary>
        /// 序号
        /// </summary>
        [PropertyRepository()]
        [IntRange(0, 99999)]
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


        #region 父菜单

        private static readonly DomainProperty ParentProperty = DomainProperty.Register<Menu, Menu>("Parent");

        [PropertyRepository()]
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


        #endregion

        #region 子菜单

        #region 空对象

        /// <summary>
        /// 建议空对象的定义放在所有领域属性定义之后，避免由于空对象的属性赋值在领域属性定义之前执行的带来的问题
        /// </summary>
        private class MenuEmpty : Menu
        {
            public MenuEmpty()
                : base(Guid.Empty, string.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static Menu Empty = new MenuEmpty();

        #endregion

        //private static readonly DomainProperty ChildsProperty = DomainProperty.RegisterObjectCollection<Menu, Menu>("Childs");


        //private ObjectCollection<Menu> _Childs
        //{
        //    get
        //    {
        //        return GetValue<ObjectCollection<Menu>>(ChildsProperty);
        //    }
        //    set
        //    {
        //        SetValue(ChildsProperty, value);
        //    }
        //}

        //public IEnumerable<Menu> Childs
        //{
        //    get
        //    {
        //        return _Childs;
        //    }
        //}

        #endregion

        [ConstructorRepository]
        public Menu(Guid id, string markedCode)
            : base(id)
        {
            this.MarkedCode = markedCode;
            this.OnConstructed();
        }
    }
}
