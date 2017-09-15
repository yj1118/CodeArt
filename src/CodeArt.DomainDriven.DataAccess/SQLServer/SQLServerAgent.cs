using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;

using CodeArt.Runtime;
using CodeArt.DomainDriven;
using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    [SafeAccess]
    public sealed class SQLServerAgent : DatabaseAgent
    {
        public override string Database => DatabaseType.SQLServer;

        public static readonly SQLServerAgent Instance = new SQLServerAgent();
    }
}
