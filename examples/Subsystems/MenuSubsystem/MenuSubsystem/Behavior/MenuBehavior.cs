using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;
using AccountSubsystem;

namespace MenuSubsystem
{
    [ObjectRepository(typeof(IMenuRepository))]
    public abstract class MenuBehavior : ValueObject, IMenuBehavior
    {
        public MenuBehavior()
        {
        }


        public abstract string GetCode();

        public abstract IMenuBehavior Clone();

        #region 空对象

        private class MenuBehaviorEmpty : MenuBehavior
        {
            public MenuBehaviorEmpty()
            {
                this.OnConstructed();
            }

            public override IMenuBehavior Clone()
            {
                return MenuBehavior.Empty;
            }

            public override string GetCode()
            {
                return string.Empty;
            }


            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly MenuBehavior Empty = new MenuBehaviorEmpty();

        #endregion



        ///// <summary>
        ///// 账号<paramref name="account"/>可以访问的权限中，是否有该菜单下的功能
        ///// </summary>
        ///// <param name="account"></param>
        ///// <returns></returns>
        //private bool IsPermissionOverlap(Account account)
        //{
        //    var items = this.Menu.Items;
        //    if (items.Count == 0) return true;
        //    foreach (var item in items)
        //    {
        //        if (account.InPermission(item)) return true;
        //    }
        //    return false;
        //}

        //public bool CanRender(Account account, IMenuRender render)
        //{
        //    return CanRenderByPermission(account, render) && render.CanRender(this.Menu, account);
        //}

        //private bool CanRenderByPermission(Account account, IMenuRender render)
        //{
        //    //如果菜单没有设置相关功能的绑定，那么需要查看子菜单的权限来决定是否显示本菜单
        //    if (IsNonePermission()) return ForwardCheckRender(account, render);
        //    return IsPermissionOverlap(account); //如果菜单设置了相关功能的绑定，那么使用当前菜单的绑定
        //}

        //protected abstract bool ForwardCheckRender(Account account, IMenuRender render);


    }
}
