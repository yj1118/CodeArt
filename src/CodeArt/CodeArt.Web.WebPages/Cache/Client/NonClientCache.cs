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

        public bool IsExpired(ResolveRequestCache controller)
        {
            return true;
        }

        public void SetCache(ResolveRequestCache controller)
        {
        }

        public static readonly IClientCache Instance = new NonClientCache();

    }
}
