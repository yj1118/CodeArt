using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;

[assembly: PreApplicationStartAttribute(typeof(CodeArt.Media.ApplicationLife),"Init")]
[assembly: PreApplicationEndAttribute(typeof(CodeArt.Media.ApplicationLife), "Clear")]

namespace CodeArt.Media
{
    public static class ApplicationLife
    {
        public static void Init()
        {
            ScreenStreamer.CleanUp();
            FFMpeg.CleanUp();
        }

        public static void Clear()
        {
            ScreenStreamer.CleanUp();
            FFMpeg.CleanUp();
        }

    }
}
