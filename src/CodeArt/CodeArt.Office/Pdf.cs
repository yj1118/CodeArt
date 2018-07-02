using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Aspose.Pdf;
using Aspose.Pdf.Devices;
using CodeArt.Drawing;

namespace CodeArt.Office
{
    public class Pdf : DocumentBase
    {
        private string _fileName;
        private Document _document;
        private PngDevice _device;

        public Pdf(string fileName)
        {
            _fileName = fileName;
            _document = new Document(_fileName);
            InitDevice();
        }

        private void InitDevice()
        {
            //Width, Height, Resolution, Quality
            //Quality [0-100], 100 is Maximum
            //create Resolution object
            Resolution resolution = new Resolution(170);
            //JpegDevice jpegDevice = new JpegDevice(500, 700, resolution,100);
            _device = new PngDevice(resolution);
        }

        public override int PageCount
        {
            get
            {
                return _document.Pages.Count;
            }
        }

        protected override Stream ExtractImage(int pageIndex)
        {
            var stream = new MemoryStream();
            _device.Process(_document.Pages[pageIndex + 1], stream);
            return stream;
        }

        public override void Dispose()
        {
            _document.Dispose();
        }


        #region 图片转pdf


        /// <summary>
        /// 将图片转换为pdf
        /// </summary>
        /// <param name="images"></param>
        /// <param name="outputFileName"></param>
        public static void Convert(IEnumerable<string> images, string outputFileName,Action<float> progressCallback)
        {
            if(File.Exists(outputFileName))
            {
                File.Delete(outputFileName);
            }

            var progress = 0F;
            var total = images.Count();

            {
                if (total > 0)
                {
                    var first = images.First();
                    WriteImage(outputFileName, first, true);
                }

                progress = 1F / total;
                progressCallback(progress);
            }

            float index = 0F;
            foreach (var fileName in images)
            {
                if (index == 0)
                {
                    index++;
                    continue;
                }

                WriteImage(outputFileName, fileName, false);

                index++;

                progress = index / total;
                progressCallback(progress);
            }
        }

        private static void WriteImage(string outputFileName, string imageFileName,bool create)
        {
            if(create)
            {
                using (Document doc = new Document())
                {
                    AddImage(doc, imageFileName, ()=>
                    {
                        doc.Save(outputFileName);
                    });
                }
            }
            else
            {
                using (var fs = new FileStream(outputFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    using (Document doc = new Document(fs))
                    {
                        AddImage(doc, imageFileName, () =>
                        {
                            doc.Save();
                        });
                    }
                }
            }
        }

        private static void AddImage(Document doc,string imageFileName,Action save)
        {
            Page page = doc.Pages.Add();
            using (var stream = new FileStream(imageFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var g = stream.GetImage())
                {
                    var width = g.Width;
                    var height = g.Height;

                    page.Rect = new Rectangle(0, 0, width, height);
                }

                Image image = new Image();
                image.ImageStream = stream;

                page.AddImage(imageFileName, page.Rect);

                save();
            }
        }

        #endregion

    }
}
