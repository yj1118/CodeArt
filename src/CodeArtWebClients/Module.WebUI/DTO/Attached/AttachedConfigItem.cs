using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace Module.WebUI
{

    [DTOClass]
    public struct AttachedConfigItem
    {
        [DTOMember("name")]
        public string Name
        {
            get;
            set;
        }

        [DTOMember("type")]
        public string Type
        {
            get;
            set;
        }

        [DTOMember("required")]
        public bool Required
        {
            get;
            set;
        }


        [DTOMember("message")]
        public string Message
        {
            get;
            set;
        }



        [DTOMember("options")]
        public string[] Options
        {
            get;
            set;
        }
    }



}
