using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;

using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 字符串格式验证
    /// </summary>
    public class StringFormatValidator : PropertyListabelValidator<string>
    {
        private StringFormat _formatValue;

        private string[] _contains;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="contains">可以包含的文本</param>
        public StringFormatValidator(StringFormat format, string[] contains)
        {
            _formatValue = format;
            _contains = contains;
        }

        private string _pattern;

        private RegexPool _regex;

        public StringFormatValidator(string pattern)
        {
            _pattern = pattern;
            _regex = new RegexPool(pattern, RegexOptions.IgnoreCase);
        }

        protected override void Validate(DomainObject domainObject, DomainProperty property, string propertyValue, ValidationResult result)
        {
            if (string.IsNullOrEmpty(propertyValue)) return;

            if (!string.IsNullOrEmpty(_pattern))
            {
                if (!_regex.IsMatch(propertyValue))
                {
                    result.AddError(property.Name, StringFormatError, string.Format(Strings.DoesNotMeetRule, property.Call, _pattern));
                }
            }
            else
            {
                var value = propertyValue;
                if (_contains.Length > 0)
                {
                    foreach (string temp in _contains)
                    {
                        value = value.Replace(temp, string.Empty);
                    }
                }

                RegexPool reg = null;
                string message = null;
                if (IsChineseAndLetterAndNumber(_formatValue))
                {
                    reg = _chineseAndLetterAndNumberRegex;
                    message = Strings.CEN;
                }
                else if (IsLetterAndNumber(_formatValue))
                {
                    reg = _letterAndNumberRegex;
                    message = Strings.EN;
                }
                else if (IsChineseAndLetter(_formatValue))
                {
                    reg = _chineseAndLetterRegex;
                    message = Strings.CE;
                }
                else if (IsChineseAndNumber(_formatValue))
                {
                    reg = _chineseAndNumberRegex;
                    message = Strings.CN;
                }

                if (reg != null && !reg.IsMatch(value))
                {
                    result.AddError(property.Name, StringFormatError, string.Format(Strings.CanOnlyInclude, property.Call, message));
                }
            }
        }


        #region 格式


        private static RegexPool _chineseAndLetterAndNumberRegex = new RegexPool(@"^[A-Za-z0-9\u4e00-\u9fa5]+$", RegexOptions.IgnoreCase);


        private static bool IsFormatType(StringFormat value, StringFormat formatType)
        {
            return (value & formatType) != 0;
        }


        private static bool IsChineseAndLetterAndNumber(StringFormat value)
        {
            const StringFormat formatValue = StringFormat.Chinese | StringFormat.Letter | StringFormat.Number;
            return value == formatValue;
        }

        private static RegexPool _chineseAndLetterRegex = new RegexPool(@"^[A-Za-z\u4e00-\u9fa5]+$", RegexOptions.IgnoreCase);

        private static bool IsChineseAndLetter(StringFormat value)
        {
            const StringFormat formatValue = StringFormat.Chinese | StringFormat.Letter;
            return value == formatValue;
        }

        private static RegexPool _letterAndNumberRegex = new RegexPool(@"^[A-Za-z0-9]+$", RegexOptions.IgnoreCase);

        private static bool IsLetterAndNumber(StringFormat value)
        {
            const StringFormat formatValue = StringFormat.Letter | StringFormat.Number;
            return value == formatValue;
        }

        private static RegexPool _chineseAndNumberRegex = new RegexPool(@"^[0-9\u4e00-\u9fa5]+$", RegexOptions.IgnoreCase);

        private static bool IsChineseAndNumber(StringFormat value)
        {
            const StringFormat formatValue = StringFormat.Chinese | StringFormat.Number;
            return value == formatValue;
        }

        #endregion

        public const string StringFormatError = "StringFormatError";

    }

    public enum StringFormat : int
    {
        Letter = 0x0100,
        Number = 0x0200,
        Chinese = 0x0400,
    }
}
