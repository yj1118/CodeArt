using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Imaging;

using CodeArt.IO;
using CodeArt.Util;

using Aspose.Cells;
using Aspose.Cells.Rendering;

namespace CodeArt.Office
{
    public class Excel : DocumentBase
    {
        private string _fileName;
        private Workbook _book;
        private ImageOrPrintOptions _options;

        public Excel(string fileName)
        {
            _fileName = fileName;
            _book = new Workbook(_fileName);
            _options = GetOptions();
        }

        private ImageOrPrintOptions GetOptions()
        {
            ImageOrPrintOptions options = new ImageOrPrintOptions();
            options.ImageFormat = ImageFormat.Png;
            //options.OnePagePerSheet = true;  开启此参数在某些情况下会报错，所以屏蔽了该参数
            options.PrintingPage = PrintingPageType.IgnoreBlank;
            return options;
        }

        public override int PageCount
        {
            get
            {
                return _book.Worksheets.Count;
            }
        }

        protected override Stream ExtractImage(int pageIndex)
        {
            var stream = new MemoryStream();
            Worksheet sheet = _book.Worksheets[pageIndex];
            sheet.PageSetup.LeftMargin = 1;
            sheet.PageSetup.RightMargin = 1;
            sheet.PageSetup.BottomMargin = 1;
            sheet.PageSetup.TopMargin = 1;

            SheetRender sr = new SheetRender(sheet, _options);
            sr.ToImage(0, stream);
            return stream;
        }

        public override void Dispose()
        {
            _book.Dispose();
        }

    }
}
