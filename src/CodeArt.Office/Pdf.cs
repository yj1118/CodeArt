using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Aspose.Pdf;
using Aspose.Pdf.Devices;

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
    }
}
