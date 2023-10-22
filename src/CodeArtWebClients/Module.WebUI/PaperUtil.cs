using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using CodeArt.DTO;
using CodeArt;
using CodeArt.Util;


namespace Module.WebUI
{
    /// <summary>
    /// 
    /// </summary>
    public static class PaperUtil
    {
        /// <summary>
        /// 将代码转为可以存储的通用格式的html代码
        /// </summary>
        /// <param name="html"></param>
        /// <param name="imageHost"></param>
        /// <returns></returns>
        public static (DTObject Content, IEnumerable<string> FileIds,int PageCount) FormatInput(DTObject content)
        {
            if (content == null) return (null, null, 1);

            List<string> fileIds = new List<string>();
            content.Each("items", (item) =>
            {
                var type = item.GetValue<string>("type");
                switch (type)
                {
                    case "p":
                        {
                            var code = item.GetValue<string>("value.content");
                            var result = TinymceUtil.FormatInput(code);
                            fileIds.AddRange(result.FileIds);
                            item.SetValue("value.content", result.Content);
                            break;
                        }
                }
            });
            int pageCount = content.GetValue<int>("pageCount", 1);
            return (content, fileIds, pageCount);
        }

        /// <summary>
        /// 将格式化的内容转换为可以在页面上呈现的html代码
        /// </summary>
        /// <returns></returns>
        public static DTObject FormatOutput(DTObject content)
        {
            List<string> fileIds = new List<string>();
            content.Each("items", (item) =>
            {
                var type = item.GetValue<string>("type");
                switch (type)
                {
                    case "p":
                        {
                            var code = item.GetValue<string>("value.content");
                            var result = TinymceUtil.FormatOutput(code);
                            item.SetValue("value.content", result);
                            break;
                        }
                }
            });
            return content;
        }
    }
}
