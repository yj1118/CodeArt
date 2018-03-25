using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using CodeArt.IO;
using CodeArt.DTO;
using CodeArt.Concurrent;

using CodeArt.Net.Anycast;

namespace CodeArt.Net.Anycast
{
    internal class TransferCommand
    {
        public const string Save = "transfer save";

        public const string Delete = "transfer delete";

        /// <summary>
        /// 取消保存文件的操作
        /// </summary>
        public const string CancelSave = "transfer canel save";

        public const string SaveResult = "transfer save result";


        public const string DeleteResult = "transfer delete result";

        public const string Load = "transfer load";

        public const string LoadResult = "transfer load result";


        public const string Size = "transfer size";
        public const string SizeResult = "transfer size result";

        public const string Error = "transfer save error";
    }
}
