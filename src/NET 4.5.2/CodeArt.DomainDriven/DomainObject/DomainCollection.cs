using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;


using CodeArt.Util;
using CodeArt.Runtime;


namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 领域集合
    /// </summary>
    /// <typeparam name="TMember"></typeparam>
    public class DomainCollection<TMember> : ObservableCollection<TMember>, IDomainCollection

    {
        public DomainObject Parent
        {
            get;
            set;
        }

        /// <summary>
        /// 集合在父对象中所担当的属性定义
        /// </summary>
        public DomainProperty PropertyInParent
        {
            get;
            private set;
        }

        public DomainCollection(DomainProperty propertyInParent)
            : this(propertyInParent, EmptyArray<TMember>.Value)
        {
        }

        public DomainCollection(DomainProperty propertyInParent, IEnumerable<TMember> items)
            : base(items)
        {
            this.PropertyInParent = propertyInParent;
            this.CollectionChanged += OnCollectionChanged;
        }

        /// <summary>
        /// 当集合发生改变时，通知父对象
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.Parent == null) return;
            if (this.Parent.IsConstructing) return; //构造时不用标记
            this.Parent.MarkPropertyChanged(this.PropertyInParent);
        }
    }
}