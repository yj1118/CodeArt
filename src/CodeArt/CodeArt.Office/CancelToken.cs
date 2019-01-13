using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Office
{
    public sealed class CancelToken
    {
        public bool IsCancelled
        {
            get;
            private set;
        }

        public void Cancel()
        {
            this.IsCancelled = true;
        }

        public CancelToken()
        {
            this.IsCancelled = false;
        }

    }
}
