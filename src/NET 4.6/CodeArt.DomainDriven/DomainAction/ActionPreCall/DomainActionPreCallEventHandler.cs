﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public delegate void DomainActionPreCallEventHandler(object sender, DomainActionPreCallEventArgs e);
}