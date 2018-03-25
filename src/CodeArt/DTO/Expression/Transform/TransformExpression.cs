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
    internal abstract class TransformExpression
    {

        public abstract void Execute(DTObject target);

        #region 静态成员

        private static Func<string, TransformExpression> _getExpression = LazyIndexer.Init<string, TransformExpression>((transformString) =>
        {
            int index = transformString.IndexOf("=>");  //转换成员名称
            if (index > 0)
            {
                return new ChangeNameExpression(transformString);
            }

            index = transformString.IndexOf("=");  //赋值
            if (index > 0)
            {
                return new AssignExpression(transformString);
            }

            index = transformString.IndexOf("!");  //移除表达式对应的成员
            if (index == 0)
            {
                return new RemoveExpression(transformString);
            }

            index = transformString.IndexOf("~");  //保留表达式对应的成员，其余的均移除
            if (index == 0)
            {
                return new RetainExpression(transformString);
            }

            throw new DTOException(string.Format("{0}{1}", Strings.TransformExpressionError, transformString));
        });

        public static TransformExpression Create(string transformString)
        {
            return _getExpression(transformString);
        }

        #endregion

    }


}
