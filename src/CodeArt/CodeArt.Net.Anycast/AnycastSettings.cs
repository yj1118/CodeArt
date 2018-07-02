using CodeArt.Log;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;


namespace CodeArt.Net.Anycast
{
    public class AnycastSettings
    {
        internal const int MissedIntervalsTimeout = 10; //心跳超时时间（10秒）
    }
}
