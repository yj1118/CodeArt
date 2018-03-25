using System;
using System.Web;
using System.Text;


namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class NonClientCache : IClientCache
    {
        private NonClientCache() { }

        public bool IsExpired(WebPageContext context)
        {
            return true;
        }

        public void SetCache(WebPageContext context)
        {
        }

        public static readonly IClientCache Instance = new NonClientCache();

    }
}
