using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;
using CodeArt.Web.WebPages;
using CodeArt.Util;

namespace RPC.Common
{
    public class ImageSize
    {
        //默认质量
        public const int DefaultQuality = 30;

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }

        private ImageSize(int width,int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public readonly static ImageSize _200x200 = new ImageSize(200, 200);
        public readonly static ImageSize _400x400 = new ImageSize(400, 400);
        public readonly static ImageSize _600x600 = new ImageSize(600, 600);
        public readonly static ImageSize _900x405 = new ImageSize(900, 405);
        public readonly static ImageSize _900x1080 = new ImageSize(900, 1080);
        public readonly static ImageSize _1200x750 = new ImageSize(1200, 750);
        public readonly static ImageSize _900x900 = new ImageSize(900, 900);
    }
}