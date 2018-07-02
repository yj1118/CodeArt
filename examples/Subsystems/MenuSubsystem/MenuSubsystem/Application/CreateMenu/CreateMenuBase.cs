using System;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace MenuSubsystem
{
    public abstract class CreateMenuBase : Command<Menu>
    {
        private string _name;
        private int _orderIndex;
        private Guid _parentId;
        private string _markedCode;

        public CreateMenuBase(string name, int orderIndex, Guid parentId, string markedCode)
        {
            _name = name;
            _orderIndex = orderIndex;
            _parentId = parentId;
            _markedCode = markedCode;
        }

        protected override Menu ExecuteProcedure()
        {
            var behavior = CreateBehavior();
            return MenuService.Create(_name, _markedCode, _orderIndex, _parentId, behavior);
        }

        protected abstract MenuBehavior CreateBehavior();
    }
}
