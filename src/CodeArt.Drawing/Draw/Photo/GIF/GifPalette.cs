using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace CodeArt.Drawing
{
    public class GifPalette
    {
        private static ArrayList _cardPalette;
        private Color[] _colors;
        private Hashtable _colorMap;

        public GifPalette(ArrayList palette)
        {
            _colorMap = new Hashtable();
            _colors = new Color[palette.Count];
            palette.CopyTo(_colors);
        }

        public GifPalette()
        {
            ArrayList palette = SetPalette();
            _colorMap = new Hashtable();
            _colors = new Color[palette.Count];
            palette.CopyTo(_colors);
        }

        public Bitmap Quantize(Image source)
        {
            int height = source.Height;
            int width = source.Width;

            Rectangle bounds = new Rectangle(0, 0, width, height);

            Bitmap copy = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Bitmap output = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            using (Graphics g = Graphics.FromImage(copy))
            {
                g.PageUnit = GraphicsUnit.Pixel;

                g.DrawImageUnscaled(source, bounds);
            }

            BitmapData sourceData = null;

            try
            {
                sourceData = copy.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                output.Palette = this.GetPalette(output.Palette);
                SecondPass(sourceData, output, width, height, bounds);
            }
            finally
            {
                copy.UnlockBits(sourceData);
            }

            return output;
        }

        private ColorPalette GetPalette(ColorPalette palette)
        {
            for (int index = 0; index < _colors.Length; index++)
                palette.Entries[index] = _colors[index];
            return palette;
        }

        private unsafe void SecondPass(BitmapData sourceData, Bitmap output, int width, int height, Rectangle bounds)
        {
            BitmapData outputData = null;

            try
            {
                outputData = output.LockBits(bounds, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

                byte* pSourceRow = (byte*)sourceData.Scan0.ToPointer();
                Int32* pSourcePixel = (Int32*)pSourceRow;
                Int32* pPreviousPixel = pSourcePixel;

                byte* pDestinationRow = (byte*)outputData.Scan0.ToPointer();
                byte* pDestinationPixel = pDestinationRow;

                byte pixelValue = QuantizePixel((Color32*)pSourcePixel);

                *pDestinationPixel = pixelValue;

                for (int row = 0; row < height; row++)
                {
                    pSourcePixel = (Int32*)pSourceRow;

                    pDestinationPixel = pDestinationRow;

                    for (int col = 0; col < width; col++, pSourcePixel++, pDestinationPixel++)
                    {
                        if (*pPreviousPixel != *pSourcePixel)
                        {
                            pixelValue = QuantizePixel((Color32*)pSourcePixel);

                            pPreviousPixel = pSourcePixel;
                        }

                        *pDestinationPixel = pixelValue;
                    }

                    pSourceRow += sourceData.Stride;

                    pDestinationRow += outputData.Stride;
                }
            }
            finally
            {
                output.UnlockBits(outputData);
            }
        }

        private unsafe byte QuantizePixel(Color32* pixel)
        {
            byte colorIndex = 0;
            int colorHash = pixel->ARGB;

            if (_colorMap.ContainsKey(colorHash))
                colorIndex = (byte)_colorMap[colorHash];
            else
            {
                if (0 == pixel->Alpha)
                {
                    for (int index = 0; index < _colors.Length; index++)
                    {
                        if (0 == _colors[index].A)
                        {
                            colorIndex = (byte)index;
                            break;
                        }
                    }
                }
                else
                {
                    int leastDistance = int.MaxValue;
                    int red = pixel->Red;
                    int green = pixel->Green;
                    int blue = pixel->Blue;

                    for (int index = 0; index < _colors.Length; index++)
                    {
                        Color paletteColor = _colors[index];

                        int redDistance = paletteColor.R - red;
                        int greenDistance = paletteColor.G - green;
                        int blueDistance = paletteColor.B - blue;

                        int distance = (redDistance * redDistance) +
                            (greenDistance * greenDistance) +
                            (blueDistance * blueDistance);

                        if (distance < leastDistance)
                        {
                            colorIndex = (byte)index;
                            leastDistance = distance;

                            if (0 == distance)
                                break;
                        }
                    }
                }

                _colorMap.Add(colorHash, colorIndex);
            }

            return colorIndex;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct Color32
        {
            [FieldOffset(0)]
            public byte Blue;

            [FieldOffset(1)]
            public byte Green;

            [FieldOffset(2)]
            public byte Red;

            [FieldOffset(3)]
            public byte Alpha;

            [FieldOffset(0)]
            public int ARGB;

            public Color Color
            {
                get { return Color.FromArgb(Alpha, Red, Green, Blue); }
            }
        }

        public static ArrayList SetPalette()
        {
            if (null == _cardPalette)
            {
                _cardPalette = new ArrayList();

                //Insert the colors into the arraylist             
                #region Insert the colors into the arraylist
                _cardPalette.Add(Color.FromArgb(255, 0, 0, 0));
                _cardPalette.Add(Color.FromArgb(255, 128, 0, 0));
                _cardPalette.Add(Color.FromArgb(255, 0, 128, 0));
                _cardPalette.Add(Color.FromArgb(255, 128, 128, 0));
                _cardPalette.Add(Color.FromArgb(255, 0, 0, 128));
                _cardPalette.Add(Color.FromArgb(255, 128, 0, 128));
                _cardPalette.Add(Color.FromArgb(255, 0, 128, 128));
                _cardPalette.Add(Color.FromArgb(255, 192, 192, 192));
                _cardPalette.Add(Color.FromArgb(255, 192, 220, 192));
                _cardPalette.Add(Color.FromArgb(255, 166, 202, 240));
                _cardPalette.Add(Color.FromArgb(255, 1, 25, 83));
                _cardPalette.Add(Color.FromArgb(255, 1, 37, 92));
                _cardPalette.Add(Color.FromArgb(255, 2, 51, 103));
                _cardPalette.Add(Color.FromArgb(255, 18, 66, 114));
                _cardPalette.Add(Color.FromArgb(255, 39, 78, 123));
                _cardPalette.Add(Color.FromArgb(255, 101, 63, 107));
                _cardPalette.Add(Color.FromArgb(255, 72, 92, 119));
                _cardPalette.Add(Color.FromArgb(255, 89, 74, 121));
                _cardPalette.Add(Color.FromArgb(255, 85, 101, 122));
                _cardPalette.Add(Color.FromArgb(255, 122, 89, 127));
                _cardPalette.Add(Color.FromArgb(255, 101, 108, 106));
                _cardPalette.Add(Color.FromArgb(255, 111, 116, 111));
                _cardPalette.Add(Color.FromArgb(255, 109, 118, 122));
                _cardPalette.Add(Color.FromArgb(255, 120, 119, 97));
                _cardPalette.Add(Color.FromArgb(255, 121, 124, 114));
                _cardPalette.Add(Color.FromArgb(255, 1, 52, 154));
                _cardPalette.Add(Color.FromArgb(255, 16, 61, 156));
                _cardPalette.Add(Color.FromArgb(255, 15, 63, 160));
                _cardPalette.Add(Color.FromArgb(255, 37, 55, 131));
                _cardPalette.Add(Color.FromArgb(255, 24, 69, 158));
                _cardPalette.Add(Color.FromArgb(255, 21, 68, 162));
                _cardPalette.Add(Color.FromArgb(255, 35, 71, 137));
                _cardPalette.Add(Color.FromArgb(255, 33, 71, 152));
                _cardPalette.Add(Color.FromArgb(255, 43, 93, 130));
                _cardPalette.Add(Color.FromArgb(255, 51, 68, 139));
                _cardPalette.Add(Color.FromArgb(255, 48, 79, 159));
                _cardPalette.Add(Color.FromArgb(255, 53, 85, 131));
                _cardPalette.Add(Color.FromArgb(255, 49, 81, 151));
                _cardPalette.Add(Color.FromArgb(255, 34, 78, 167));
                _cardPalette.Add(Color.FromArgb(255, 41, 84, 170));
                _cardPalette.Add(Color.FromArgb(255, 50, 91, 173));
                _cardPalette.Add(Color.FromArgb(255, 54, 94, 176));
                _cardPalette.Add(Color.FromArgb(255, 53, 101, 136));
                _cardPalette.Add(Color.FromArgb(255, 60, 97, 168));
                _cardPalette.Add(Color.FromArgb(255, 59, 99, 177));
                _cardPalette.Add(Color.FromArgb(255, 68, 93, 134));
                _cardPalette.Add(Color.FromArgb(255, 67, 91, 150));
                _cardPalette.Add(Color.FromArgb(255, 86, 93, 151));
                _cardPalette.Add(Color.FromArgb(255, 64, 102, 142));
                _cardPalette.Add(Color.FromArgb(255, 76, 105, 153));
                _cardPalette.Add(Color.FromArgb(255, 75, 116, 150));
                _cardPalette.Add(Color.FromArgb(255, 81, 103, 140));
                _cardPalette.Add(Color.FromArgb(255, 84, 106, 147));
                _cardPalette.Add(Color.FromArgb(255, 91, 113, 146));
                _cardPalette.Add(Color.FromArgb(255, 75, 107, 172));
                _cardPalette.Add(Color.FromArgb(255, 65, 103, 180));
                _cardPalette.Add(Color.FromArgb(255, 77, 113, 184));
                _cardPalette.Add(Color.FromArgb(255, 90, 104, 162));
                _cardPalette.Add(Color.FromArgb(255, 90, 115, 160));
                _cardPalette.Add(Color.FromArgb(255, 90, 123, 189));
                _cardPalette.Add(Color.FromArgb(255, 101, 87, 130));
                _cardPalette.Add(Color.FromArgb(255, 106, 108, 158));
                _cardPalette.Add(Color.FromArgb(255, 101, 115, 130));
                _cardPalette.Add(Color.FromArgb(255, 103, 121, 149));
                _cardPalette.Add(Color.FromArgb(255, 112, 99, 139));
                _cardPalette.Add(Color.FromArgb(255, 122, 110, 148));
                _cardPalette.Add(Color.FromArgb(255, 101, 122, 165));
                _cardPalette.Add(Color.FromArgb(255, 116, 124, 172));
                _cardPalette.Add(Color.FromArgb(255, 93, 126, 192));
                _cardPalette.Add(Color.FromArgb(255, 96, 127, 192));
                _cardPalette.Add(Color.FromArgb(0, 72, 254, 42));
                _cardPalette.Add(Color.FromArgb(255, 90, 128, 160));
                _cardPalette.Add(Color.FromArgb(255, 97, 130, 159));
                _cardPalette.Add(Color.FromArgb(255, 119, 129, 138));
                _cardPalette.Add(Color.FromArgb(255, 118, 133, 154));
                _cardPalette.Add(Color.FromArgb(255, 107, 131, 169));
                _cardPalette.Add(Color.FromArgb(255, 105, 132, 186));
                _cardPalette.Add(Color.FromArgb(255, 118, 138, 170));
                _cardPalette.Add(Color.FromArgb(255, 117, 137, 180));
                _cardPalette.Add(Color.FromArgb(255, 118, 145, 173));
                _cardPalette.Add(Color.FromArgb(255, 124, 152, 183));
                _cardPalette.Add(Color.FromArgb(255, 95, 128, 192));
                _cardPalette.Add(Color.FromArgb(255, 102, 133, 195));
                _cardPalette.Add(Color.FromArgb(255, 112, 141, 199));
                _cardPalette.Add(Color.FromArgb(255, 120, 147, 202));
                _cardPalette.Add(Color.FromArgb(255, 154, 53, 53));
                _cardPalette.Add(Color.FromArgb(255, 131, 72, 91));
                _cardPalette.Add(Color.FromArgb(255, 143, 87, 104));
                _cardPalette.Add(Color.FromArgb(255, 129, 123, 92));
                _cardPalette.Add(Color.FromArgb(255, 156, 124, 68));
                _cardPalette.Add(Color.FromArgb(255, 129, 126, 101));
                _cardPalette.Add(Color.FromArgb(255, 154, 105, 120));
                _cardPalette.Add(Color.FromArgb(255, 169, 83, 82));
                _cardPalette.Add(Color.FromArgb(255, 165, 125, 70));
                _cardPalette.Add(Color.FromArgb(255, 160, 125, 80));
                _cardPalette.Add(Color.FromArgb(255, 176, 109, 114));
                _cardPalette.Add(Color.FromArgb(255, 205, 53, 2));
                _cardPalette.Add(Color.FromArgb(255, 209, 67, 19));
                _cardPalette.Add(Color.FromArgb(255, 210, 85, 47));
                _cardPalette.Add(Color.FromArgb(255, 230, 67, 21));
                _cardPalette.Add(Color.FromArgb(255, 243, 88, 46));
                _cardPalette.Add(Color.FromArgb(255, 255, 97, 53));
                _cardPalette.Add(Color.FromArgb(255, 201, 92, 71));
                _cardPalette.Add(Color.FromArgb(255, 214, 107, 79));
                _cardPalette.Add(Color.FromArgb(255, 205, 117, 105));
                _cardPalette.Add(Color.FromArgb(255, 245, 112, 74));
                _cardPalette.Add(Color.FromArgb(255, 131, 101, 136));
                _cardPalette.Add(Color.FromArgb(255, 139, 110, 144));
                _cardPalette.Add(Color.FromArgb(255, 130, 120, 156));
                _cardPalette.Add(Color.FromArgb(255, 146, 124, 155));
                _cardPalette.Add(Color.FromArgb(255, 162, 117, 131));
                _cardPalette.Add(Color.FromArgb(255, 141, 133, 94));
                _cardPalette.Add(Color.FromArgb(255, 156, 131, 72));
                _cardPalette.Add(Color.FromArgb(255, 151, 133, 83));
                _cardPalette.Add(Color.FromArgb(255, 157, 144, 92));
                _cardPalette.Add(Color.FromArgb(255, 137, 132, 102));
                _cardPalette.Add(Color.FromArgb(255, 135, 136, 120));
                _cardPalette.Add(Color.FromArgb(255, 147, 139, 102));
                _cardPalette.Add(Color.FromArgb(255, 148, 143, 115));
                _cardPalette.Add(Color.FromArgb(255, 156, 146, 106));
                _cardPalette.Add(Color.FromArgb(255, 148, 145, 122));
                _cardPalette.Add(Color.FromArgb(255, 168, 136, 73));
                _cardPalette.Add(Color.FromArgb(255, 168, 138, 88));
                _cardPalette.Add(Color.FromArgb(255, 172, 147, 90));
                _cardPalette.Add(Color.FromArgb(255, 178, 138, 82));
                _cardPalette.Add(Color.FromArgb(255, 186, 153, 69));
                _cardPalette.Add(Color.FromArgb(255, 179, 150, 91));
                _cardPalette.Add(Color.FromArgb(255, 174, 139, 100));
                _cardPalette.Add(Color.FromArgb(255, 166, 154, 107));
                _cardPalette.Add(Color.FromArgb(255, 161, 151, 114));
                _cardPalette.Add(Color.FromArgb(255, 182, 154, 101));
                _cardPalette.Add(Color.FromArgb(255, 190, 162, 81));
                _cardPalette.Add(Color.FromArgb(255, 172, 160, 117));
                _cardPalette.Add(Color.FromArgb(255, 183, 161, 103));
                _cardPalette.Add(Color.FromArgb(255, 182, 163, 119));
                _cardPalette.Add(Color.FromArgb(255, 205, 168, 63));
                _cardPalette.Add(Color.FromArgb(255, 218, 174, 52));
                _cardPalette.Add(Color.FromArgb(255, 221, 177, 53));
                _cardPalette.Add(Color.FromArgb(255, 255, 154, 1));
                _cardPalette.Add(Color.FromArgb(255, 255, 161, 18));
                _cardPalette.Add(Color.FromArgb(255, 235, 184, 44));
                _cardPalette.Add(Color.FromArgb(255, 228, 182, 52));
                _cardPalette.Add(Color.FromArgb(255, 247, 190, 36));
                _cardPalette.Add(Color.FromArgb(255, 200, 155, 94));
                _cardPalette.Add(Color.FromArgb(255, 192, 153, 104));
                _cardPalette.Add(Color.FromArgb(255, 223, 130, 103));
                _cardPalette.Add(Color.FromArgb(255, 215, 132, 118));
                _cardPalette.Add(Color.FromArgb(255, 201, 167, 68));
                _cardPalette.Add(Color.FromArgb(255, 196, 167, 87));
                _cardPalette.Add(Color.FromArgb(255, 209, 173, 70));
                _cardPalette.Add(Color.FromArgb(255, 212, 169, 91));
                _cardPalette.Add(Color.FromArgb(255, 213, 177, 73));
                _cardPalette.Add(Color.FromArgb(255, 198, 166, 102));
                _cardPalette.Add(Color.FromArgb(255, 196, 168, 123));
                _cardPalette.Add(Color.FromArgb(255, 219, 172, 112));
                _cardPalette.Add(Color.FromArgb(255, 219, 183, 106));
                _cardPalette.Add(Color.FromArgb(255, 217, 185, 115));
                _cardPalette.Add(Color.FromArgb(255, 255, 131, 91));
                _cardPalette.Add(Color.FromArgb(255, 249, 143, 109));
                _cardPalette.Add(Color.FromArgb(255, 227, 186, 108));
                _cardPalette.Add(Color.FromArgb(255, 229, 186, 112));
                _cardPalette.Add(Color.FromArgb(255, 255, 164, 123));
                _cardPalette.Add(Color.FromArgb(255, 207, 195, 127));
                _cardPalette.Add(Color.FromArgb(255, 253, 204, 94));
                _cardPalette.Add(Color.FromArgb(255, 235, 194, 108));
                _cardPalette.Add(Color.FromArgb(255, 233, 197, 117));
                _cardPalette.Add(Color.FromArgb(255, 252, 204, 104));
                _cardPalette.Add(Color.FromArgb(255, 249, 204, 115));
                _cardPalette.Add(Color.FromArgb(255, 251, 208, 106));
                _cardPalette.Add(Color.FromArgb(255, 253, 209, 117));
                _cardPalette.Add(Color.FromArgb(255, 128, 137, 140));
                _cardPalette.Add(Color.FromArgb(255, 136, 144, 145));
                _cardPalette.Add(Color.FromArgb(255, 151, 150, 130));
                _cardPalette.Add(Color.FromArgb(255, 144, 149, 148));
                _cardPalette.Add(Color.FromArgb(255, 137, 131, 165));
                _cardPalette.Add(Color.FromArgb(255, 137, 140, 180));
                _cardPalette.Add(Color.FromArgb(255, 140, 154, 168));
                _cardPalette.Add(Color.FromArgb(255, 133, 151, 180));
                _cardPalette.Add(Color.FromArgb(255, 149, 137, 167));
                _cardPalette.Add(Color.FromArgb(255, 149, 152, 188));
                _cardPalette.Add(Color.FromArgb(255, 138, 168, 188));
                _cardPalette.Add(Color.FromArgb(255, 175, 135, 148));
                _cardPalette.Add(Color.FromArgb(255, 169, 154, 129));
                _cardPalette.Add(Color.FromArgb(255, 187, 135, 134));
                _cardPalette.Add(Color.FromArgb(255, 178, 138, 148));
                _cardPalette.Add(Color.FromArgb(255, 184, 146, 156));
                _cardPalette.Add(Color.FromArgb(255, 168, 149, 173));
                _cardPalette.Add(Color.FromArgb(255, 170, 163, 131));
                _cardPalette.Add(Color.FromArgb(255, 184, 165, 128));
                _cardPalette.Add(Color.FromArgb(255, 181, 166, 157));
                _cardPalette.Add(Color.FromArgb(255, 162, 175, 186));
                _cardPalette.Add(Color.FromArgb(255, 189, 185, 170));
                _cardPalette.Add(Color.FromArgb(255, 130, 155, 206));
                _cardPalette.Add(Color.FromArgb(255, 134, 158, 208));
                _cardPalette.Add(Color.FromArgb(255, 146, 155, 196));
                _cardPalette.Add(Color.FromArgb(255, 142, 169, 193));
                _cardPalette.Add(Color.FromArgb(255, 140, 163, 209));
                _cardPalette.Add(Color.FromArgb(255, 153, 168, 199));
                _cardPalette.Add(Color.FromArgb(255, 148, 170, 213));
                _cardPalette.Add(Color.FromArgb(255, 157, 177, 216));
                _cardPalette.Add(Color.FromArgb(255, 164, 166, 193));
                _cardPalette.Add(Color.FromArgb(255, 171, 179, 198));
                _cardPalette.Add(Color.FromArgb(255, 167, 185, 220));
                _cardPalette.Add(Color.FromArgb(255, 181, 179, 205));
                _cardPalette.Add(Color.FromArgb(255, 181, 188, 211));
                _cardPalette.Add(Color.FromArgb(255, 173, 190, 224));
                _cardPalette.Add(Color.FromArgb(255, 178, 196, 217));
                _cardPalette.Add(Color.FromArgb(255, 174, 192, 224));
                _cardPalette.Add(Color.FromArgb(255, 174, 212, 224));
                _cardPalette.Add(Color.FromArgb(255, 184, 198, 227));
                _cardPalette.Add(Color.FromArgb(255, 185, 218, 229));
                _cardPalette.Add(Color.FromArgb(255, 202, 145, 142));
                _cardPalette.Add(Color.FromArgb(255, 196, 172, 134));
                _cardPalette.Add(Color.FromArgb(255, 203, 161, 158));
                _cardPalette.Add(Color.FromArgb(255, 203, 177, 134));
                _cardPalette.Add(Color.FromArgb(255, 207, 185, 151));
                _cardPalette.Add(Color.FromArgb(255, 210, 186, 132));
                _cardPalette.Add(Color.FromArgb(255, 215, 189, 148));
                _cardPalette.Add(Color.FromArgb(255, 213, 168, 167));
                _cardPalette.Add(Color.FromArgb(255, 234, 155, 132));
                _cardPalette.Add(Color.FromArgb(255, 253, 172, 140));
                _cardPalette.Add(Color.FromArgb(255, 193, 191, 193));
                _cardPalette.Add(Color.FromArgb(255, 206, 194, 135));
                _cardPalette.Add(Color.FromArgb(255, 214, 198, 136));
                _cardPalette.Add(Color.FromArgb(255, 219, 204, 145));
                _cardPalette.Add(Color.FromArgb(255, 218, 209, 141));
                _cardPalette.Add(Color.FromArgb(255, 215, 209, 154));
                _cardPalette.Add(Color.FromArgb(255, 231, 196, 135));
                _cardPalette.Add(Color.FromArgb(255, 225, 213, 143));
                _cardPalette.Add(Color.FromArgb(255, 231, 217, 148));
                _cardPalette.Add(Color.FromArgb(255, 251, 212, 129));
                _cardPalette.Add(Color.FromArgb(255, 255, 199, 172));
                _cardPalette.Add(Color.FromArgb(255, 202, 205, 219));
                _cardPalette.Add(Color.FromArgb(255, 193, 205, 230));
                _cardPalette.Add(Color.FromArgb(255, 200, 211, 233));
                _cardPalette.Add(Color.FromArgb(255, 211, 220, 237));
                _cardPalette.Add(Color.FromArgb(255, 213, 222, 240));
                _cardPalette.Add(Color.FromArgb(255, 201, 226, 228));
                _cardPalette.Add(Color.FromArgb(255, 201, 231, 242));
                _cardPalette.Add(Color.FromArgb(255, 216, 227, 232));
                _cardPalette.Add(Color.FromArgb(255, 219, 227, 241));
                _cardPalette.Add(Color.FromArgb(255, 228, 233, 237));
                _cardPalette.Add(Color.FromArgb(255, 229, 234, 244));
                _cardPalette.Add(Color.FromArgb(255, 236, 241, 248));
                _cardPalette.Add(Color.FromArgb(255, 240, 239, 243));
                _cardPalette.Add(Color.FromArgb(255, 253, 253, 254));
                _cardPalette.Add(Color.FromArgb(255, 255, 251, 240));
                _cardPalette.Add(Color.FromArgb(255, 160, 160, 164));
                _cardPalette.Add(Color.FromArgb(255, 128, 128, 128));
                _cardPalette.Add(Color.FromArgb(255, 255, 0, 0));
                _cardPalette.Add(Color.FromArgb(255, 0, 255, 0));
                _cardPalette.Add(Color.FromArgb(255, 255, 255, 0));
                _cardPalette.Add(Color.FromArgb(255, 0, 0, 255));
                _cardPalette.Add(Color.FromArgb(255, 255, 0, 255));
                _cardPalette.Add(Color.FromArgb(255, 0, 255, 255));
                _cardPalette.Add(Color.FromArgb(255, 255, 255, 255));
                #endregion
            }
            return _cardPalette;
        }
    }
}