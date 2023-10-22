using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Timers;

using CodeArt.DTO;
using CodeArt.Web;
using CodeArt.Log;

namespace CodeArt.WeChat
{
    public static class WeChatToken
    {
        private static Dictionary<string, string> _cache = new Dictionary<string, string>();

        private static object _syncObject = new object();

        private static void Clear()
        {
            lock(_syncObject)
            {
                _cache.Clear();
            }
        }

        public static void Remove(string wxapp)
        {
            if (!_cache.TryGetValue(wxapp, out var token)) return;

            lock (_cache)
            {
                _cache.Remove(wxapp);
            }
        }

        /// <summary>
        /// app微信小程序的名称，跟配置文件里对应上即可
        /// </summary>
        /// <param name="wxapp"></param>
        /// <returns></returns>
        public static string Get(string wxapp)
        {
            {
                if (_cache.TryGetValue(wxapp, out var token)) return token;
            }

            lock(_cache)
            {
                if (_cache.TryGetValue(wxapp, out var token)) return token;

                token = Load(wxapp);
                _cache.Add(wxapp, token);
                return token;
            }
        }

        private static string Load(string wxapp)
        {
            var appid = ConfigurationManager.AppSettings["wx-appid-" + wxapp];
            var secret = ConfigurationManager.AppSettings["wx-secret-" + wxapp];

            if (string.IsNullOrEmpty(appid)
                || string.IsNullOrEmpty(secret))
            {
                throw new UserUIException("没有配置" + wxapp + "的appid或secret");
            }

            var tokenUrl = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", appid, secret);
            var tokenArray = WebUtil.SendGet(tokenUrl);

            var data = DTObject.Create(Encoding.UTF8.GetString(tokenArray));
            if (!data.Exist("access_token"))
            {
                throw new UserUIException("没有找到access_token，代码为" + data.GetCode(false, false));
            }
            return data.GetValue<string>("access_token");
        }



        #region 定时器

        private static Timer _timer;


        private static void InitTimer()
        {
            const int time = 1000 * 60 * 90; //90分钟刷新一次，token是2小时过期
            _timer = new Timer(time);
            _timer.Elapsed += OnElapsed;
            _timer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)
            _timer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
        }

        private static void OnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Clear();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
            finally
            {
                _timer.Start();
            }
        }

        private static void ClearTimer()
        {
            _timer.Close();
            _timer.Dispose();
        }

        #endregion

        public static void Initialize()
        {
            InitTimer();
        }

        public static void Dispose()
        {
            ClearTimer();
        }

    }
}
