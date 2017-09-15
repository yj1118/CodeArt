using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;

namespace CodeArt.DomainDrivenTest.Demo
{
    /// <summary>
    /// 为book扩展能力
    /// </summary>
    [ExtendedClass(typeof(BookExtensions), typeof(BookExtensions2))]
    public static class BookExtensions2
    {
        #region 个性签名2

        /// <summary>
        /// 书的个性签名
        /// </summary>
        [StringLength(Max = 150)]
        [PropertyRepository]
        private static readonly DomainProperty Signature2Property = DomainProperty.Register<string, Book>("Signature2");

        /// <summary>
        /// 扩展类中表示属性的获取和设置用的方法是GetXXX,SetXXX，这是我们的约定
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public static string GetSignature2(this Book book)
        {
            return book.GetValue<string>(Signature2Property);
        }

        /// <summary>
        /// 扩展类中表示属性的获取和设置用的方法是GetXXX,SetXXX，这是我们的约定
        /// </summary>
        /// <param name="book"></param>
        /// <param name="signature"></param>
        public static void SetSignature2(this Book book, string signature)
        {
            book.SetValue(Signature2Property, signature);
        }


        #endregion


    }
}