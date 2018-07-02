using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace CodeArt.TestTools.DomainDriven
{
    public class StartEventTest : EventCommand<NodeEvent0>
    {
        private string _config;

        public StartEventTest(string config)
        {
            _config = config;
        }

        protected override NodeEvent0 CreateEvent()
        {
            return new NodeEvent0()
            {
                Code = _config
            };
        }
    }
}
