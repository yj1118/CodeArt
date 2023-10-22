using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Nest;

namespace CodeArt.DomainDriven.DataAccess
{
    public abstract class Document : IDocument
    {
        [Nest.Keyword(Name = "id", Store = true)]
        public Id Id { get; set; }

        /// <summary>
        /// 系统保留
        /// </summary>
        [Nest.Text(Index = false, Store = true)]
        public string OperationType
        {
            get;
            set;
        }
    }
}
