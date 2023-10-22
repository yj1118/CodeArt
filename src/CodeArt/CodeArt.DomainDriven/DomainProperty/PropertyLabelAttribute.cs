using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Resources;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.AppSetting;


namespace CodeArt.DomainDriven
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PropertyLabelAttribute : Attribute
    {
        public string Name
        {
            get;
            private set;
        }

        public string Value
        {
            get
            {
                return _getValue(AppSession.Language)(this.Name);
            }
        }

        public PropertyLabelAttribute(string name)
        {
            this.Name = name;
        }

        private static Func<string, Func<string, string>> _getValue = LazyIndexer.Init<string, Func<string, string>>((language) =>
        {
            return LazyIndexer.Init<string, string>((name) =>
            {
                return LanguageUtil.GetString(language, name);
            });
        });

    }
}