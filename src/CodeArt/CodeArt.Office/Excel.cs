using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Imaging;

using CodeArt.IO;
using CodeArt.Util;

using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Drawing;

namespace CodeArt.Office
{
    /// <summary>
    /// 之前采用的通过EXCEL COM组件提供的API来转换，由于API提供的页数据本身就会出错，所以不能使用该方法
    /// 这里我们采用的做法是，将EXCEL导出为PDF，然后由PDF每页转换成图片，这样输出的格式就与打印的格式是一样的
    /// 至此，问题解决了
    /// </summary>
    public class Excel : Pdf
    {
        public Excel(string fileName)
            : base(GetPdfFileName(fileName))
        {
        }

        private static string GetPdfFileName(string fileName)
        {
            Application application = null;
            Workbook book = null;

            try
            {
                application = GetApplication();
                book = GetWorkbook(application, fileName);

                var temp = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), Guid.NewGuid().ToString("N"));
                temp = string.Format("{0}.pdf", temp);

                //转成pdf
                book.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, temp, null, false);
                return temp;
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                if (book != null)
                {
                    book.Close(false, Type.Missing, Type.Missing);
                    Marshal.ReleaseComObject(book);
                }

                if(application !=null)
                {
                    application.Quit();
                    Marshal.ReleaseComObject(application);
                }
            }
        }


        private static Application GetApplication()
        {
            Application app = new Application();
            app.Visible = app.ScreenUpdating = app.DisplayAlerts = false;
            app.CopyObjectsWithCells = true;
            app.CutCopyMode = XlCutCopyMode.xlCopy;
            app.DisplayClipboardWindow = false;
            return app;
        }

        private static Workbook GetWorkbook(Application app, string fileName)
        {
            return app.Workbooks.Open(fileName, false, false, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                      Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                      Type.Missing, Type.Missing);
        }

    }
}
