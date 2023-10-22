using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using CodeArt.AppSetting;
using CodeArt.Util;
using CodeArt.DTO;
using CodeArt.Caching.Redis;
using CodeArt.Web;
using System.Text;

namespace CodeArt.WeChat
{
    public class WeChatUtil
    {

        #region 获取手机号

        public static string GetPhoneNumber(string wxapp, string code)
        {
            var data = GetPhoneData(wxapp, code);
            var errmsg = data.GetValue<string>("errmsg");

            if (errmsg != "ok")
            {
                var errcode = data.GetValue<int>("errcode", 0);
                if (errcode == 40001)    //token无效
                {
                    WeChatToken.Remove(wxapp);  //有可能是我们内部开发把token更新了，而服务器上的token还是老的，所以，主动删除一次，再获取新的
                    data = GetPhoneData(wxapp, code);

                    //再检查一次
                    errmsg = data.GetValue<string>("errmsg");
                    if (errmsg != "ok") throw new UserUIException(errmsg);
                }
                else
                    throw new UserUIException(errmsg);
            }

            return data.GetValue<string>("phone_info.phoneNumber");
        }

        private static DTObject GetPhoneData(string wxapp, string code)
        {
            var token = WeChatToken.Get(wxapp);

            var getPhonenUrl = string.Format("https://api.weixin.qq.com/wxa/business/getuserphonenumber?access_token={0}", token);

            var phoneArg = DTObject.Create();
            phoneArg.SetValue("code", code);
            var phoneStr = WebUtil.SendPost(getPhonenUrl, phoneArg.GetCode(false, false), Encoding.UTF8);

            return DTObject.Create(phoneStr);
        }


        #endregion


        /// <summary>
        /// 获得微信应用的key
        /// </summary>
        /// <param name="wxapp"></param>
        /// <returns></returns>
        public static (string APPId, string Secret) GetWXAppKey(string wxapp)
        {
            var appid = ConfigurationManager.AppSettings["wx-appid-" + wxapp];
            var secret = ConfigurationManager.AppSettings["wx-secret-" + wxapp];

            return (appid, secret);
        }

        /// <summary>
        /// 在微信小程序里，获得用户在微信里的身份编号
        /// </summary>
        /// <param name="wxapp"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static (string OpenId, string UnionId) GetUserIdByXCX(string wxapp, string code)
        {
            var key = GetWXAppKey(wxapp);
            var url = $"https://api.weixin.qq.com/sns/jscode2session?appid={key.APPId}&secret={key.Secret}&js_code={code}&grant_type=authorization_code";

            var reuslt = WebUtil.SendGet(url);
            var reusltStr = Encoding.UTF8.GetString(reuslt);
            var data = DTObject.Create(reusltStr);
            WeChatException.Check(data);

            var openId = data.GetValue<string>("openid",string.Empty);
            var unionId = data.GetValue<string>("unionid", string.Empty);
            return (openId, unionId);
        }

        //public static DTObject LoadOpenId(string wxapp, string code)
        //{
        //    var key = GetWXKey(wxapp);
        //    var url = $"https://api.weixin.qq.com/sns/jscode2session?appid={key.APPId}&secret={key.Secret}&js_code={code}&grant_type=authorization_code";
        //    var reuslt = WebUtil.SendGet(url);
        //    var reusltStr = Encoding.UTF8.GetString(reuslt);
        //    var data = DTObject.Create(reusltStr);
        //    WeChatException.Check(data);
        //    return data;
        //}

        /// <summary>
        /// 获得用户的openId
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetOpenId(string code, out int errorCode)
        {
            string openId = string.Empty;
            errorCode = 0;
            try
            {
                var data = LoadOpenId(code);
                openId = data.GetValue<string>("openid", string.Empty);
            }
            catch (WeChatException ex)
            {
                errorCode = ex.Code;
            }
            catch (Exception)
            {
                throw;
            }
            return openId;
        }

        private static DTObject LoadOpenId(string code)
        {
            var config = WeChatConfiguration.Current;
            var url = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", config.MPAppId, config.MPAppSecret, code);
            var reuslt = WebUtil.SendGet(url);
            var reusltStr = Encoding.UTF8.GetString(reuslt);
            var data = DTObject.Create(reusltStr);
            WeChatException.Check(data);
            return data;
        }

        /// <summary>
        /// 轮询公众号中所有关注的用户的openId和unionId等信息
        /// </summary>
        public static void EachOpenIds(string access_token, Func<IEnumerable<string>,bool> action)
        {
            string next_openId = string.Empty;

            {
                var url = $"https://api.weixin.qq.com/cgi-bin/user/get?access_token={access_token}&next_openid={next_openId}";
                var data_array = WebUtil.SendGet(url);
                var data = DTObject.Create(Encoding.UTF8.GetString(data_array));
                next_openId = data.GetValue<string>("next_openid", string.Empty);
                var openIds = data.GetValues<string>("data.openid");
                if (!action(openIds)) return;   //处理编号，如果返回false,那么不再轮询
            }

            while(!string.IsNullOrEmpty(next_openId))
            {
                var url = $"https://api.weixin.qq.com/cgi-bin/user/get?access_token={access_token}&next_openid={next_openId}";
                var data_array = WebUtil.SendGet(url);
                var data = DTObject.Create(Encoding.UTF8.GetString(data_array));
                var openIds = data.GetValues<string>("data.openid");
                next_openId = data.GetValue("next_openid", string.Empty);
                if (!action(openIds)) return;   //处理编号，如果返回false,那么不再轮询
            }

            //"{\"total\":255,\"count\":255,\"data\":{\"openid\":[\"oJS8U6U2LYfSOeYrbQf6SSxvNkmM\",\"oJS8U6cPTm3bxXqKwcAX7HYgpoIA\",\"oJS8U6e0joF7bbuDBWiHyFjETfyw\",\"oJS8U6Yu3iRT1LJp3pTrIanfAyx0\",\"oJS8U6WclfUHYEL_sYpodfv_x8eg\",\"oJS8U6eiKR6Q4pYiz3BILoiaqoX4\",\"oJS8U6e6pd5kkBj4KA5rB6h4ap34\",\"oJS8U6YZ4JXjcZiczHqF7Br7fldU\",\"oJS8U6Sp40bO5R9yzgq_cd68uoWc\",\"oJS8U6XRO_cZUdLt9NeOiYHLdNJ0\",\"oJS8U6Xixxk-miFhT1Ycrp5-1So0\",\"oJS8U6U4sMTEm6_h6NtSaFWNTz_g\",\"oJS8U6YvDduqA2ALVf15bvJnUjwQ\",\"oJS8U6ROfvX_UUmHrWxvhUz3SwUk\",\"oJS8U6ZMSysvJAO4CQo9Yc5bHFl4\",\"oJS8U6TKIwY2b0B_XaOt8wkaAD0A\",\"oJS8U6aNo8OpMuFszl8qAyMNhsmI\",\"oJS8U6amsrqGkvChrUjV5RAvPrOo\",\"oJS8U6Yvy9gAohWJHmwk9Cx2umcY\",\"oJS8U6VvKL6BXGyvE-Gl63ATkApQ\",\"oJS8U6e6QjH7Nhm3dIyFoYft4_3s\",\"oJS8U6dW4Gaaz1OZJB12pea_jCCE\",\"oJS8U6cNDEy91-b6MSiNfLp_1fKM\",\"oJS8U6Q7Jj12jz_CW8LpgYJUJgPI\",\"oJS8U6RGDYAyotZKCfQZrbDXysZs\",\"oJS8U6cJYWtaZCSGmOY7NJPANaus\",\"oJS8U6Y7X2yAF1u5DsWmPhCUkZ_U\",\"oJS8U6QwX7z0icTaRieRqarGwrXQ\",\"oJS8U6SEQattRwNEtcO7NT-Kvjys\",\"oJS8U6coOh07t3FKObE117GsiQx0\",\"oJS8U6TGMzSiCVP_mNtt8Xr_Q6nM\",\"oJS8U6brjH-BnPynLd_O6O9RXPOE\",\"oJS8U6dgxhlRwp7KsASIRGFOkOsU\",\"oJS8U6VvuxL_9RR1Fnyha5l-T9dA\",\"oJS8U6QqFr58cnkXCqp0rYnolKzE\",\"oJS8U6WlZXGX4zrDNJiHN67_ja44\",\"oJS8U6ckxnTSZ-gMGU8CgUZag-Xk\",\"oJS8U6b81n3LYuoomwXo00jKChXI\",\"oJS8U6WsokCXmpUCFNFiFw-QejMY\",\"oJS8U6QuiAbvAiB9klZszKXrdCcs\",\"oJS8U6SvDSDAVawO-wo8u5y2D5Vs\",\"oJS8U6Y3tQkiKoU4TKaZbLFN4_QA\",\"oJS8U6S_2dQqKaxZdAVW8l3xrXMM\",\"oJS8U6YX4HlAGyTcAhUdPb5IjCXs\",\"oJS8U6egkohjGVfEBhukckQ9RQXI\",\"oJS8U6fTGy94h_y-rz2NHhoDdfBA\",\"oJS8U6U-IDcoZVoz2AyIS2BpvRik\",\"oJS8U6VsMm44CogkDM3CPa415bIk\",\"oJS8U6e8MsijNq3m7aJodem3Cqmg\",\"oJS8U6UiFles0TIR3pqOGozazE4Y\",\"oJS8U6cT_DFgL56zINn-J1wPOsak\",\"oJS8U6Z-mmQpNQDou0p97bGutT8c\",\"oJS8U6VVJT3S1F2EhKu7ficStg38\",\"oJS8U6aEwScfS50SwhGtsYn2SkV8\",\"oJS8U6aX7WXhVrSAhOHreCPBkC_c\",\"oJS8U6ScGy1BqT4KRo1BQAP-Byno\",\"oJS8U6X5gDYxWrl7qDdeI0C3nxwU\",\"oJS8U6ZiSG-lYH2TGb6FumsrmhCs\",\"oJS8U6b9cmp3Q2jjOhZDwMpF6eLQ\",\"oJS8U6cKrCOkb-v5JAp7mrtKAMZ4\",\"oJS8U6QWfsUeL-cAK6ZWzqnGkO_Q\",\"oJS8U6WTH7byCplJ3FbA1K1vl6LE\",\"oJS8U6WRcfQpxv4qUHOpZILT25Uk\",\"oJS8U6TabKgW4UDmGFbUbK48GybU\",\"oJS8U6R8-zS4BLb9i-GIYmncvlKg\",\"oJS8U6a8B3eyOnvstHU29nacmgN4\",\"oJS8U6aCOSZAaHOw-VSSEEH31rW0\",\"oJS8U6SNFX4o9lDTY-e8Q6wMzJCo\",\"oJS8U6UhpviFaa_OQ02bsg-wHLXo\",\"oJS8U6cgZElv5Q8jV1ca7Ktt6U1o\",\"oJS8U6WH1WGob9BOrFVYb5EY8mFI\",\"oJS8U6V2bjRpOekmGHvrZppnpEQ8\",\"oJS8U6XMIigce3fVn0B3K8ZIUGyU\",\"oJS8U6QU-PKSnyZv4bw_yLFrXvmY\",\"oJS8U6QyKMYuvaUA6gPrsPiHgMkw\",\"oJS8U6ckaRO_EgQkgSqZ7IqT_Q7g\",\"oJS8U6SJvXG1NoVuaCSFHNGYszlU\",\"oJS8U6az_v4ShddASSXAuWcmuSMQ\",\"oJS8U6YWcQ_K_Jg4ZoGr5UkCieA0\",\"oJS8U6fqyC9mVlazUmRkSAsFSux0\",\"oJS8U6SM2vYmwHW0C22nDQrYlxj0\",\"oJS8U6ZE3v0-TtX-w7N8YdzcLQVE\",\"oJS8U6auWWG_0IiAzSC_SDj-BbGo\",\"oJS8U6cq0aLIL7gOmPlNzBxgXKkg\",\"oJS8U6Rbc0oU879SO2Q-_pCchkDo\",\"oJS8U6bHO_CSDi6olUElm0POuFgQ\",\"oJS8U6ZkoB-5Tt2Yb0PBF1LYtJDU\",\"oJS8U6ancBjAAt3Hca21G-dn3dGc\",\"oJS8U6clcWSIJCauWsis4NARj0oU\",\"oJS8U6X6eftnYASAsSdc8Yy900ks\",\"oJS8U6ZTOPSUFjWB9qEQD7Ff99pU\",\"oJS8U6akrzffOUyOl_nL3a9klR1E\",\"oJS8U6dOA8Jdi8U0iNf3YArd_1BA\",\"oJS8U6e2rpjZCGh5-_KU_9MubHdw\",\"oJS8U6TElyETGyWBlefaR4tTKRBs\",\"oJS8U6fB-L_qztVGR-Uwzk0UwaLI\",\"oJS8U6f28rKoXUCZEkjK8_FkcLg0\",\"oJS8U6RW24ZleBldiQTiX76fcc9Y\",\"oJS8U6aHpI9RjrRa8WI4M6Z_1YI4\",\"oJS8U6b9jvl-4gcsMGMNBA5pjEcI\",\"oJS8U6VWcHEQG4zO4ontT_yxyLgY\",\"oJS8U6cUmUJJGyP2FaST2is76ntM\",\"oJS8U6Zcr7CQtRUBtFrnjna5M_Xo\",\"oJS8U6dkCUeDJc52qzqCGQof_8NI\",\"oJS8U6fN2CTvKoNJxig34fgUKbIc\",\"oJS8U6TSNcq1jRvIYxfEv6s4tw2s\",\"oJS8U6cJqOKiBOF1E9NlBoOrjh8c\",\"oJS8U6R06yhypq0KrPPrJnFWAtjs\",\"oJS8U6RmD6ggsnwEmmJhjrg0Ta_w\",\"oJS8U6TIPy40jljVKkBZ3P4_cEZs\",\"oJS8U6SRzZudzv2Oji5SfeJMCZaY\",\"oJS8U6Q1dWSJ0heDQ02WQPOmHmXw\",\"oJS8U6bd83zM1kfLzcclBEqgDT14\",\"oJS8U6fLk5NBdIo2-NaXYghhkRS8\",\"oJS8U6VSOvu2B8deoq_qwBENjUUA\",\"oJS8U6en3YwuSm1RwYido9cYIJSk\",\"oJS8U6U8wJLByyTQwIV-N2F25YNo\",\"oJS8U6RL0eLtU1lVMCpSNLj9s6eg\",\"oJS8U6anPyOa4gBggqaftCkEVG5Y\",\"oJS8U6QTduSRdBdEL_u5-ScfKRGQ\",\"oJS8U6V1odbnlkVl-ra8StenObwE\",\"oJS8U6dE120w9VmBGsv0IVWAx9yg\",\"oJS8U6aydncFefnYt8cNCv1W9ouI\",\"oJS8U6WDAuch64JR__R1LHnQ0wRc\",\"oJS8U6TsFwJB7V18_d5lrxdXuBoY\",\"oJS8U6bCgwTgklX16RFzagifumkA\",\"oJS8U6dBLuE5Dpau5PRoTZSD3VH8\",\"oJS8U6bMfCbSWVyRES3tCiV4gXz4\",\"oJS8U6QRK0Q3A7yhl_uSpoaPyZqw\",\"oJS8U6dupzAfxQM_dSXb1I_fr3GQ\",\"oJS8U6Qb-Zlt4N0qtwC_eCOOgSuo\",\"oJS8U6eyByyZQrwQPXesDvMRymw0\",\"oJS8U6cRPwgPgVSmBIPojQur3G6Q\",\"oJS8U6RASZZW4sZ9Ch_Mygzr_UfQ\",\"oJS8U6SEDfmGaz1uHA1ZI7DeD_rE\",\"oJS8U6e8t9HaFVdQeAospbFM8QMQ\",\"oJS8U6RA5CojczfTP83IhNBx01nY\",\"oJS8U6R1PMVpFJdoZxdz7zjKPg-8\",\"oJS8U6TDhEghggVJ79XvnUUvu3Eo\",\"oJS8U6dm14T0w2QPBSKsARzbIRmU\",\"oJS8U6R1Dwc_BJ2FLH2NbKoCvU1k\",\"oJS8U6ekIKOnW16_Zr98SnQS3CCc\",\"oJS8U6YBzHlCijHq1JV0zGR7qaRA\",\"oJS8U6QZGLtWFH4AftrpD2wzNOFc\",\"oJS8U6V-Czt1X-PEnp_zUvklzaKU\",\"oJS8U6T66f8c160Qf7WyNWjXnfJg\",\"oJS8U6YyYn9GHvsMsk84_R5f95NI\",\"oJS8U6UKAQEvBJK0qI4l1H-se87w\",\"oJS8U6XItwbkhDz7pcTaZi-rqzEw\",\"oJS8U6TFuHvHNnxOmMNEsUVtYa9c\",\"oJS8U6UBilFp8bsF5UTJLpzpA3oU\",\"oJS8U6YFs83-9mIylPyEeQ8MLCRA\",\"oJS8U6SN5js4Qjghw5D3JVRUUI8Y\",\"oJS8U6f2H4ChyerXpOMmlkaANP34\",\"oJS8U6d4d08pPnn4f83kXubdjjxM\",\"oJS8U6Z7QF35wj5hyOQSxtWw0PwQ\",\"oJS8U6cxHd4k3SKcCPimoCTuIwf8\",\"oJS8U6aSCz69DXviJPmZtCnfHQKs\",\"oJS8U6T_tqP0bHSPt3ozjD72BU6Q\",\"oJS8U6ee2Kd5fL-hohsWIhmL9AFI\",\"oJS8U6fxNTkolhDA7LfUGAY1bHLU\",\"oJS8U6ViEROz7vS5WyxRTirQRdlk\",\"oJS8U6RJkAw-EGxoaWcu3hj6Krj0\",\"oJS8U6b1BrfjuoxR_A-89vvMm3IA\",\"oJS8U6XQlfNfQr6NUKI0BBDYg9oo\",\"oJS8U6cmm0KnygDj_B0PYU2o4sZ0\",\"oJS8U6VwCF-oh52ADXvYC2_xJqNQ\",\"oJS8U6Tc-waULXkSsHbGfp8DLBFY\",\"oJS8U6eSTJfyVwpl8Rl5iygk9dXI\",\"oJS8U6R5Qes9hYAOq4Q1wCBClBDE\",\"oJS8U6TIV1JjfSJ4F19nJEjt656s\",\"oJS8U6VRPUMWJBjeSGZJw2w75oSU\",\"oJS8U6Q2aC04FjS4uHLq_793yE00\",\"oJS8U6YYIH4LR5qngiGQYzOMpSpo\",\"oJS8U6ViXDChiT2PafKhx0afMsIQ\",\"oJS8U6frlV2_LIOUEWelajvAnn94\",\"oJS8U6WNmDWIuRH7FMea6qZXeWCE\",\"oJS8U6cX67xj9pfeSTC6w5ppb27s\",\"oJS8U6UOfMQ-W19NtENKaQSWAZzc\",\"oJS8U6Wza-pZCQRvujV1IH7mQldc\",\"oJS8U6YSNNXFuf40820vwXoZIASg\",\"oJS8U6WWzR_kcRr4jIs0Q9wlZ44Y\",\"oJS8U6XiqA-Jxo-oa_BAHoQY2AaM\",\"oJS8U6fNtUlafhx5p07DlqARTNcM\",\"oJS8U6X5MiX_fzQHDD2GPWUvBuc4\",\"oJS8U6XgLOt1xJ94TqIWrFWuK3zA\",\"oJS8U6QoEVb61v5Qh5k4DbB4N46U\",\"oJS8U6fkAGR7v0vyVzCB8p-taa4E\",\"oJS8U6SEPzcBRvxD7RehSwXPTURI\",\"oJS8U6dMEOMfI6ynP_0Tgy9uHhZc\",\"oJS8U6Q5ufNQkyhf65je52sAnvhc\",\"oJS8U6VSN6cByqaIGfbDrhmrZ7HY\",\"oJS8U6bKWMqqmjPKqL9qvicQAVA8\",\"oJS8U6R-ZV5lFdgNwBB9e3zvysXs\",\"oJS8U6Rv4Ir_Ts5nvDHn8TcNnBQA\",\"oJS8U6UDtkl6fK8897Dfn12AqqpI\",\"oJS8U6dnCmuZs0S9ibHfvDuv8bkA\",\"oJS8U6cNp0F5DHknYFPOpMedUeHk\",\"oJS8U6QBh1VYNyUhPoCESakvRAEU\",\"oJS8U6YiKckkQsOwJBF3g3vx8J18\",\"oJS8U6b_YFEsUEzxe3fMwglnXnOk\",\"oJS8U6VT3xVyHGDp-jA-OdXhqwsk\",\"oJS8U6Wnw1QU1fROLsyWV8VuWQxw\",\"oJS8U6W2SmWENNhDpgNru_VLghAM\",\"oJS8U6dEKKsmhothWfIFiYcijVBQ\",\"oJS8U6dCAih96Qn0mzKCg79pt9Jc\",\"oJS8U6T_aAQK7y-KC2fKp2rgGwYk\",\"oJS8U6UsA5cLrNni25gbFHAWthh0\",\"oJS8U6VLiK7M2Kuhx-US3hCowcwU\",\"oJS8U6TeOXhmiXizP374f_tBDV1U\",\"oJS8U6fOVm3VeK0WgNXstGxqNAOI\",\"oJS8U6ScbXBlGw0EsXo7g3Gduo-E\",\"oJS8U6fZqdAmZ2_gQ1_s883ESaMc\",\"oJS8U6U-26DLjjoYnFCWGcDfj49E\",\"oJS8U6c3LF1V-0-9HlJ8XDUFsKwc\",\"oJS8U6T0OdV48j2LQneeOeERfkvE\",\"oJS8U6QI3eM2fmWP1oAPO9LXUQaU\",\"oJS8U6UvjFYTvvRHWWUfF3sROX7U\",\"oJS8U6X-TKRoFt8-gk2nuzzWUXpg\",\"oJS8U6TJLQIfzxtHSnGIe8dLDCOM\",\"oJS8U6eHCRudP1hiVc7WsY21o_HA\",\"oJS8U6S5nni9NMdNeF7WfmfvmUP0\",\"oJS8U6eOscAC9bYFd290TGkN8Sw8\",\"oJS8U6fk8_K9FXgQdK9ayF9ZXjqE\",\"oJS8U6SYznSky7u0z9lUPfMO5X34\",\"oJS8U6YOs-EWhkpCec9PcjYq6bZM\",\"oJS8U6WnK56St_U8Rvw6xCoCt6q8\",\"oJS8U6d_JUUOSdSvWsnMByojyoIA\",\"oJS8U6QRlKegQk8RLfCEAtxPry8Y\",\"oJS8U6WfPZUJfTuWqZb5lj8vvFKs\",\"oJS8U6bCqVj1sHfVPP3eSMmnKeu4\",\"oJS8U6SsKtrrGC4PetqYWnE73NHM\",\"oJS8U6aMMsBZNc7dV7YKkUi4yfpc\",\"oJS8U6Y-hELbZvg74pxLGGG2_Bo8\",\"oJS8U6dNpNvfeEbOEgOHccNoEC_4\",\"oJS8U6aMt2psBZNQS8wluHLQUiyY\",\"oJS8U6TSCVIadBdIY9IEYi3z7Prs\",\"oJS8U6S-od0EHW5ponkcjgnJXlOQ\",\"oJS8U6XepV3BgWtn8Ahiyp6-dDJo\",\"oJS8U6YVCWCpkWkThexUtB0HS-so\",\"oJS8U6Zw7m_5yRJ2RBpEk5RrcjSc\",\"oJS8U6b334SBp-E0gI0GJPy5WYFI\",\"oJS8U6ansqJzPz335YpuzoyNuVXY\",\"oJS8U6XJ7vBs0JLhK5Bv1ezPeqPU\",\"oJS8U6SIS8KS9ILZYwzR5DSGzE_w\",\"oJS8U6e_mPGa68qY7xEeQEmJZHSQ\",\"oJS8U6QZpzB0lEKgAdujUL2vH_kk\",\"oJS8U6Y8VG6tZque5aIkZLEpy3SU\",\"oJS8U6eCviEcohW_yztnX1O87hus\",\"oJS8U6ZHYPS3rIS_fucCehq4rN18\",\"oJS8U6UnG9wGxLQkZkNHCkwS-80c\",\"oJS8U6SzP30DG15W_J7KejUiOepY\",\"oJS8U6Ww2iOhZ4Z21f2n_vCiGZCo\",\"oJS8U6duIWveG6tanQyVGiBieHjQ\",\"oJS8U6Z3TTdtdjwYulvTTMDoDkTk\"]},\"next_openid\":\"oJS8U6Z3TTdtdjwYulvTTMDoDkTk\"}"
        }

        /// <summary>
        /// 根据公众号的opeId，获得公众号下的用户信息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="openIds"></param>
        /// <returns></returns>
        public static DTObject GetUserInfo(string access_token, string openId)
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/user/info?access_token={access_token}&openid={openId}&lang=zh_CN";
            var data_array = WebUtil.SendGet(url);
            var data = DTObject.Create(Encoding.UTF8.GetString(data_array));
            return WeChatException.IsValid(data) ? data : DTObject.Empty;

//            {
//                "subscribe": 1, 
//    "openid": "o6_bmjrPTlm6_2sgVt7hMZOPfL2M", 
//    "language": "zh_CN", 
//    "subscribe_time": 1382694957,
//    "unionid": " o6_bmasdasdsad6_2sgVt7hMZOPfL",
//    "remark": "",
//    "groupid": 0,
//    "tagid_list":[128,2],
//    "subscribe_scene": "ADD_SCENE_QR_CODE",
//    "qr_scene": 98765,
//    "qr_scene_str": ""
//}

        }

    }
}
