using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    public struct Page<T>
    {
        public int PageIndex
        {
            get;
            private set;
        }

        public int PageSize
        {
            get;
            private set;
        }

        /// <summary>
        /// 数据总数
        /// </summary>
        public int DataCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 对象
        /// </summary>
        public IEnumerable<T> Objects
        {
            get;
            private set;
        }

        public int GetPageCount()
        {
            if (this.DataCount == 0 || this.PageSize == 0) return 0;
            int count = (int)(this.DataCount / this.PageSize);
            if (this.DataCount % this.PageSize > 0) count++;
            return count;
        }


        public Page(int pageIndex, int pageSize, IEnumerable<T> objects, int dataCount)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.Objects = objects;
            this.DataCount = dataCount;
        }
    }
}
