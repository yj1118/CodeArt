using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using System.Drawing;
using System.Windows.Interop;
using System.Windows;
using System.Threading;

using Acrobat;
using CodeArt.Drawing;
using System.Windows.Media.Imaging;
using CodeArt.WPF.Screen;

namespace CodeArt.Office
{
    public class Pdf : DocumentBase
    {
        protected string _fileName;
        private CAcroPDDoc _document = null;
        private CAcroRect _rect = null;

        public Pdf(string fileName)
        {
            _fileName = fileName;
            _document = (CAcroPDDoc)Interaction.CreateObject("AcroExch.PDDoc", "");
            if (!_document.Open(fileName)) { throw new FileNotFoundException(fileName); }
            _rect = (CAcroRect)Microsoft.VisualBasic.Interaction.CreateObject("AcroExch.Rect", "");
        }

        public override int PageCount
        {
            get
            {
                return _document.GetNumPages();
            }
        }

        /// <summary>
        /// 当pdf内容只有一张时，可以使用该方法提取图片
        /// </summary>
        /// <returns></returns>
        public Stream ExtractImage()
        {
            return ExtractImage(0);
        }


        protected override Stream ExtractImage(int pageIndex)
        {
            var stream = new MemoryStream();
            UsingImage(pageIndex, (image) =>
            {
                image.Save(stream, ImageFormat.Png);
            });
            return stream;
        }

        /// <summary>
        /// 当pdf内容只有一张时，可以使用该方法保存图片
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveImage(string fileName)
        {
            SaveImage(0, fileName);
        }

        protected override void SaveImage(int pageIndex, string fileName)
        {
            UsingImage(pageIndex, (image) =>
             {
                 image.Save(fileName, ImageFormat.Png);
             });
        }

        /// <summary>
        /// 核心代码是从网上找的，但是那些代码是运行于winform环境下的剪切板，要在WPF下，需要改进，已经改进完毕
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="action"></param>
        private void UsingImage(int pageIndex, Action<Bitmap> action)
        {
            var page = (CAcroPDPage)_document.AcquirePage(pageIndex);
            try
            {
                var point = (CAcroPoint)page.GetSize();

                //WPF下，需要转换逻辑像素到物理像素
                //double resolution = SystemScreen.DPI.ScalingY; //设置图片的分辨率，数字越大越清晰，默认值为1
                double resolution =1.5;
                int imgWidth = (int)((double)point.x * resolution);
                int imgHeight = (int)((double)point.y * resolution);

                _rect.Left = 0;
                _rect.right = (short)imgWidth;
                _rect.Top = 0;
                _rect.bottom = (short)imgHeight;

                using (MemoryStream ms = GetStream(page, resolution))
                {
                    using (var image = new Bitmap(ms))
                    {
                        action(image);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (page != null)
                {
                    Marshal.ReleaseComObject(page);
                }
            }
        }

        private MemoryStream GetStream(CAcroPDPage page, double resolution)
        {
            MemoryStream ms = new MemoryStream();
            Thread th = new Thread(new ThreadStart(delegate ()
            {
                /* 以下剪贴板操作仅在wpf下工作 */
                // Render to clipboard, scaled by 100 percent (ie. original size)
                // Even though we want a smaller image, better for us to scale in .NET
                // than Acrobat as it would greek out small text
                page.CopyToClipboard(_rect, 0, 0, (short)(100 * resolution));
                var clipboardData = Clipboard.GetDataObject();
                if (clipboardData.GetDataPresent(DataFormats.Bitmap))
                {
                    var source = (InteropBitmap)clipboardData.GetData(DataFormats.Bitmap);
                    BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(source));
                    encoder.Save(ms);
                }

            }));
            th.TrySetApartmentState(ApartmentState.STA);
            th.Start();
            th.Join();
            return ms;
        }

        public override void Dispose()
        {
            if (_rect != null)
            {
                Marshal.ReleaseComObject(_rect);
            }

            if (_document != null)
            {
                _document.Close();
                Marshal.ReleaseComObject(_document);
            }

        }
    }
}
