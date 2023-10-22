using System;
using System.Threading.Tasks;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Captcha.V20190722;
using TencentCloud.Captcha.V20190722.Models;
using System.Web;
using CodeArt.DTO;
using System.Configuration;

namespace RPC.Common
{
    public class TCaptcha
    {
        private string SecretId = "";
        private string SecretKey = "";
        private long CaptchaAppId = 0;
        private string AppSecretKey = "";

        public TCaptcha()
        {
            var code = ConfigurationManager.AppSettings["tcaptcha"];
            var data = DTObject.Create(code);

            SecretId = data.GetValue<string>("SecretId", string.Empty);
            SecretKey = data.GetValue<string>("SecretKey", string.Empty);
            CaptchaAppId = data.GetValue<long>("CaptchaAppId", 0);
            AppSecretKey = data.GetValue<string>("AppSecretKey", string.Empty);
        }

        public static readonly TCaptcha Instance = new TCaptcha();

        public bool Verify(string ticket, string randstr)
        {
            try
            {
                Credential cred = new Credential
                {
                    SecretId = SecretId,
                    SecretKey = SecretKey,
                };


                ClientProfile clientProfile = new ClientProfile();
                HttpProfile httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("captcha.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;

                CaptchaClient client = new CaptchaClient(cred, "", clientProfile);
                DescribeCaptchaResultRequest req = new DescribeCaptchaResultRequest();

                req.CaptchaAppId = (ulong)CaptchaAppId;
                req.AppSecretKey = AppSecretKey;

                req.UserIp = HttpContext.Current.Request.UserHostAddress;
                req.CaptchaType = 9;
                req.Ticket = ticket;
                req.Randstr = randstr;

                DescribeCaptchaResultResponse resp = client.DescribeCaptchaResultSync(req);
                var result = AbstractModel.ToJsonString(resp);
                //"{\"CaptchaCode\":1,\"CaptchaMsg\":\"OK\",\"EvilLevel\":0,\"GetCaptchaTime\":0,\"EvilBitmap\":null,\"SubmitCaptchaTime\":1691068923,\"RequestId\":\"928b5360-3dcc-41fb-8e07-c0012980fe70\"}"
                var data = DTObject.Create(result);
                var code = data.GetValue<int>("CaptchaCode", 0);
                return code == 1;
            }
            catch (Exception e)
            {
                return false;
            }

        }
    }
}
