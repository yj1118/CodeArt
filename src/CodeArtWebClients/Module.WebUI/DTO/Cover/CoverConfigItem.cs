using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace Module.WebUI
{

    [DTOClass]
    public struct CoverConfigItem
    {
        [DTOMember("name")]
        public string Name
        {
            get;
            set;
        }

        [DTOMember("width")]
        public int Width
        {
            get;
            set;
        }

        [DTOMember("height")]
        public int Height
        {
            get;
            set;
        }
    }
}
