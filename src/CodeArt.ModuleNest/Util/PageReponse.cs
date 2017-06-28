using System;
using CodeArt.DTO;

namespace CodeArt.ModuleNest
{
    [DTOClass()]
    public class PageReponse<T>
    {
        [DTOMember("dataCount")]
        public int DataCount
        {
            get;
            set;
        }

        [DTOMember("pageCount")]
        public int PageCount
        {
            get;
            set;
        }

        [DTOMember("pageIndex")]
        public int PageIndex
        {
            get;
            set;
        }

        [DTOMember("pageSize")]
        public int PageSize
        {
            get;
            set;
        }

        [DTOMember("rows")]
        public T[] Rows
        {
            get;
            set;
        }


    }
}
