using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;
using AccountSubsystem;
using TreeSubsystem;

namespace MenuSubsystem
{
    internal static class MenuService
    {
        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="name"></param>
        /// <param name="markedCode"></param>
        /// <param name="orderIndex"></param>
        /// <param name="parentId"></param>
        /// <param name="behavior"></param>
        public static Menu Create(string name,
                                    string markedCode,
                                    int orderIndex,
                                    Guid parentId,
                                    MenuBehavior behavior)
        {
            Menu menu = new Menu(Guid.NewGuid());
            menu.Name = name;
            menu.Behavior = behavior;

            var node = TreeService.CreateNode(TreeGroupName, menu, parentId, orderIndex);

            IMenuRepository repository = Repository.Create<IMenuRepository>();
            repository.Add(menu);

            return menu;
        }

        private const string TreeGroupName = "menu";
    }
}
