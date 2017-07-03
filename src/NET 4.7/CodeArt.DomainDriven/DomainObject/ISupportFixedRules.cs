using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 每个领域对象具有验证固定规则的能力
    /// </summary>
    public interface ISupportFixedRules
    {
        /// <summary>
        /// 验证对象是否满足固定规则
        /// </summary>
        /// <returns></returns>
        ValidationResult Validate();
    }
}
