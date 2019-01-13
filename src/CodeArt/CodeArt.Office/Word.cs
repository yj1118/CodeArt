
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MSWord = Microsoft.Office.Interop.Word;
using System.Runtime.InteropServices;
using CodeArt.IO;
using System.Reflection;
using CodeArt.Drawing;
using Microsoft.Office.Interop.Word;

namespace CodeArt.Office
{
    public class Word : DocumentBase
    {
        private string _fileName;
        private MSWord.ApplicationClass _application;
        private MSWord.Document _wordDocument;
        private string _tempFileName;
        private string _tempPdfFileName;

        public Word(string fileName)
        {
            _fileName = fileName;

            _application = new MSWord.ApplicationClass();
            _application.Visible = false;

            _wordDocument = _application.Documents.Open(_fileName, false, true);

            _tempFileName = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), string.Format("{0}.png", Guid.NewGuid().ToString("N")));
            _tempPdfFileName = string.Format("{0}.pdf", _tempFileName);
        }

        public override int PageCount
        {
            get
            {
                //return _wordDocument.ComputeStatistics(MSWord.WdStatistic.wdStatisticPages);
                return _wordDocument.Range().ComputeStatistics(MSWord.WdStatistic.wdStatisticPages);
            }
        }

        protected override Stream ExtractImage(int pageIndex)
        {
            SaveAsPDF(_wordDocument, _tempPdfFileName, pageIndex + 1);

            Stream stream = null;
            using (var pdf = new Pdf(_tempPdfFileName))
            {
                stream = pdf.ExtractImage();
            }
            return stream;
        }

        protected override void SaveImage(int pageIndex, string fileName)
        {
            SaveAsPDF(_wordDocument, _tempPdfFileName, pageIndex + 1);

            using (var pdf = new Pdf(_tempPdfFileName))
            {
                pdf.SaveImage(fileName);
            }
        }

        private static void SaveAsPDF(MSWord.Document document, string fileName, int pageIndex)
        {
            IOUtil.Delete(fileName);
            document.ExportAsFixedFormat(fileName, MSWord.WdExportFormat.wdExportFormatPDF, false, 
                MSWord.WdExportOptimizeFor.wdExportOptimizeForPrint, MSWord.WdExportRange.wdExportFromTo, pageIndex, pageIndex);
        }

        public override void Dispose()
        {
            _wordDocument.Close(false);
            Marshal.ReleaseComObject(_wordDocument);

            _application.Quit(false);
            Marshal.ReleaseComObject(_application);
        }


        #region 图片转word


        /// <summary>
        /// 将图片转换为pdf
        /// </summary>
        /// <param name="images"></param>
        /// <param name="outputFileName"></param>
        public static void ConvertPDF(IEnumerable<string> images, string outputFileName, Action<float> progressCallback)
        {
            if (File.Exists(outputFileName))
            {
                File.Delete(outputFileName);
            }

            WriteImages(outputFileName, images, progressCallback);
        }

        private static void WriteImages(string outputFileName, IEnumerable<string> imageFileNames, Action<float> progressCallback)
        {
            float count = imageFileNames.Count();
            if (count == 0)
            {
                progressCallback(1);
                return;
            }


            ApplicationClass app = null;
            Document doc = null;
            try
            {
                app = new ApplicationClass();
                doc = app.Documents.Add();

                doc.PageSetup.LeftMargin = 90;
                doc.PageSetup.RightMargin = 90;
                doc.PageSetup.TopMargin = 72;
                doc.PageSetup.BottomMargin = 72;

                int index = 0;
                foreach (var imageFileName in imageFileNames)
                {
                    AddImage(app, doc, imageFileName, index);
                    index++;
                    if (index != count)
                    {
                        progressCallback(index / count);
                    }
                }

                object format = MSWord.WdSaveFormat.wdFormatDocument;
                //doc.SaveAs(outputFileName);
                doc.ExportAsFixedFormat(outputFileName, MSWord.WdExportFormat.wdExportFormatPDF, false,
                    MSWord.WdExportOptimizeFor.wdExportOptimizeForPrint, MSWord.WdExportRange.wdExportFromTo, 1, index);
                progressCallback(1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(false);
                    Marshal.ReleaseComObject(doc);
                }

                if (app != null)
                {
                    app.Quit(false);
                    Marshal.ReleaseComObject(app);
                }
            }
        }

        private static void AddImage(ApplicationClass app, Document doc, string imageFileName, int index)
        {
            (int Width, int Height) rect;
            using (var stream = new FileStream(imageFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var g = stream.GetImage())
                {
                    var width = g.Width;
                    var height = g.Height;

                    rect = (width, height);
                }
            }

            //因为WORD的页大小有限制，所以要控制图片的大小在1200*700之内，

            if (rect.Width > 1200)
            {
                var width = 1200;
                var height = (float)rect.Height / rect.Width * 1200;
                rect = (width, (int)height);
            }

            if (rect.Height > 700)
            {
                var height = 700;
                var width = (float)rect.Width / rect.Height * 700;
                rect = ((int)width, height);
            }


            //定义要向文档中插入图片的位置
            var last = doc.Paragraphs.Add();
            object range = last.Range;
            const bool linkToFile = false;//默认
            const bool saveWithDocument = true;
            //向word中写入图片
            var shapes = doc.InlineShapes;
            shapes.AddPicture(imageFileName, linkToFile, saveWithDocument, range);
            app.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;//居中显示图片

            if (index == 0)
            {
                var ps = doc.PageSetup;
                ps.PageWidth = rect.Width + ps.LeftMargin + ps.RightMargin;
                ps.PageHeight = rect.Height + ps.TopMargin + ps.BottomMargin;
            }

            shapes[index + 1].Height = rect.Height;
            shapes[index + 1].Width = rect.Width;
        }

        #endregion

    }
}
