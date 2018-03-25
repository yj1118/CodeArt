using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Aspose.Words;
using Aspose.Words.Saving;

namespace CodeArt.Office
{
    public class Word : DocumentBase
    {
        private string _fileName;
        private Document _document;

        public Word(string fileName)
        {
            _fileName = fileName;
            _document = new Document(_fileName);
        }

        public override int PageCount
        {
            get
            {
                return _document.PageCount;
            }
        }

        protected override Stream ExtractImage(int pageIndex)
        {
            ImageSaveOptions option = new ImageSaveOptions(SaveFormat.Png);
            option.PageIndex = pageIndex;
            option.PageCount = 1;
            option.Resolution = 170; //图片分辨率，单位为每英寸像素点数
            option.UseHighQualityRendering = true;
            var stream = new MemoryStream();
            _document.Save(stream, option);
            return stream;
        }

        public override void Dispose()
        {
        }

    }
}
