using System;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace MenuSubsystem
{
    public sealed class CreateGroupMenu : CreateMenuBase
    {
        public CreateGroupMenu(string name, int orderIndex, Guid parentId, string markedCode)
            : base(name, orderIndex, parentId, markedCode)
        {
        }
        protected override MenuBehavior CreateBehavior()
        {
            GroupMB mb = new GroupMB();
            return mb;
        }

    }
}
