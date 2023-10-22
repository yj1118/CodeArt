using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt;
using CodeArt.Log;
using CodeArt.DomainDriven;
using CodeArt.DTO;
using CodeArt.ServiceModel;
using CodeArt.Concurrent;

namespace CodeArt.TestPlatform
{
    [SafeAccess]
    public class ServiceRecorder : IServiceRecorder
    {
        private ServiceRecorder() { }

        public void Write(string name, DTObject input, DTObject output)
        {
            var invoke = new ServiceInvoke(Guid.NewGuid(), name, input.GetCode(false, false), output.GetCode(false, false));
            var repository = Repository.Create<IServiceInvokeRepository>();
            repository.Add(invoke);
        }

        public static readonly ServiceRecorder Instance = new ServiceRecorder();
    }
}
