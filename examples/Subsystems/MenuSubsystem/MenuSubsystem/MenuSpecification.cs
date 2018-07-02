using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CodeArt;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace MenuSubsystem
{
    /// <summary>
    /// 该验证器不是线程安全的
    /// </summary>
    [SafeAccess]
    internal sealed class MenuSpecification : ObjectValidator<Menu>
    {
        public MenuSpecification()
        {

        }

        protected override void Validate(Menu obj, ValidationResult result)
        {
            Validator.CheckPropertyRepeated(obj, Menu.MarkedCodeProperty, result);
        }
    }
}
