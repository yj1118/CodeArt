using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using CodeArt.IO;
using CodeArt.Util;

using MSWord = Microsoft.Office.Interop.Word;
using MSPowerPoint = Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;

namespace CodeArt.Office
{
    public class PPT : DocumentBase
    {
        private string _fileName;
        private MSPowerPoint.ApplicationClass _application;
        private MSPowerPoint.Presentation _presentation;
        private string _tempFileName;

        public PPT(string fileName)
        {
            _fileName = fileName;
            _application = new MSPowerPoint.ApplicationClass();
            _presentation = _application.Presentations.Open(fileName, MsoTriState.msoTrue, MsoTriState.msoTrue, MsoTriState.msoFalse);
            _tempFileName = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), string.Format("{0}.png", Guid.NewGuid().ToString("N")));
        }

        public override int PageCount
        {
            get
            {
                return _presentation.Slides.Count;
            }
        }

        protected override Stream ExtractImage(int pageIndex)
        {
            IOUtil.Delete(_tempFileName);

            var stream = new MemoryStream();
            var slide = _presentation.Slides[pageIndex + 1]; //_presentation.Slides的下标是从1开始的
            slide.Export(_tempFileName, "PNG");
            using (var fs = new FileStream(_tempFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var bytes = fs.GetBytes(5120);
                stream.Write(bytes, 0, bytes.Length);
            }
            return stream;
        }

        protected override void SaveImage(int pageIndex, string fileName)
        {
            var slide = _presentation.Slides[pageIndex + 1]; //_presentation.Slides的下标是从1开始的
            var extension = IOUtil.GetExtension(fileName);
            slide.Export(fileName, extension);
        }


        public override void Dispose()
        {
            _presentation.Close();
            Marshal.ReleaseComObject(_presentation);

            _application.Quit();
            Marshal.ReleaseComObject(_application);
        }

    }
}
