using System;
using CodeArt.DTO;

namespace CodeArt.ModuleNest
{
    [DTOClass()]
    public class ListReponse<T>
    {
        [DTOMember("dataCount")]
        public int DataCount
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
