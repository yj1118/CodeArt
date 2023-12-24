﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.WebPages.Xaml.Script;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 在执行行为之前触发
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="args"></param>
    /// <param name="allow"></param>
    /// <returns></returns>
    public delegate object PreCallActionCallback(DependencyObject obj, object[] args, ref bool allow);
}