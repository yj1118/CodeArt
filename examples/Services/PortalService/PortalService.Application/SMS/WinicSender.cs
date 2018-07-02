using System;
using System.Text;
using System.Configuration;
using CodeArt.Concurrent;

using CodeArt.Web;

namespace PortalService.Application
{
    /// <summary>
    /// 基于“吉信通”平台的SMSSender
    /// </summary>
    public class WinicSender : ISMSSender
    {
        private WinicSender() { }

        public void Send(string mobileNumber, string message)
        {
            string uid = ConfigurationManager.AppSettings["sms-name"];
            string pwd = ConfigurationManager.AppSettings["sms-password"];

            using (var temp = StringPool.Borrow())
            {
                var url = temp.Item;

                url.Append("http://service.winic.org/sys_port/gateway/?");
                url.AppendFormat("id={0}", uid);
                url.AppendFormat("&pwd={0}", pwd);
                url.AppendFormat("&to={0}", mobileNumber);
                url.AppendFormat("&content={0}", message);
                url.Append("&time=");

                var data = WebUtil.SendGet(url.ToString());

                string result = System.Text.Encoding.GetEncoding("GB2312").GetString(data).Trim();

                string error = null;
                switch (result)
                {
                    case "-01": error = "当前账号余额不足!"; break;
                    case "-02": error = "当前用户ID错误!"; break;
                    case "-03": error = "当前密码错误!"; break;
                    case "-05": error = "手机号码格式不对!"; break;
                }

                if (!string.IsNullOrEmpty(error)) throw new ApplicationException(error);
            }


                

            //____________________________

            //MSXML2.XMLHTTP xmlhttp = new MSXML2.XMLHTTP();

            //xmlhttp.open("GET", Send_URL.ToString(), false, null, null);
            //xmlhttp.send("");
            //MSXML2.XMLDocument dom = new XMLDocument();
            //Byte[] b = (Byte[])xmlhttp.responseBody;

            //string result = System.Text.Encoding.GetEncoding("GB2312").GetString(b).Trim();

            //返回示例
            //-02   /Send:1     /Consumption:0  / Tmoney:0 / sid
            //状态码 / 发送条数 / 当次消费金额 / 总体余额 / 短信编号

            //返回状态码 信息说明
            //000 成送成功！
            //-01 当前账号余额不足！
            //-02 当前用户ID错误！
            //-03 当前密码错误！
            //-04 参数不够或参数内容的类型错误！
            //-05 手机号码格式不对！
            //-06 短信内容编码不对！
            //-07 短信内容含有敏感字符！
            //-8 无接收数据
            //-09 系统维护中..
            //-10 手机号码数量超长！（100个/次 超100个请自行做循环）
            //-11 短信内容超长！（70个字符）
            //-12 其它错误！
            //string error = null;
            //switch (result)
            //{
            //    case "-01": error = "当前账号余额不足!"; break;
            //    case "-02": error = "当前用户ID错误!"; break;
            //    case "-03": error = "当前密码错误!"; break;
            //    case "-05": error = "手机号码格式不对!"; break;
            //}

            //if (!string.IsNullOrEmpty(error)) throw new ApplicationException(error);
        }

        //public void Send(string mobileNumber, string message)
        //{
        //    string uid = ConfigurationManager.AppSettings["sms-name"];
        //    string pwd = ConfigurationManager.AppSettings["sms-password"];

        //    StringBuilder Send_URL = new StringBuilder();

        //    Send_URL.Append("http://service.winic.org/sys_port/gateway/?");
        //    Send_URL.Append("id=" + uid);
        //    Send_URL.Append("&pwd=" + pwd);
        //    Send_URL.Append("&to=" + mobileNumber);
        //    Send_URL.Append("&content=" + message);
        //    Send_URL.Append("&time=");

        //    //____________________________

        //    MSXML2.XMLHTTP xmlhttp = new MSXML2.XMLHTTP();

        //    xmlhttp.open("GET", Send_URL.ToString(), false, null, null);
        //    xmlhttp.send("");
        //    MSXML2.XMLDocument dom = new XMLDocument();
        //    Byte[] b = (Byte[])xmlhttp.responseBody;

        //    string result = System.Text.Encoding.GetEncoding("GB2312").GetString(b).Trim();

        //    //返回示例
        //    //-02   /Send:1     /Consumption:0  / Tmoney:0 / sid
        //    //状态码 / 发送条数 / 当次消费金额 / 总体余额 / 短信编号

        //    //返回状态码 信息说明
        //    //000 成送成功！
        //    //-01 当前账号余额不足！
        //    //-02 当前用户ID错误！
        //    //-03 当前密码错误！
        //    //-04 参数不够或参数内容的类型错误！
        //    //-05 手机号码格式不对！
        //    //-06 短信内容编码不对！
        //    //-07 短信内容含有敏感字符！
        //    //-8 无接收数据
        //    //-09 系统维护中..
        //    //-10 手机号码数量超长！（100个/次 超100个请自行做循环）
        //    //-11 短信内容超长！（70个字符）
        //    //-12 其它错误！
        //    string error = null;
        //    switch (result)
        //    {
        //        case "-01": error = "当前账号余额不足!"; break;
        //        case "-02": error = "当前用户ID错误!"; break;
        //        case "-03": error = "当前密码错误!"; break;
        //        case "-05": error = "手机号码格式不对!"; break;
        //    }

        //    if (!string.IsNullOrEmpty(error)) throw new ApplicationException(error);
        //}

        public static readonly WinicSender Instance = new WinicSender();

    }
}
