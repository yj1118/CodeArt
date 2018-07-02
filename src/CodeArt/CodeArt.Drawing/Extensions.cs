using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.IO;

namespace CodeArt.Drawing
{
    public static class Extensions
    {
        public static System.Drawing.Image GetImage(this Stream stream)
        {
            return System.Drawing.Image.FromStream(stream);
        }
    }
}
