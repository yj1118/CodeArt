using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.Util;

using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Push.Model.V20160801;
using System.Configuration;

namespace CodeArt.MessagePush
{
    internal static class AliPush
    {
        private static DefaultAcsClient Client()
        {
            IClientProfile clientProfile = DefaultProfile.GetProfile("cn-hangzhou", AliUtil.GetAccessKeyId(), AliUtil.GetAccessKeySecret());
            return new DefaultAcsClient(clientProfile);
        }

        /// <summary>
        /// 绑定标签
        /// </summary>
        /// <param name="keyType">设备：DEVICE 账号：ACCOUNT 别名：ALIAS</param>
        /// <param name="deviceIds">设备Id，可为多个逗号分割</param>
        /// <param name="tags">标签，可为多个逗号分割</param>
        /// <returns>请求Id</returns>
        public static string BindTag(long appKey, string keyType, string deviceIds, string tags)
        {
            var client = Client();
            BindTagRequest request = new BindTagRequest();
            request.AppKey = appKey;
            request.KeyType = keyType;
            request.ClientKey = deviceIds;
            request.TagName = tags;

            try
            {
                BindTagResponse response = client.GetAcsResponse(request);
                return response.RequestId;
            }
            catch (ServerException e)
            {
                ServerException(e);
            }
            catch (ClientException e)
            {
                ClientException(e);
            }

            return string.Empty;
        }

        /// <summary>
        /// 解绑标签
        /// </summary>
        /// <param name="keyType">设备：DEVICE 账号：ACCOUNT 别名：ALIAS</param>
        /// <param name="deviceIds">设备Id，可为多个逗号分割</param>
        /// <param name="tags">标签，可为多个逗号分割</param>
        /// <returns></returns>
        public static string UnbindTag(long appKey, string keyType, string deviceIds, string tags)
        {
            var client = Client();
            UnbindTagRequest request = new UnbindTagRequest();
            request.AppKey = appKey;
            request.KeyType = keyType;
            request.ClientKey = deviceIds;
            request.TagName = tags;

            try
            {
                UnbindTagResponse response = client.GetAcsResponse(request);
                return response.RequestId;
            }
            catch (ServerException e)
            {
                ServerException(e);
            }
            catch (ClientException e)
            {
                ClientException(e);
            }

            return string.Empty;
        }

        /// <summary>
        /// 取消推送
        /// </summary>
        /// <param name="messageId">消息Id</param>
        /// <returns>请求Id</returns>
        public static string CancelPush(long appKey, long? messageId)
        {
            var client = Client();
            CancelPushRequest request = new CancelPushRequest();
            request.AppKey = appKey;
            request.MessageId = messageId;
            try
            {
                CancelPushResponse response = client.GetAcsResponse(request);
                return response.RequestId;
            }
            catch (ServerException e)
            {
                ServerException(e);
            }
            catch (ClientException e)
            {
                ClientException(e);
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取所有标签
        /// </summary>
        public static IEnumerable<string> ListTags(long appKey)
        {
            var client = Client();
            ListTagsRequest request = new ListTagsRequest();
            request.AppKey = appKey;

            try
            {
                ListTagsResponse response = client.GetAcsResponse(request);
                //Console.WriteLine("RequestId:" + response.RequestId);

                using (var temp = ListPool<string>.Borrow())
                {
                    var list = temp.Item;
                    foreach (var info in response.TagInfos)
                    {
                        list.Add(info.TagName);
                    }

                    return list;
                }

            }
            catch (ServerException e)
            {
                ServerException(e);
            }
            catch (ClientException e)
            {
                ClientException(e);
            }

            return null;
        }


        ///// <summary>
        ///// 推送精简版
        ///// </summary>
        ///// <param name="target">推送目标: DEVICE:按设备推送 ALIAS : 按别名推送 ACCOUNT:按帐号推送  TAG:按标签推送; ALL: 广播推送</param>
        ///// <param name="body">内容</param>
        ///// <param name="targetValue">根据Target来设定，如Target=DEVICE, 则对应的值为 设备id1,设备id2. 多个值使用逗号分隔.(帐号与设备有一次最多100个的限制)</param>
        ///// <param name="pushType">消息类型 MESSAGE NOTICE</param>
        ///// <param name="deviceType">设备类型 ANDROID iOS ALL</param>
        ///// <param name="title">标题</param>
        ///// <returns></returns>
        //public static string Push( string body, string target ="ALL", string targetValue = "All", string pushType = "MESSAGE", string deviceType = "ALL",  string title = "Message")
        //{
        //    var client = Client();
        //    PushRequest request = new PushRequest();

        //    request.AppKey = AliUtil.GetAppKey();
        //    //推送目标: DEVICE:按设备推送 ALIAS : 按别名推送 ACCOUNT:按帐号推送  TAG:按标签推送; ALL: 广播推送
        //    request.Target = target;
        //    //根据Target来设定，如Target=DEVICE, 则对应的值为 设备id1,设备id2. 多个值使用逗号分隔.(帐号与设备有一次最多100个的限制)
        //    request.TargetValue = targetValue;
        //    //消息类型 MESSAGE NOTICE
        //    request.PushType = pushType;
        //    //设备类型 ANDROID iOS ALL.
        //    request.DeviceType = deviceType;
        //    // 消息的标题
        //    request.Title = title;
        //    // 消息的内容
        //    request.Body = body;

        //    try
        //    {
        //        PushResponse response = client.GetAcsResponse(request);
        //        return response.MessageId;
        //    }
        //    catch (ServerException e)
        //    {
        //        ServerException(e);
        //    }
        //    catch (ClientException e)
        //    {
        //        ClientException(e);
        //    }

        //    return string.Empty;
        //}


        /// <summary>
        /// 推送安卓消息
        /// </summary>
        /// <param name="target">推送目标: DEVICE:按设备推送 ALIAS : 按别名推送 ACCOUNT:按帐号推送  TAG:按标签推送; ALL: 广播推送</param>
        /// <param name="title">标题</param>
        /// <param name="body">内容</param>
        /// <param name="targetValue">根据Target来设定，如Target=DEVICE, 则对应的值为 设备id1,设备id2. 多个值使用逗号分隔.(帐号与设备一次最多100)</param>
        /// <returns>消息Id</returns>
        public static string PushMessageToAndroid(string body, string target = "ALL", string targetValue = "All", string title = "Message")
        {
            var client = Client();
            PushMessageToAndroidRequest request = new PushMessageToAndroidRequest();
            request.AppKey = AliUtil.GetAndroidAppKey();
            request.Target = target;
            request.TargetValue = targetValue;
            request.Title = title;
            request.Body = body;

            try
            {
                PushMessageToAndroidResponse response = client.GetAcsResponse(request);
                return response.MessageId;
            }
            catch (ServerException e)
            {
                ServerException(e);
            }
            catch (ClientException e)
            {
                ClientException(e);
            }

            return string.Empty;
        }

        /// <summary>
        /// 推送苹果消息
        /// </summary>
        /// <param name="target">推送目标: DEVICE:按设备推送 ALIAS : 按别名推送 ACCOUNT:按帐号推送  TAG:按标签推送; ALL: 广播推送</param>
        /// <param name="title">标题</param>
        /// <param name="body">内容</param>
        /// <param name="targetValue">根据Target来设定，如Target=DEVICE, 则对应的值为 设备id1,设备id2. 多个值使用逗号分隔.(帐号与设备一次最多100)</param>
        /// <returns>消息Id</returns>
        public static string PushMessageToIOS(string body, string target = "ALL", string targetValue = "All", string title = "Message")
        {
            var client = Client();
            PushMessageToiOSRequest request = new PushMessageToiOSRequest();
            request.AppKey = AliUtil.GetIOSAppKey();
            request.Target = target;
            request.TargetValue = targetValue;
            request.Title = title;
            request.Body = body;
            try
            {
                PushMessageToiOSResponse response = client.GetAcsResponse(request);
                return response.MessageId;
            }
            catch (ServerException e)
            {
                ServerException(e);
            }
            catch (ClientException e)
            {
                ClientException(e);
            }

            return string.Empty;
        }



        public static void PushMessage(string body, string target = "ALL", string targetValue = "All", string title = "Message")
        {
            PushMessageToAndroid(body, target, targetValue, title);
            PushMessageToIOS(body, target, targetValue, title);
        }

        ///// <summary>
        ///// 推送安卓通知
        ///// </summary>
        ///// <param name="target">推送目标: DEVICE:按设备推送 ALIAS : 按别名推送 ACCOUNT:按帐号推送  TAG:按标签推送; ALL: 广播推送</param>
        ///// <param name="title">标题</param>
        ///// <param name="body">内容</param>
        ///// <param name="targetValue">根据Target来设定，如Target=DEVICE, 则对应的值为 设备id1,设备id2. 多个值使用逗号分隔.(帐号与设备一次最多100个)</param>
        ///// <param name="extParameters">额外参数</param>
        ///// <returns>消息Id</returns>
        //public static string PushNoticeToAndroid(string title, string body, string target = "ALL", string targetValue = "All", string extParameters = "{\"api_name\":\"PushNoticeToAndroidRequest\"}")
        //{
        //    var client = Client();
        //    PushNoticeToAndroidRequest request = new PushNoticeToAndroidRequest();
        //    request.AppKey = AliUtil.GetAndroidAppKey();
        //    request.Target = target;
        //    request.TargetValue = targetValue;
        //    request.Title = title;
        //    request.Body = body;
        //    request.ExtParameters = extParameters;
        //    try
        //    {
        //        PushNoticeToAndroidResponse response = client.GetAcsResponse(request);
        //        return response.MessageId;
        //    }
        //    catch (ServerException e)
        //    {
        //        ServerException(e);
        //    }
        //    catch (ClientException e)
        //    {
        //        ClientException(e);
        //    }

        //    return string.Empty;
        //}

        ///// <summary>
        ///// 推送苹果通知
        ///// </summary>
        ///// <param name="target">推送目标: DEVICE:按设备推送 ALIAS : 按别名推送 ACCOUNT:按帐号推送  TAG:按标签推送; ALL: 广播推送</param>
        ///// <param name="title">标题</param>
        ///// <param name="body">内容</param>
        ///// <param name="targetValue">根据Target来设定，如Target=DEVICE, 则对应的值为 设备id1,设备id2. 多个值使用逗号分隔.(帐号与设备一次最多100个)</param>
        ///// <param name="extParameters">额外参数</param>
        ///// <returns>消息Id</returns>
        //public static string PushNoticeToIOS(string title, string body, string target = "ALL", string targetValue = "all", string extParameters = "{\"api_name\":\"PushNoticeToAndroidRequest\"}")
        //{
        //    var client = Client();
        //    PushNoticeToiOSRequest request = new PushNoticeToiOSRequest();
        //    request.AppKey = AliUtil.GetIOSAppKey();
        //    request.Target = target;//推送目标: DEVICE:按设备推送 ALIAS : 按别名推送 ACCOUNT:按帐号推送  TAG:按标签推送; ALL: 广播推送
        //    request.TargetValue = targetValue;//根据Target来设定，如Target=DEVICE, 则对应的值为 设备id1,设备id2. 多个值使用逗号分隔.(帐号与设备一次最多100个)
        //    request.Title = title;
        //    request.Body = body;
        //    request.ExtParameters = extParameters;
        //    try
        //    {
        //        PushNoticeToiOSResponse response = client.GetAcsResponse(request);
        //        return response.MessageId;
        //    }
        //    catch (ServerException e)
        //    {
        //        ServerException(e);
        //    }
        //    catch (ClientException e)
        //    {
        //        ClientException(e);
        //    }

        //    return string.Empty;
        //}

        /// <summary>
        /// 查询设备统计
        /// </summary>
        /// <param name="queryType">NEW: 新增设备查询, TOTAL: 留存设备查询</param>
        /// <param name="deviceType">iOS,ANDROID,ALL</param>
        /// <param name="start">为负值，查询近几天的数据</param>
        /// <returns></returns>
        public static IEnumerable<(string Time, string DeviceType, long? Count)> QueryDeviceStat(long appKey, string queryType, string deviceType, int start)
        {
            var client = Client();
            QueryDeviceStatRequest request = new QueryDeviceStatRequest();
            request.AppKey = appKey;

            request.QueryType = queryType;////NEW: 新增设备查询, TOTAL: 留存设备查询
            request.DeviceType = "ALL";//iOS,ANDROID,ALL
            String startTime = DateTime.UtcNow.AddDays(start).ToString("yyyy-MM-ddTHH\\:mm\\:ssZ");//查询近几天的数据
            String endTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ssZ");
            request.StartTime = startTime;
            request.EndTime = endTime;

            try
            {
                QueryDeviceStatResponse response = client.GetAcsResponse(request);

                using (var temp = ListPool<(string Time, string DeviceType, long? Count)>.Borrow())
                {
                    var list = temp.Item;
                    foreach (var stat in response.AppDeviceStats)
                    {
                        list.Add((stat.Time, stat.DeviceType, stat.Count));
                    }

                    return list;
                }
            }
            catch (ServerException e)
            {
                ServerException(e);
            }
            catch (ClientException e)
            {
                ClientException(e);
            }

            return null;
        }

        /// <summary>
        /// 查询App推送统计
        /// </summary>
        /// <param name="start">为负值，查询近几天的数据</param>
        /// <param name="granularity">DAY: 天粒度 MONTH: 月粒度</param>
        /// <returns></returns>
        public static IEnumerable<(string Time, long? SentCount, long? ReceivedCount, long? OpenedCount, long? DeletedCount)> QueryPushStatByApp(long appKey, int start, string granularity = "DAY")
        {
            var client = Client();
            QueryPushStatByAppRequest request = new QueryPushStatByAppRequest();
            request.AppKey = appKey;

            request.Granularity = granularity; //DAY: 天粒度 MONTH: 月粒度
            String startTime = DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-ddTHH\\:mm\\:ssZ");//查询近期几天的数据
            String endTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ssZ");
            request.StartTime = startTime;
            request.EndTime = endTime;

            try
            {
                QueryPushStatByAppResponse response = client.GetAcsResponse(request);
                using (var temp = ListPool<(string Time, long? SentCount, long? ReceivedCount, long? OpenedCount, long? DeletedCount)>.Borrow())
                {
                    var list = temp.Item;

                    foreach (var stat in response.AppPushStats)
                    {
                        list.Add((stat.Time, stat.SentCount, stat.ReceivedCount, stat.OpenedCount, stat.DeletedCount));
                    }

                    return list;
                }

            }
            catch (ServerException e)
            {
                ServerException(e);
            }
            catch (ClientException e)
            {
                ClientException(e);
            }

            return null;
        }

        /// <summary>
        /// 查询消息推送统计
        /// </summary>
        /// <param name="messageId">消息Id</param>
        /// <returns></returns>
        public static IEnumerable<(string MessageId, long? SentCount, long? ReceivedCount, long? OpenedCount, long? DeletedCount)> QueryPushStatByMsg(long appKey, long? messageId)
        {

            var client = Client();
            QueryPushStatByMsgRequest request = new QueryPushStatByMsgRequest();
            request.AppKey = appKey;
            request.MessageId = messageId;

            try
            {
                QueryPushStatByMsgResponse response = client.GetAcsResponse(request);
                using (var temp = ListPool<(string MessageId, long? SentCount, long? ReceivedCount, long? OpenedCount, long? DeletedCount)>.Borrow())
                {
                    var list = temp.Item;
                    foreach (var stat in response.PushStats)
                    {
                        list.Add((stat.MessageId, stat.SentCount, stat.ReceivedCount, stat.OpenedCount, stat.DeletedCount));
                    }

                    return list;
                }
            }
            catch (ServerException e)
            {
                ServerException(e);
            }
            catch (ClientException e)
            {
                ClientException(e);
            }


            return null;
        }


        /// <summary>
        /// 查询标签
        /// </summary>
        /// <param name="keyType">设备：DEVICE 账号：ACCOUNT 别名：ALIAS</param>
        /// <param name="deviceId">设备Id</param>
        /// <returns></returns>
        public static IEnumerable<string> QueryTags(long appKey, string keyType, string deviceId)
        {
            var client = Client();
            QueryTagsRequest request = new QueryTagsRequest();
            request.AppKey = appKey;
            request.KeyType = keyType;
            request.ClientKey = deviceId;

            try
            {
                QueryTagsResponse response = client.GetAcsResponse(request);

                using (var temp = ListPool<string>.Borrow())
                {
                    var list = temp.Item;
                    foreach (var info in response.TagInfos)
                    {
                        list.Add(info.TagName);
                    }

                    return list;
                }
            }
            catch (ServerException e)
            {
                ServerException(e);
            }
            catch (ClientException e)
            {
                ClientException(e);
            }

            return null;
        }

        /// <summary>
        /// 查询特殊设备统计
        /// </summary>
        /// <param name="start">为负值，查询近几天的数据</param>
        /// <param name="granularity">DAY: 天粒度 MONTH: 月粒度</param>
        public static IEnumerable<(string Time, long? Count)> QueryUniqueDeviceStat(long appKey, int start, string granularity = "DAY")
        {
            var client = Client();
            QueryUniqueDeviceStatRequest request = new QueryUniqueDeviceStatRequest();
            request.AppKey = appKey;

            request.Granularity = granularity;
            String startTime = DateTime.UtcNow.AddDays(start).ToString("yyyy-MM-ddTHH\\:mm\\:ssZ");
            String endTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ssZ");
            request.StartTime = startTime;
            request.EndTime = endTime;

            try
            {
                QueryUniqueDeviceStatResponse response = client.GetAcsResponse(request);

                using (var temp = ListPool<(string Time, long? Count)>.Borrow())
                {
                    var list = temp.Item;
                    foreach (var stat in response.AppDeviceStats)
                    {
                        list.Add((stat.Time, stat.Count));
                    }

                    return list;
                }
            }
            catch (ServerException e)
            {
                ServerException(e);
            }
            catch (ClientException e)
            {
                ClientException(e);
            }

            return null;
        }

        private static void ServerException(ServerException e)
        {
            Console.WriteLine(e.ErrorCode);
            Console.WriteLine(e.ErrorMessage);
            throw new Exception(string.Format("{0},{1}", e.ErrorCode, e.ErrorMessage));
        }

        private static void ClientException(ClientException e)
        {
            Console.WriteLine(e.ErrorCode);
            Console.WriteLine(e.ErrorMessage);
            throw new Exception(string.Format("{0},{1}", e.ErrorCode, e.ErrorMessage));
        }
    }


    static class AliUtil
    {

        public static string GetAccessKeyId()
        {
            return ConfigurationManager.AppSettings["accessKeyId"];
        }

        public static string GetAccessKeySecret()
        {
            return ConfigurationManager.AppSettings["accessKeySecret"];
        }

        public static long GetAndroidAppKey()
        {
            var key = ConfigurationManager.AppSettings["androidAppKey"];
            return long.Parse(key);
        }

        public static long GetIOSAppKey()
        {
            var key = ConfigurationManager.AppSettings["iosAppKey"];
            return long.Parse(key);
        }
    }

    //public static class PushTarget
    //{
    //    //广播推送
    //    public const string All = "All";
    //    //按设备推送
    //    public const string DEVICE = "DEVICE";
    //    //按别名推送
    //    public const string ALIAS = "ALIAS";
    //    //按帐号推送
    //    public const string ACCOUNT = "ACCOUNT";
    //    //按标签推送
    //    public const string TAG = "TAG";
    //}

    //public static class KeyType
    //{
    //    //账号
    //    public const string ACCOUNT = "ACCOUNT";
    //    //设备
    //    public const string DEVICE = "DEVICE";
    //    //别名
    //    public const string ALIAS = "ALIAS";

    //}

}