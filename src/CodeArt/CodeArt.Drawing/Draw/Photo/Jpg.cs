using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace CodeArt.Drawing
{
    public class Jpg:IPhotoHandler
    {
        private Jpg() { }

        /// <summary>
        /// �����ģʽ����ͼƬ
        /// </summary>
        public virtual void Fit(Stream sourceStream, Stream ouputStream, int width, int height, bool highQuality)
        {
            Image source = null;
            Graphics gdiobj = null;
            try
            {
                source = Image.FromStream(sourceStream);
                if (source.Width == width && source.Height == height)
                {
                    //ԭͼ���
                    source.Save(ouputStream, source.RawFormat);
                    return;
                }


                if (width == 0 || (width > 500 && width > source.Width)) width = source.Width;//��500������,ʵ���ļ�����500�ģ�����ʵ��Ϊ��С
                if (height == 0 || (height > 500 && height > source.Height)) height = source.Height;//��500������,ʵ���ļ�����500�ģ�����ʵ��Ϊ��С

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

                using (Bitmap img = new Bitmap(width, height))
                {
                    //�������ֻ�ڴ�ӡЧ���йأ���ͼ������Ļ�ϵ���ʾЧ���޹ء�
                    img.SetResolution(72f, 72f);
                    img.SetResolution(source.HorizontalResolution, source.VerticalResolution);
                    gdiobj = Graphics.FromImage(img);
                    gdiobj.CompositingQuality = CompositingQuality.HighQuality;
                    gdiobj.SmoothingMode = SmoothingMode.HighQuality;
                    gdiobj.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gdiobj.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    gdiobj.FillRectangle(new SolidBrush(Color.White), 0, 0, width, height);
                    Rectangle destrect = new Rectangle(x, y, tw, th);
                    gdiobj.DrawImage(source, destrect, 0, 0, source.Width,
                                    source.Height, GraphicsUnit.Pixel);

                    using (System.Drawing.Imaging.EncoderParameters ep = new System.Drawing.Imaging.EncoderParameters(1))
                    {
                        ep.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)100);
                        if (!highQuality)
                        {
                            img.Save(ouputStream, ImageFormat.Jpeg);
                        }
                        else
                        {
                            System.Drawing.Imaging.ImageCodecInfo ici = GetEncoderInfo("image/jpeg");
                            if (ici != null)
                            {
                                img.Save(ouputStream, ici, ep);
                            }
                            else
                            {
                                img.Save(ouputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if(source != null) source.Dispose();
                if(gdiobj!=null)  gdiobj.Dispose();
            }
        }

        public virtual void Cover(Stream sourceStream, Stream ouputStream, int width, int height, bool highQuality)
        {
            Image source = null;
            Graphics gdiobj = null;
            System.Drawing.Imaging.EncoderParameters ep = null;
            Bitmap img = null;
            try
            {
                source = System.Drawing.Image.FromStream(sourceStream);
                if (source.Width == width && source.Height == height)
                {
                    //ԭͼ���
                    source.Save(ouputStream, source.RawFormat);
                    return;
                }

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

                //���ݼ����ֵ����ʼ�ü�
                using (Bitmap bitmap = new Bitmap(cutWidth, cutHeight, PixelFormat.Format32bppArgb))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        Rectangle destRect = new Rectangle(0, 0, cutWidth, cutHeight);
                        g.DrawImage(source, destRect, x, y, cutWidth, cutHeight, GraphicsUnit.Pixel);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bitmap.Save(ms, ImageFormat.Jpeg);
                            source.Dispose();
                            source = Image.FromStream(ms);

                            //������
                            ImageZip(cutWidth, cutHeight, ref width, ref height);

                            img = new Bitmap(width, height);
                            //img.SetResolution(72f, 72f);
                            img.SetResolution(source.HorizontalResolution, source.VerticalResolution);
                            gdiobj = Graphics.FromImage(img);
                            gdiobj.CompositingQuality = CompositingQuality.HighQuality;
                            gdiobj.SmoothingMode = SmoothingMode.HighQuality;
                            gdiobj.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            gdiobj.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            gdiobj.FillRectangle(new SolidBrush(Color.White), 0, 0, width, height);
                            Rectangle destrect = new Rectangle(0, 0, width, height);
                            gdiobj.DrawImage(source, destrect, 0, 0, source.Width,
                                            source.Height, GraphicsUnit.Pixel);
                            ep = new System.Drawing.Imaging.EncoderParameters(1);
                            ep.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)100);
                            if (!highQuality)
                            {
                                img.Save(ouputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                            else
                            {
                                System.Drawing.Imaging.ImageCodecInfo ici = GetEncoderInfo("image/jpeg");
                                if (ici != null)
                                {
                                    img.Save(ouputStream, ici, ep);
                                }
                                else
                                {
                                    img.Save(ouputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (source != null) source.Dispose();
                if (gdiobj != null) gdiobj.Dispose();
                if (ep != null) ep.Dispose();
                if (img != null) img.Dispose();
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
            Graphics gdiobj = null;
            System.Drawing.Imaging.EncoderParameters ep = null;
            Bitmap img = null;
            try
            {
                source = System.Drawing.Image.FromStream(sourceStream);
                if (source.Width == width && source.Height == height)
                {
                    //ԭͼ���
                    source.Save(ouputStream, source.RawFormat);
                    return;
                }

                ImageZip(source.Width, source.Height, ref width, ref height);
                img = new Bitmap(width, height);
                //img.SetResolution(72f, 72f);
                img.SetResolution(source.HorizontalResolution, source.VerticalResolution);
                gdiobj = Graphics.FromImage(img);
                gdiobj.CompositingQuality = CompositingQuality.HighQuality;
                gdiobj.SmoothingMode = SmoothingMode.HighQuality;
                gdiobj.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gdiobj.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gdiobj.FillRectangle(new SolidBrush(Color.White), 0, 0, width, height);
                Rectangle destrect = new Rectangle(0, 0, width, height);
                gdiobj.DrawImage(source, destrect, 0, 0, source.Width,
                                source.Height, GraphicsUnit.Pixel);
                ep = new System.Drawing.Imaging.EncoderParameters(1);
                ep.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)100);
                if (!highQuality)
                {
                    img.Save(ouputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                else
                {
                    System.Drawing.Imaging.ImageCodecInfo ici = GetEncoderInfo("image/jpeg");
                    if (ici != null)
                    {
                        img.Save(ouputStream, ici, ep);
                    }
                    else
                    {
                        img.Save(ouputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (source != null) source.Dispose();
                if (gdiobj != null) gdiobj.Dispose();
                if (ep != null) ep.Dispose();
                if (img != null) img.Dispose();
            }

        }

        public virtual void Part(Stream sourceStream, Stream ouputStream, int width, int height,bool highQuality)
        {
            throw new ApplicationException("Jpg.ThumbByPart��δʵ�֣�");
        }

        public ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        public static Jpg Instance = new Jpg();

    }
}
