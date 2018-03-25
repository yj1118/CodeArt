using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Imaging;

using CodeArt.IO;
using CodeArt.Util;

using Aspose.Slides;
using Aspose.Slides.Export;

namespace CodeArt.Office
{
    public class PPT : DocumentBase
    {
        private string _fileName;
        private Presentation _presentation;

        public PPT(string fileName)
        {
            _fileName = fileName;
            _presentation = new Presentation(_fileName);
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
            var stream = new MemoryStream();
            _presentation.Save(stream, new int[] { pageIndex + 1 }, SaveFormat.Tiff);
            return stream;
        }

        public override void Dispose()
        {
            _presentation.Dispose();
        }

    }
}
