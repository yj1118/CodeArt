using System.Drawing;

namespace CodeArt.Drawing
{
    public class Frame
    {
        public int Delay = 0;
        public Bitmap Img;
        public Color BgColor;
        public Frame(Bitmap img, int delay,Color c)
        {
            Delay = delay;
            Img = img;
            BgColor = c;
        }
    }
}
