//using CodeArt.AOP;

//namespace CodeArt.Web.WebPages
//{
//    public class WebCrossDomain : AspectInitializerBase
//    {
//        private string _originString = "*";
//        //private string _headerString = "Authorization,Content-Type,Accept,Origin,User-Agent,DNT,Cache-Control,X-Mx-ReqToken,Keep-Alive,X-Requested-With,If-Modified-Since,PostAction,Content-Disposition";
//        private string _headerString = "*";

//        public WebCrossDomain()
//            : this(null, null)
//        {
//        }

//        public WebCrossDomain(string[] origins, string[] headers)
//        {
//            if (origins != null && origins.Length > 0) _originString = string.Join(",", origins);
//            if (headers != null && headers.Length > 0) _headerString = string.Format("{0},{1}", _headerString, string.Join(",", headers));
//        }

//        public override void Init()
//        {
//            //跨域通过配置来，不通过代码来设置，因为会重叠设置，导致跨域失败
//            //var context = WebPageContext.Current;
//            //context.Response.AddHeader("Access-Control-Allow-Origin", _originString);
//            //context.Response.AddHeader("Access-Control-Allow-Headers", _headerString);
//            //context.Response.AddHeader("Access-Control-Allow-Methods", "POST,OPTIONS,GET");
//            //context.Response.AddHeader("Access-Control-Allow-Credentials", "true");
//        }

//        public static readonly WebCrossDomain Instance = new WebCrossDomain();

//    }
//}
