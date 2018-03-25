using CodeArt.AOP;

namespace CodeArt.Web.WebPages
{
    public class WebCrossDomain : AspectInitializerBase
    {
        private string _originString = "*";
        private string _headerString = "Authorization,Content-Type,Accept,Origin,User-Agent,DNT,Cache-Control,X-Mx-ReqToken,Keep-Alive,X-Requested-With,If-Modified-Since,PostAction,Content-Disposition";

        public WebCrossDomain()
            : this(null, null)
        {
        }

        public WebCrossDomain(string[] origins, string[] headers)
        {
            if (origins != null && origins.Length > 0) _originString = string.Join(",", origins);
            if (headers != null && headers.Length > 0) _headerString = string.Format("{0},{1}", _headerString, string.Join(",", headers));
        }

        public override void Init()
        {
            var context = WebPageContext.Current;
            context.Response.AddHeader("Access-Control-Allow-Origin", _originString);
            context.Response.AddHeader("Access-Control-Allow-Headers", _headerString);
            context.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            context.Response.AddHeader("Access-Control-Allow-Credentials", "true");
            //if (context.Request.HttpMethod == "OPTIONS")
            //{
            //    context.Response.AddHeader("Access-Control-Allow-Origin", _originString);
            //    context.Response.AddHeader("Access-Control-Allow-Headers", _headerString);
            //    context.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            //}
            //else
            //{
            //    context.Response.AddHeader("Access-Control-Allow-Origin", _originString);
            //    context.Response.AddHeader("Access-Control-Allow-Credentials", "true");
            //    //context.Response.AddHeader("Access-Control-Allow-Headers", _headerString);
            //    //
            //}
        }

        public static WebCrossDomain Instance = new WebCrossDomain();

    }
}
