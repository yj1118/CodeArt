using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace CodeArt.Web.WebPages.Xaml
{
    internal interface IPropertyMetadata
    {
        /// <summary>
        /// 属性的默认值
        /// </summary>
        /// <returns></returns>
        Func<object> GetDefaultValue { get; }

        PropertyChangedCallback ChangedCallBack { get; }

        PreSetValueCallback PreSetValueCallback { get; }

        GotValueCallback GotValueCallback { get; }


        #region 对外暴露的事件

        event DependencyPropertyChangedEventHandler Changed;

        event DependencyPropertyPreSetEventHandler PreSet;

        event DependencyPropertyGotEventHandler Got;

        #endregion

    }
}