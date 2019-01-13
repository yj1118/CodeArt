using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.DTO;

namespace CodeArt.DomainDriven
{
    public static class DTObjectPro
    {
        /// <summary>
        /// 创建翻页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rowSchemaCode"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static DTObject Create<T>(string rowSchemaCode, Page<T> page)
        {
            var result = DTObject.Create();
            result.SetValue("dataCount", page.DataCount);
            result.SetValue("pageCount", page.GetPageCount());
            result.SetValue("pageIndex", page.PageIndex);
            result.SetValue("pageSize", page.PageSize);
            var objs = page.Objects;
            result.Push("rows", objs, (obj) =>
            {
                return DTObject.Create(rowSchemaCode, obj);
            });
            return result;
        }

        /// <summary>
        /// 创建列表数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rowSchemaCode"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static DTObject Create<T>(string rowSchemaCode, IEnumerable<T> objs)
        {
            var result = DTObject.Create();
            result.SetValue("dataCount", objs.Count());
            result.Push("rows", objs, (obj) =>
            {
                return DTObject.Create(rowSchemaCode, obj);
            });
            return result;
        }

        public static DTObject Create<T>(IEnumerable<T> objs, Func<T, DTObject> process)
        {
            var result = DTObject.Create();
            result.SetValue("dataCount", objs.Count());
            result.Push("rows", objs, (obj) =>
            {
                return process(obj);
            });
            return result;
        }

    }


}
