using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CodeArt.Drawing;

namespace CodeArt.Drawing.GIF
{
    public class GifHelper
    {
        public  void WaterMark(string gifFilePath, string text,string outputPath)
        {
            if (!File.Exists(gifFilePath))
            {
                throw new IOException(string.Format("文件{0}不存在!",gifFilePath));
            }
            Bitmap ora_Img = new Bitmap(gifFilePath);
            if (ora_Img.RawFormat.Guid != ImageFormat.Gif.Guid)
            {
                throw new IOException(string.Format("文件{0}!", gifFilePath));
            }          
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
                        Bitmap img = Image.FromHbitmap(ora_Img.GetHbitmap()) as Bitmap;
                        Color backColor = img.GetPixel(1, 1);//                       
                        Font font = new Font(new FontFamily("宋体"), 15.0f, FontStyle.Bold);
                        Graphics g = Graphics.FromImage(img);
                        g.DrawString(text, font, Brushes.BlanchedAlmond, new PointF(10.0f, 10.0f));
                        g.Dispose();
                        Frame frame = new Frame(img, delay,backColor);
                        frames.Add(frame);
                    }
                }
            }
            AnimatedGifEncoder gif = new AnimatedGifEncoder();
            gif.Start(outputPath);
            gif.SetRepeat(0);
            for (int i = 0; i < frames.Count; i++)
            {               
                gif.SetDelay(frames[i].Delay);             
                gif.AddFrame(frames[i].Img);                    
            }
            gif.Finish();  
        }

        public void Thumbnail(string gifFilePath, int width, string outputPath)
        {
            if (!File.Exists(gifFilePath))
            {
                throw new IOException(string.Format("文件{0}不存在!", gifFilePath));
            }
            Bitmap ora_Img = new Bitmap(gifFilePath);
            if (ora_Img.RawFormat.Guid != ImageFormat.Gif.Guid)
            {
                throw new IOException(string.Format("文件{0}!", gifFilePath));
            }
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
                        Bitmap img = Image.FromHbitmap(ora_Img.GetHbitmap()) as Bitmap;
                        int ora_Width = img.Width;
                        int height = img.Height * width / ora_Width;
                        img = img.GetThumbnailImage(width, height,null, new System.IntPtr()) as Bitmap;
                        Color backColor = img.GetPixel(1, 1);//      
                        Frame frame = new Frame(img, delay, backColor);
                        frames.Add(frame);
                    }
                }
            }          
            AnimatedGifEncoder gif = new AnimatedGifEncoder();
            gif.Start(outputPath);
            gif.SetRepeat(0);
            for (int i = 0; i < frames.Count; i++)
            {              
                gif.SetDelay(frames[i].Delay);
                gif.AddFrame(frames[i].Img);
            }
            gif.Finish();
        }
    }
}
