using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

using HtmlAgilityPack;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    public class CustomAttributeCollection : DependencyCollection
    {
        public CustomAttributeCollection() { }

        public object GetValue(string name)
        {
            return GetValue(name, false);
        }

        internal object GetValue(string name,bool ignoreExpression)
        {
            var attr = GetAttribute(name);
            return attr == null ? null : attr.GetValue(CustomAttribute.ValueProperty, ignoreExpression);
        }

        public void SetValue(object owner, string name, object value)
        {
            SetValue(owner, name, value, false);
        }

        internal void SetValue(object owner, string name, object value, bool ignoreExpression)
        {
            var attr = GetAttribute(name);
            if (attr == null)
            {
                attr = new CustomAttribute(owner, name);
                this.Add(attr);
                attr.SetValue(CustomAttribute.ValueProperty, value, ignoreExpression);

                //var exp = value as Expression;
                //if (exp != null)
                //{
                //    exp.OnAttach(owner, name);
                //}
            }
            else
            {
                //var oldValue = attr.GetValue(CustomAttribute.ValueProperty, ignoreExpression);

                attr.SetValue(CustomAttribute.ValueProperty, value, ignoreExpression);

                //var exp = value as Expression;
                //if (exp != null)
                //{
                //    exp.OnAttach(owner, name);
                //}


                //exp = oldValue as Expression;
                //if (exp != null)
                //{
                //    exp.OnDetach(owner, name);
                //}
            }
        }

        public void RemoveValue(string name)
        {
            var attr = GetAttribute(name);
            if (attr != null) this.Remove(attr);
        }

        public CustomAttribute GetAttribute(string name)
        {
            CustomAttribute attr = null;
            foreach (var item in this)
            {
                var t = item as CustomAttribute;
                if (t != null && t.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    attr = t;
                    break;
                }
            }
            return attr;
        }
    }
}