using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace Module.WebUI
{

    [DTOClass]
    public struct ExpandedProperty
    {
        [DTOMember("name")]
        public string Name
        {
            get;
            set;
        }

        [DTOMember("value")]
        public string value
        {
            get;
            set;
        }

    }



}
