using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DomainDriven
{
    public class StringFormatAttribute : PropertyValidatorAttribute
    {
        private StringFormat _formatValue;
        private string[] _contains;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="contains">可以包含的文本</param>
        public StringFormatAttribute(StringFormat format, params string[] contains)
        {
            _formatValue = format;
            _contains = contains;
        }

        private string _pattern;

        public StringFormatAttribute(string pattern)
        {
            _pattern = pattern;
        }

        public override IPropertyValidator CreateValidator()
        {
            if (!string.IsNullOrEmpty(_pattern)) return new StringFormatValidator(_pattern);
            return new StringFormatValidator(_formatValue, _contains);
        }
    }
}