using System;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Text;
using System.Net;

using CodeArt;
using CodeArt.IO;
using CodeArt.Runtime;
using CodeArt.Concurrent;

namespace CodeArt.Net
{
    [SafeAccess]
    public class HttpCommunicator
    {
        static HttpCommunicator()
        {
            //在Http协议中，规定了同个Http请求的并发连接数最大为2. 这个数值，可谓是太小了。
            //而目前的浏览器，已基本不再遵循这个限制，但是Dot Net平台上的 System.Net 还是默认遵循了这个标准的。
            //从而造成了，在使用HttpWebRequset 或者 WebClient 利用多线程的方式，访问某个网站时，经常出现 连接被异常关闭 的错误，大大降低了效率。
            //这个限制的值，是可以自己设置或配置的。建议不要超过1024，推荐为512，已经足够了。
            //此值设置后，只对以后发起的HTTP请求有效。
            ServicePointManager.DefaultConnectionLimit = 512;
        }

        private HttpCommunicator()
        {
        }

        private HttpWebRequest CreateGetRequest(string address)
        {
            var request = WebRequest.Create(address) as HttpWebRequest; //这个对象无法重用，所以没有用池机制
            request.Method = "GET";
            request.Timeout = 30000; //30秒超时
            request.KeepAlive = false;
            return request;
        }

        private void ClearRequest(HttpWebRequest request)
        {
            if(request != null)
                request.Abort();
        }

        public byte[] Get(string address)
        {
            HttpWebRequest request = null;
            byte[] data = null;
            try
            {
                request = CreateGetRequest(address);

                using (var httpResponse = request.GetResponse() as HttpWebResponse)
                {
                    using (Stream stream = httpResponse.GetResponseStream())
                    {
                        data = stream.GetBytes();
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("访问地址" + address + "出错", ex);
            }
            finally
            {
                ClearRequest(request);
            }
        }


        public static readonly HttpCommunicator Instance = new HttpCommunicator();

    }
}
