using System;

using CodeArt.IO;

namespace CodeArt.ServiceModel
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Util
    {
        public static readonly SegmentSize DefaultRequestSegmentSize = SegmentSize.Byte256;

        public static readonly SegmentSize DefaultResponseSegmentSize = SegmentSize.KB1;
    }
}
