﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;

namespace RPC.Common
{
    [Procedure("GetVerificationCode")]
    [SafeAccess()]
    public class GetVerificationCode : Procedure
    {
        public override DTObject Invoke(DTObject arg)
        {
            return ServiceContext.Invoke("getVerificationCode", arg);
        }
    }
}
