using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using CodeArt.DTO;

namespace CodeArt.ServiceModel
{
    public abstract class AuthProvider : ServiceProvider
    {
        protected override DTObject Invoke(DTObject arg)
        {
            var target = arg.GetObject("target");
            var data = arg.GetObject("data");
            return Verify(target, data) ? Success : DTObject.Empty;
        }

        private readonly static DTObject Success = DTObject.Create("{success:true}").AsReadOnly();

        protected abstract bool Verify(DTObject target, DTObject data);

    }
}