using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven.DataAccess
{
    public interface IDocument : CodeArt.Search.IDocument
    {
        /// <summary>
        /// 给系统使用
        /// </summary>
        string OperationType
        {
            get;
            set;
        }
    }
}
