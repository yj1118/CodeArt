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

namespace CodeArt.ServiceModel
{
    [SafeAccess]
    internal class HttpCommunicator
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

        private HttpWebRequest CreateRequest(string address)
        {
            var request = WebRequest.Create(address) as HttpWebRequest; //这个对象无法重用，所以没有用池机制
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            request.Timeout = 30000; //30秒超时
            request.KeepAlive = false;
            return request;
        }

        private void ClearRequest(HttpWebRequest request)
        {
            if(request != null)
                request.Abort();
        }

        public ServiceResponse Send(string address, ByteArray requestData)
        {
            HttpWebRequest request = null;
            ServiceResponse response;
            try
            {
                request = CreateRequest(address);
                request.ContentLength = requestData.Length; //post方法需要手工指定

                //发送POST数据  
                using (Stream stream = request.GetRequestStream())
                {
                    requestData.Read(stream);
                }

                using (var httpResponse = request.GetResponse() as HttpWebResponse)
                {
                    int responseDataLength = (int)httpResponse.ContentLength;
                    using (Stream stream = httpResponse.GetResponseStream())
                    {
                        using (var responseDataItem = ByteBuffer.Borrow(responseDataLength))
                        {
                            var responseData = responseDataItem.Item;
                            responseData.Write(stream, responseDataLength);
                            response = DataAnalyzer.DeserializeResponse(responseData);
                        }
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new ServiceException("访问服务地址" + address + "出错", ex);
            }
            finally
            {
                ClearRequest(request);
            }
        }

        #region 异步访问

        public async Task<ServiceResponse> SendAsync(string address, ByteArray requestData)
        {
            HttpWebRequest request = null;
            ServiceResponse response;
            try
            {
                request = CreateRequest(address);
                request.ContentLength = requestData.Length; //post方法需要手工指定

                //发送POST数据  
                using (Stream stream = await request.GetRequestStreamAsync())
                {
                    requestData.Read(stream);
                }

                using (var httpResponse = await request.GetResponseAsync() as HttpWebResponse)
                {
                    int responseDataLength = (int)httpResponse.ContentLength;
                    using (Stream stream = httpResponse.GetResponseStream())
                    {
                        using (var responseDataItem = ByteBuffer.Borrow(responseDataLength))
                        {
                            var responseData = responseDataItem.Item;
                            responseData.Write(stream, responseDataLength);
                            response = DataAnalyzer.DeserializeResponse(responseData);
                        }
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new ServiceException("访问服务地址" + address + "出错", ex);
            }
            finally
            {
                ClearRequest(request);
            }
        }

        #endregion

        public static readonly HttpCommunicator Instance = new HttpCommunicator();

    }
}
