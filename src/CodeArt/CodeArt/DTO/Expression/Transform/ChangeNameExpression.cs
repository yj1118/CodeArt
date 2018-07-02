using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.DTO
{
    /// <summary>
    /// 变换表达式
    /// </summary>
    internal class ChangeNameExpression : TransformExpression
    {
        private string _findExp;
        private string _name;

        public ChangeNameExpression(string exp)
        {
            int index = exp.IndexOf("=>");  //转换成员名称
            if (index > 0)
            {
                _findExp = exp.Substring(0, index).Trim();
                _name = exp.Substring(index + 2).Trim();
            }

            ArgumentAssert.IsNotNullOrEmpty(_findExp, "findExp");
            ArgumentAssert.IsNotNullOrEmpty(_name, "name");
        }

        public override void Execute(DTObject dto)
        {
            var entities = dto.FindEntities(_findExp, false);
            foreach (var e in entities)
            {
                e.Name = _name;
            }
            //dto.OrderEntities();
        }
    }
}
