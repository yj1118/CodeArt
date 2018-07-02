using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using CodeArt;
using CodeArt.DomainDriven;
using AccountSubsystem;

namespace MenuSubsystem
{
    [DerivedClass(typeof(GroupMB), "{0B9D2380-F7B2-4EC6-A559-ACEA29332917}")]
    [ObjectRepository(typeof(IMenuRepository))]
    public class GroupMB : MenuBehavior
    {
        [ConstructorRepository]
        public GroupMB()
        {
            this.OnConstructed();
        }


        public override string GetCode()
        {
            return string.Empty;
        }

        public override IMenuBehavior Clone()
        {
            return new GroupMB();
        }


        #region 空对象

        private class GroupMBEmpty : GroupMB
        {
            public GroupMBEmpty()
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public new static readonly GroupMB Empty = new GroupMBEmpty();

        #endregion


        //protected override bool ForwardCheckRender(Account account, IMenuRender render)
        //{
        //    foreach (var child in this.Menu.Childs)
        //    {
        //        if (child.CanRender(account, render)) return true;
        //    }
        //    return false;
        //}
    }
}
