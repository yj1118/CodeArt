using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Compilation;

using System.Web;


namespace CodeArt.Web.WebPages
{
   internal sealed class HttpRequestRange
   {
       public long StartIndex { get; private set; }
       public long EndIndex { get; private set; }

       public int LastBytes { get; private set; }

       public bool IsEmpty()
       {
           return this.StartIndex == 0 && this.EndIndex == 0 && this.LastBytes == 0;
       }

       private HttpRequestRange()
       {
           this.StartIndex = this.EndIndex = 0;
       }


       public void Process(HttpResponse response, long fileSize,out long offset,out long count)
       {
            if (this.IsEmpty())
            {
                response.AddHeader("Content-Length", fileSize.ToString());
                offset = 0;
                count = fileSize;
            }
            else if (this.LastBytes > 0)
            {
                count = this.LastBytes > fileSize ? fileSize : this.LastBytes;
                long startPosition = fileSize - count;
                long endPosition = fileSize - 1;

                response.AddHeader("Content-Length", count.ToString());
                response.AddHeader("Content-Range", string.Format("bytes {0}-{1}/{2}", startPosition, endPosition, fileSize));

                offset = startPosition;
                response.StatusCode = 206;
            }
            else
            {
                long startPosition = this.StartIndex;
                long endPosition = this.EndIndex == -1 ? fileSize - 1 : this.EndIndex;

                count = endPosition - startPosition + 1;
                response.AddHeader("Content-Length", count.ToString());
                response.AddHeader("Content-Range", string.Format("bytes {0}-{1}/{2}", startPosition, endPosition, fileSize));

                offset = this.StartIndex;
                response.StatusCode = 206;
            }
       }


       public static HttpRequestRange Empty = new HttpRequestRange();

       public static HttpRequestRange New(HttpRequest request)
       {
           var rangeStr = request.Headers["Range"];
           if (string.IsNullOrEmpty(rangeStr)) return HttpRequestRange.Empty;
           rangeStr = rangeStr.Replace("bytes=", string.Empty);
           if (string.IsNullOrEmpty(rangeStr)) return HttpRequestRange.Empty;
           HttpRequestRange range = new HttpRequestRange();
           if (rangeStr.IndexOf('-') == -1)
           {
               //Range bytes=1000 传输最后1000个字节。
               range.LastBytes = int.Parse(rangeStr);
           }
           else
           {
               //bytes=0- 或者 bytes=1000-2000
               var temp = rangeStr.Split('-');
               range.StartIndex = long.Parse(temp[0]);
               range.EndIndex =(temp.Length > 1 && !string.IsNullOrEmpty(temp[1])) ? long.Parse(temp[1]) : -1;
           }
           return range;
       }


   }
}
