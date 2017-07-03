using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Transactions;

using CodeArt;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// ²Ö´¢ÐÐÎªÃ¶¾Ù
    /// </summary>
    public enum RepositoryAction : byte
    {
        Add = 1,
        Update = 2,
        Delete = 3,
        Query = 4
    }
}
