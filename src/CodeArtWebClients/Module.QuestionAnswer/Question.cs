using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.XamlControls.Metronic;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.ServiceModel;
using CodeArt.Util;
using CodeArt.Web.WebPages;


namespace Module.QuestionAnswer
{
    public class Question : DependencyObject
    {
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register<string, Question>("Type", () => { return "1"; });

        /// <summary>
        /// 问题的类型
        /// </summary>
        public string Type
        {
            get
            {
                return (string)GetValue(TypeProperty);
            }
            set
            {
                SetValue(TypeProperty, value);
            }
        }

        public static readonly DependencyProperty DisabledProperty = DependencyProperty.Register<string, Question>("Disabled", () => { return "false"; });

        public string Disabled
        {
            get
            {
                return (string)GetValue(DisabledProperty);
            }
            set
            {
                SetValue(DisabledProperty, value);
            }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register<string, Question>("Value", () => { return string.Empty; });

        /// <summary>
        /// 问题的值
        /// </summary>
        public string Value
        {
            get
            {
                return (string)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }


        /// <summary>
        /// 是否必填
        /// </summary>
        public static readonly DependencyProperty RequiredProperty = DependencyProperty.Register<bool, Question>("Required",() => { return false; });

        public bool Required
        {
            get
            {
                return (bool)GetValue(RequiredProperty);
            }
            set
            {
                SetValue(RequiredProperty, value);
            }
        }


        public static readonly DependencyProperty TextProperty = DependencyProperty.Register<string, Question>("Text", () => { return string.Empty; });

        /// <summary>
        /// 问题的文本
        /// </summary>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// 选项集合
        /// </summary>
        public readonly static DependencyProperty OptionsProperty = DependencyProperty.Register<UIElementCollection, Question>("Options", () => { return new UIElementCollection(); });

        public UIElementCollection Options
        {
            get
            {
                return GetValue(OptionsProperty) as UIElementCollection;
            }
            set
            {
                SetValue(OptionsProperty, value);
            }
        }

        public Question()
        {
        }


        static Question()
        { }
    }
}
   