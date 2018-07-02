using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.DTO
{
    /// <summary>
    /// 
    /// </summary>
    internal class RemoveExpression : TransformExpression
    {
        private string[] _findExps;

        public RemoveExpression(string exp)
        {
            _findExps = exp.Substring(1).Split(',').Select((temp) =>
            {
                return temp.Trim();
            }).ToArray();
        }

        public override void Execute(DTObject dto)
        {
            foreach (var findExp in _findExps)
            {
                RemoveEntities(dto, findExp);
            }
        }

        private void RemoveEntities(DTObject dto, string findExp)
        {
            var targets = dto.FindEntities(findExp, false);
            foreach (var target in targets)
            {
                var parent = target.Parent;
                if (parent == null) throw new DTOException("预期之外的错误，" + findExp);
                parent.DeletEntity(target);
            }
        }
    }
}
