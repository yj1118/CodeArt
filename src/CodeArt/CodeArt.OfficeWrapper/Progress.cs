using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Office
{
    /// <summary>
    /// 进度
    /// </summary>
    public struct Progress
    {
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 已完成的页数
        /// </summary>
        public int CompletedCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取进度的百分比
        /// </summary>
        /// <returns></returns>
        public float GetPercentage()
        {
            return (float)Math.Round(((double)CompletedCount / (double)PageCount), 4);
        }

        public bool IsCompleted
        {
            get
            {
                return !this.IsEmpty && this.CompletedCount == this.PageCount;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return this.PageCount == 0;
            }
        }


        public Progress(int pageCount, int completedCount)
        {
            this.PageCount = pageCount;
            this.CompletedCount = completedCount;
        }
    }
}
