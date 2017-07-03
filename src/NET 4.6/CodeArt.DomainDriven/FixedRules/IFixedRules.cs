using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 固定规则，每个领域对象都会有一组固定规则
    /// </summary>
    public interface IFixedRules
    {
        ValidationResult Validate(IDomainObject obj);
    }
}