using System;

namespace CodeArt.Diagnostics
{
    internal sealed class ActionItem
    {
        public string Name { get; set; }
        public Action Action { get; set; }
    }
}