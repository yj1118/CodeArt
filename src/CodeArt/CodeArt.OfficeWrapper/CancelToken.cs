using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Office
{
    public sealed class CancelToken
    {
        public bool IsCanceled
        {
            get;
            private set;
        }

        public void Cancel()
        {
            this.IsCanceled = true;
        }

        public CancelToken()
        {
            this.IsCanceled = false;
        }

    }
}
