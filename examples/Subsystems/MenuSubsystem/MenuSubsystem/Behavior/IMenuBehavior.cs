using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;
using AccountSubsystem;

namespace MenuSubsystem
{
    public interface IMenuBehavior : INotNullObject
    {
        /// <summary>
        /// 得到行为代码
        /// </summary>
        /// <returns></returns>
        string GetCode();

        IMenuBehavior Clone();

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="role"></param>
        ///// <param name="render">外界指定的呈现器</param>
        ///// <returns></returns>
        //bool CanRender(Account account, IMenuRender render);
    }
}
