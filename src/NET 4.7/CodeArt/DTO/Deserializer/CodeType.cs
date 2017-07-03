using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

using CodeArt.DTO;
using CodeArt.Util;

namespace CodeArt.DTO
{
    internal enum CodeType
    {
       Object,
       List,
       StringValue,
       NonStringValue
    }
}