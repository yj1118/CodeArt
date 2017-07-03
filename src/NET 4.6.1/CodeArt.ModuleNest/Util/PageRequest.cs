using System;
using CodeArt.DTO;

namespace CodeArt.ModuleNest
{
    [DTOClass()]
    public class PageRequest
    {
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

        public void Fill(DTObject dto)
        {
            dto.SetValue("pageIndex", this.PageIndex);
            dto.SetValue("pageSize", this.PageSize);
        }

    }
}
