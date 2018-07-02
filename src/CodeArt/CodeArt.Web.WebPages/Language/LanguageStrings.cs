using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Xml;
using System.Dynamic;

using CodeArt.DTO;
using CodeArt.Util;
using CodeArt.AppSetting;

namespace CodeArt.Web.WebPages
{
    public class LanguageStrings : DynamicObject
    {
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = LanguageResources.Get(binder.Name);
            return true;
        }

        public static readonly LanguageStrings Instance = new LanguageStrings();
    }
}
