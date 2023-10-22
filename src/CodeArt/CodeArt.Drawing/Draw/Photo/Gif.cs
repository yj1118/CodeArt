using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace CodeArt.Drawing
{
    public class Gif:IPhotoHandler
    {
        private Gif() { }

        /// <summary>
        /// ���û�������
        /// </summary>
        /// <param name="g"></param>
        protected virtual void SetQuality(Graphics g)
        {
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.InterpolationMode = InterpolationMode.Low;
            g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
        }

        /// <summary>
        /// �����ģʽ����ͼƬ
        /// </summary>
        public virtual void Fit(Stream sourceStream, Stream ouputStream, int width, int height, bool highQuality)
        {
            Image source = Image.FromStream(sourceStream);

            if (source.RawFormat.Guid != ImageFormat.Gif.Guid)
                throw new IOException(string.Format("�ļ�����GIF��ʽ��ͼƬ!"));

            if (width == 0 || (width > 200 && width > source.Width)) width = source.Width;//��200������,ʵ���ļ�����200�ģ�����ʵ��Ϊ��С
            if (height == 0 || (height > 200 && height > source.Height)) height = source.Height;//��200������,ʵ���ļ�����200�ģ�����ʵ��Ϊ��С

            int x = 0, y = 0, tw, th;

            //�ȵȱ�����
            if (source.Width > source.Height)
            {
                //����ڸߣ��Կ�ȱ�����
                tw = source.Width > width ? width : source.Width;
                th = (int)(source.Height * (double)tw / (double)source.Width);

                if (th > height)//�õ��Ľ������Ҫ�ĸ߶ȸߣ�����ݸ߶�����
                {
                    th = source.Height > height ? height : source.Height;
                    tw = (int)(source.Width * (double)th / (double)source.Height);
                }
            }
            else
            {
                //�ߴ��ڿ��Ըߵȱ�����
                th = source.Height > height ? height : source.Height;
                tw = (int)(source.Width * (double)th / (double)source.Height);

                if (tw > width)//�õ��Ľ������Ҫ�Ŀ�ȿ�����ݿ������
                {
                    tw = source.Width > width ? width : source.Width;
                    th = (int)(source.Height * (double)tw / (double)source.Width);
                }
            }

            if (tw < width)
                x = (width - tw) / 2;
            if (th < height)
                y = (height - th) / 2;

            sourceStream.Seek(0, SeekOrigin.Begin);

            Bitmap ora_Img = new Bitmap(sourceStream);
            List<Frame> frames = new List<Frame>();
            foreach (Guid guid in ora_Img.FrameDimensionsList)
            {
                FrameDimension frameDimension = new FrameDimension(guid);
                int frameCount = ora_Img.GetFrameCount(frameDimension);
                byte[] buffer = ora_Img.GetPropertyItem(20736).Value;
                for (int i = 0; i < frameCount; i++)
                {
                    if (ora_Img.SelectActiveFrame(frameDimension, i) == 0)
                    {
                        int delay = BitConverter.ToInt32(buffer, i * 4);
                        System.IO.MemoryStream stream = new System.IO.MemoryStream();

                        source = Image.FromHbitmap(ora_Img.GetHbitmap()) as Bitmap;
                        Color backColor = Color.White;
                        Bitmap img = new Bitmap(width, height);
                        img.SetResolution(72f, 72f);
                        img.SetResolution(source.HorizontalResolution, source.VerticalResolution);
                        using (Graphics gdiobj = Graphics.FromImage(img))
                        {
                            SetQuality(gdiobj);
                            gdiobj.FillRectangle(new SolidBrush(Color.White), 0, 0, width, height);
                            Rectangle destrect = new Rectangle(x, y, tw, th);
                            gdiobj.DrawImage(source, destrect, 0, 0, source.Width,
                                            source.Height, GraphicsUnit.Pixel);

                        }
                        Frame frame = new Frame(img, delay, backColor);
                        frames.Add(frame);
                    }
                }
            }
            AnimatedGifEncoder gif = new AnimatedGifEncoder();
            gif.Start(ouputStream);
            gif.SetRepeat(0);
            for (int i = 0; i < frames.Count; i++)
            {
                gif.SetDelay(frames[i].Delay);
                gif.AddFrame(frames[i].Img);
            }
            gif.Finish();
        }

        public virtual void Cover(Stream sourceStream, Stream ouputStream, int width, int height, bool highQuality)
        {
            Image source = null;
            try
            {
                source = System.Drawing.Image.FromStream(sourceStream);

                //�ȵȱȲü�
                int tw = width;
                int th = height;
                if (tw == 0 && th != 0)
                {
                    tw = (int)(source.Width * (double)th / source.Height);
                }
                else if (th == 0 && tw != 0)
                {
                    th = (int)(source.Height * (double)width / source.Width);
                }
                else if (th == 0 && tw == 0)
                {
                    th = source.Height;
                    tw = source.Width;
                }

                int cutWidth = 0;
                int cutHeight = 0;

                int bywWidth = source.Width;
                int bywHeight = (int)(source.Width * (double)th / tw);
                if (bywHeight < 1) bywHeight = 1;

                int byhWidth = (int)(source.Height * (double)tw / th);
                if (byhWidth < 1) byhWidth = 1;
                int byhHeight = source.Height;


                if (bywHeight > source.Height)//������ݿ�Ȳü��õ��ĸ߶ȳ���ԭʼ�߶�
                {
                    cutWidth = byhWidth;
                    cutHeight = byhHeight;
                }
                else if (byhWidth > source.Width) //������ݸ߶Ȳü��õ��Ŀ�ȸ߹�ԭʼ���
                {
                    cutWidth = bywWidth;
                    cutHeight = bywHeight;
                }
                else
                {
                    //����������ݶ����ϣ����������Ĵ�С�жϣ����Խ�󣬾�ȷ��Խ��
                    if ((bywWidth * bywHeight) > (byhWidth * byhHeight))
                    {
                        cutWidth = bywWidth;
                        cutHeight = bywHeight;
                    }
                    else
                    {
                        cutWidth = byhWidth;
                        cutHeight = byhHeight;
                    }
                }

                //�������ĵ㣬�õ��ü�����ʼ��ַ
                int x = (int)((double)source.Width / 2 - (double)cutWidth / 2);
                int y = (int)((double)source.Height / 2 - (double)cutHeight / 2);
                if (x < 0) x = 0;
                if (y < 0) y = 0;


                sourceStream.Seek(0, SeekOrigin.Begin);

                Bitmap ora_Img = new Bitmap(sourceStream);
                List<Frame> frames = new List<Frame>();
                foreach (Guid guid in ora_Img.FrameDimensionsList)
                {
                    FrameDimension frameDimension = new FrameDimension(guid);
                    int frameCount = ora_Img.GetFrameCount(frameDimension);
                    byte[] buffer = ora_Img.GetPropertyItem(20736).Value;
                    for (int i = 0; i < frameCount; i++)
                    {
                        if (ora_Img.SelectActiveFrame(frameDimension, i) == 0)
                        {
                            int delay = BitConverter.ToInt32(buffer, i * 4);
                            System.IO.MemoryStream stream = new System.IO.MemoryStream();

                            source = Image.FromHbitmap(ora_Img.GetHbitmap()) as Bitmap;
                            Color backColor = Color.White;
                            Bitmap img = new Bitmap(cutWidth, cutHeight, PixelFormat.Format32bppArgb);

                            using (Graphics g = Graphics.FromImage(img))
                            {
                                Rectangle destRect = new Rectangle(0, 0, cutWidth, cutHeight);
                                g.DrawImage(source, destRect, x, y, cutWidth, cutHeight, GraphicsUnit.Pixel);
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    img.Save(ms, ImageFormat.Jpeg);
                                    source.Dispose();
                                    source = Image.FromStream(ms);

                                    //������
                                    ImageZip(cutWidth, cutHeight, ref width, ref height);

                                    img = new Bitmap(width, height);
                                    //img.SetResolution(72f, 72f);
                                    img.SetResolution(source.HorizontalResolution, source.VerticalResolution);
                                    using (Graphics gdiobj = Graphics.FromImage(img))
                                    {
                                        SetQuality(gdiobj);
                                        gdiobj.FillRectangle(new SolidBrush(Color.White), 0, 0, width, height);
                                        Rectangle destrect = new Rectangle(0, 0, width, height);
                                        gdiobj.DrawImage(source, destrect, 0, 0, source.Width,
                                                        source.Height, GraphicsUnit.Pixel);
                                    }
                                }
                            }
                            Frame frame = new Frame(img, delay, backColor);
                            frames.Add(frame);
                        }
                    }
                }
                AnimatedGifEncoder gif = new AnimatedGifEncoder();
                gif.Start(ouputStream);
                gif.SetRepeat(0);
                for (int i = 0; i < frames.Count; i++)
                {
                    gif.SetDelay(frames[i].Delay);
                    gif.AddFrame(frames[i].Img);
                }
                gif.Finish();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                source.Dispose();
            }
        }

        private void ImageZip(int sourceWidth, int sourceHeight, ref int Newzipwidth, ref int Newzipheight)
        {
            int Originalwidth = sourceWidth;
            int Originalheight = sourceHeight;
            int zipwidth = 0;
            int zipheight = 0;
            if ((Newzipwidth > 0) && (Newzipheight > 0))
            {
                zipwidth = Newzipwidth;
                zipheight = Newzipheight;
            }
            else if ((Newzipwidth > 0) && (Newzipheight == 0))//����ȵȱ�����������ŵĸ߶�
            {
                double num = (Newzipwidth * Originalheight) / Originalwidth;
                zipwidth = Newzipwidth;
                zipheight = (int)num;
            }
            else if ((Newzipwidth == 0) && (Newzipheight > 0))//���߶ȵȱ�����������ŵĿ��
            {
                double num2 = (Originalwidth * Newzipheight) / Originalheight;
                zipwidth = (int)num2;
                zipheight = Newzipheight;
            }
            else
            {
                zipwidth = Originalwidth;
                zipheight = Originalheight;
            }

            if (zipwidth > sourceWidth) zipwidth = sourceWidth;
            if (zipheight > sourceHeight) zipheight = sourceHeight;
            Newzipwidth = zipwidth;
            Newzipheight = zipheight;
        }

        /// <summary>
        /// ����ȫͼ�����ţ�����ʹͼ����Σ�
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <param name="ouputStream"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="format"></param>
        /// <param name="highQuality"></param>
        public virtual void Stetch(Stream sourceStream, Stream ouputStream, int width, int height, bool highQuality)
        {
            Image source = null;
            try
            {
                source = System.Drawing.Image.FromStream(sourceStream);

                ImageZip(source.Width, source.Height, ref width, ref height);
                
                sourceStream.Seek(0, SeekOrigin.Begin);

                Bitmap ora_Img = new Bitmap(sourceStream);
                List<Frame> frames = new List<Frame>();
                foreach (Guid guid in ora_Img.FrameDimensionsList)
                {
                    FrameDimension frameDimension = new FrameDimension(guid);
                    int frameCount = ora_Img.GetFrameCount(frameDimension);
                    byte[] buffer = ora_Img.GetPropertyItem(20736).Value;
                    for (int i = 0; i < frameCount; i++)
                    {
                        if (ora_Img.SelectActiveFrame(frameDimension, i) == 0)
                        {
                            int delay = BitConverter.ToInt32(buffer, i * 4);
                            System.IO.MemoryStream stream = new System.IO.MemoryStream();

                            source = Image.FromHbitmap(ora_Img.GetHbitmap()) as Bitmap;
                            Color backColor = Color.White;
                            Bitmap img = new Bitmap(width, height);

                            //img.SetResolution(72f, 72f);
                            img.SetResolution(source.HorizontalResolution, source.VerticalResolution);
                            using (Graphics gdiobj = Graphics.FromImage(img))
                            {
                                SetQuality(gdiobj);
                                gdiobj.FillRectangle(new SolidBrush(Color.White), 0, 0, width, height);
                                Rectangle destrect = new Rectangle(0, 0, width, height);
                                gdiobj.DrawImage(source, destrect, 0, 0, source.Width,
                                                source.Height, GraphicsUnit.Pixel);
                            }

                            Frame frame = new Frame(img, delay, backColor);
                            frames.Add(frame);
                        }
                    }
                }
                AnimatedGifEncoder gif = new AnimatedGifEncoder();
                gif.Start(ouputStream);
                gif.SetRepeat(0);
                for (int i = 0; i < frames.Count; i++)
                {
                    gif.SetDelay(frames[i].Delay);
                    gif.AddFrame(frames[i].Img);
                }
                gif.Finish();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                source.Dispose();
            }

        }

        public virtual void Part(Stream sourceStream, Stream ouputStream, int width, int height,bool highQuality)
        {
            throw new ApplicationException("Gif.ThumbByPart��δʵ�֣�");
        }

        public static Gif Instance = new Gif();

    }
}
