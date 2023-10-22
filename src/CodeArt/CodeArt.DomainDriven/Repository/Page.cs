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

        public Page<S> Select<S>(Func<T, S> selector)
        {
            return new Page<S>(this.PageIndex, this.PageSize, this.Objects.Select(selector), this.DataCount);
        }

        public Page(int pageIndex, int pageSize, IEnumerable<T> objects, int dataCount)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.Objects = objects;
            this.DataCount = dataCount;
        }

        /// <summary>
        /// 根据页信息计算出翻页对象
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static Page<T> Calculate(int pageIndex, int pageSize, IEnumerable<T> objects)
        {
            int dataCount = objects.Count();

            var start = (pageIndex - 1) * pageSize;
            if (start >= dataCount)
            {
                return new Page<T>(pageIndex, pageSize, Array.Empty<T>(), dataCount);
            }

            var end = start + pageSize - 1;
            if (end >= dataCount) end = dataCount - 1;

            List<T> items = new List<T>(end - start + 1);

            for (var i = start; i <= end; i++)
            {
                items.Add(objects.ElementAt(i));
            }
            return new Page<T>(pageIndex, pageSize, items, dataCount);
        }

    }
}
