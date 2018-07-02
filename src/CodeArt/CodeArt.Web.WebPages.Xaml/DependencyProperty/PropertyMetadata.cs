using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace CodeArt.Web.WebPages.Xaml
{
    public class PropertyMetadata : IPropertyMetadata
    {
        private PropertyMetadata() { }

        public PropertyMetadata(Func<object> getDefaultValue)
           : this(getDefaultValue, null, null, null)
        {
        }

        public PropertyMetadata(Func<object> getDefaultValue, GotValueCallback gotValueCallback)
           : this(getDefaultValue, null, null, gotValueCallback)
        {
        }

        public PropertyMetadata(Func<object> getDefaultValue, PropertyChangedCallback propertyChangedCallback)
            : this(getDefaultValue, propertyChangedCallback, null, null)
        {
        }

        public PropertyMetadata(Func<object> getDefaultValue, PropertyChangedCallback propertyChangedCallback, PreSetValueCallback preSetValueCallback, GotValueCallback gotValueCallback)
        {
            this.GetDefaultValue = getDefaultValue;
            this.ChangedCallBack = propertyChangedCallback;
            this.PreSetValueCallback = preSetValueCallback;
            this.GotValueCallback = gotValueCallback;
        }

        /// <summary>
        /// 属性的默认值
        /// </summary>
        /// <returns></returns>
        public Func<object> GetDefaultValue { get; private set; }

        public PropertyChangedCallback ChangedCallBack { get; private set; }

        public PreSetValueCallback PreSetValueCallback { get; private set; }


        public GotValueCallback GotValueCallback { get; private set; }

        public bool IsRegisteredChanged
        {
            get
            {
                return this.Changed != null;
            }
        }

        internal void OnChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Changed != null) this.Changed(sender, e);
        }

        public event DependencyPropertyChangedEventHandler Changed;

        public bool IsRegisteredPreSet
        {
            get
            {
                return this.PreSet != null;
            }
        }

        internal void OnPreSet(object sender, DependencyPropertyPreSetEventArgs e)
        {
            if (this.PreSet != null) this.PreSet(sender, e);
        }

        public event DependencyPropertyPreSetEventHandler PreSet;

        public bool IsRegisteredGot
        {
            get
            {
                return this.Got != null;
            }
        }

        internal void OnGot(object sender, DependencyPropertyGotEventArgs e)
        {
            if (this.Got != null) this.Got(sender, e);
        }

        public event DependencyPropertyGotEventHandler Got;

    }
}