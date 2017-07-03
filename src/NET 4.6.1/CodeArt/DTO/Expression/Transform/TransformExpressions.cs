using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using CodeArt.Util;

namespace CodeArt.DTO
{
    /// <summary>
    /// 变换表达式
    /// </summary>
    internal class TransformExpressions : IEnumerable<TransformExpression>
    {
        private List<TransformExpression> _expressions;

        private TransformExpressions(string transformString)
        {
            var itemCodes = transformString.Split(';').Select((temp) =>
            {
                return temp.Trim();
            }).Where((temp) =>
            {
                return !string.IsNullOrEmpty(temp);
            }).ToList();
            _expressions = new List<TransformExpression>(itemCodes.Count);
            for (var i = 0; i < itemCodes.Count; i++)
            {
                var itemCode = itemCodes[i];
                _expressions.Add(TransformExpression.Create(itemCode));
            }
        }

        public IEnumerator<TransformExpression> GetEnumerator()
        {
            return _expressions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _expressions.GetEnumerator();
        }

        #region 静态成员

        private static Func<string, TransformExpressions> _getExpression = LazyIndexer.Init<string, TransformExpressions>((transformString) =>
        {
            return new TransformExpressions(transformString);
        });

        public static TransformExpressions Create(string transformString)
        {
            return _getExpression(transformString);
        }

        #endregion

    }
}
