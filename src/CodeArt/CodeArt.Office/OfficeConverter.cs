using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using MSWord = Microsoft.Office.Interop.Word;
using MSPowerPoint = Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;



namespace CodeArt.Office
{ 
    /// <summary>
    /// OfficeScanner 用于将word文档和ppt文档中的页转换为图片。
    /// </summary>
    public class OfficeConverter
    {
        #region Scan
        /// <summary>
        /// 将一个word/powerpoint文档中的指定页转换为位图。
        /// </summary>       
        public Bitmap[] Scan(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("File is not exist !");
            }

            string[] ary = filePath.Split('.');
            string extendName = ary[ary.Length - 1];

            if (extendName == "doc" || extendName == "docx")
            {
                return Scan4Word(filePath);
            }

            if (extendName == "ppt" || extendName == "pptx")
            {
                return Scan4PowerPoint(filePath, null);
            }

            throw new Exception("OfficeScanner only Support Word file and PowerPoint file !");
        }

        #region Scan4PowerPoint
        private Bitmap[] Scan4PowerPoint(string filePath, ICollection<int> pageIndexCollection)
        {
            List<Bitmap> bmList = new List<Bitmap>();
            MSPowerPoint.ApplicationClass pptApplicationClass = new MSPowerPoint.ApplicationClass();

            try
            {
                MSPowerPoint.Presentation presentation = pptApplicationClass.Presentations.Open(filePath, MsoTriState.msoTrue, MsoTriState.msoTrue, MsoTriState.msoFalse);
                int totalCount = presentation.Slides.Count;
                if (pageIndexCollection != null)
                {
                    totalCount = pageIndexCollection.Count;
                }
                int index = 0;
                foreach (MSPowerPoint.Slide slide in presentation.Slides)
                {
                    if (pageIndexCollection == null || pageIndexCollection.Contains(index))
                    {
                        slide.Export(@"C:\Users\74908\Desktop\111\" + index + ".png", "PNG");
                    }
                    ++index;
                }

                presentation.Close();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(presentation);

                return bmList.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                pptApplicationClass.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pptApplicationClass);
            }
        }
        #endregion

        #region Scan4Word
        private Bitmap[] Scan4Word(string filePath)
        {
            //复制目标文件，后续将操作副本
            string tmpFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\" + Path.GetFileName(filePath) + ".tmp";
            File.Copy(filePath, tmpFilePath);

            List<Bitmap> bmList = new List<Bitmap>();
            MSWord.ApplicationClass wordApplicationClass = new MSWord.ApplicationClass();
            wordApplicationClass.Visible = false;
            object missing = System.Reflection.Missing.Value;

            try
            {
                object readOnly = false;
                object filePathObject = tmpFilePath;

                MSWord.Document document = wordApplicationClass.Documents.Open(ref filePathObject, ref missing,
                    ref readOnly, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing);

                bool finished = false;
                while (!finished)
                {
                    document.Content.CopyAsPicture(); //拷贝到粘贴板
                    System.Windows.Forms.IDataObject data = Clipboard.GetDataObject();
                    if (data.GetDataPresent(DataFormats.MetafilePict))
                    {
                        object obj = data.GetData(DataFormats.MetafilePict);
                        Metafile metafile = MetafileHelper.GetEnhMetafileOnClipboard(IntPtr.Zero); //从粘贴板获取数据
                        Bitmap bm = new Bitmap(metafile.Width, metafile.Height);
                        using (Graphics g = Graphics.FromImage(bm))
                        {
                            g.Clear(Color.White);
                            g.DrawImage(metafile, 0, 0, bm.Width, bm.Height);
                        }
                        bmList.Add(bm);
                        Clipboard.Clear();
                    }

                    object What = MSWord.WdGoToItem.wdGoToPage;
                    object Which = MSWord.WdGoToDirection.wdGoToFirst;
                    object startIndex = "1";
                    document.ActiveWindow.Selection.GoTo(ref What, ref Which, ref missing, ref startIndex); // 转到下一页
                    MSWord.Range start = document.ActiveWindow.Selection.Paragraphs[1].Range;
                    MSWord.Range end = start.GoToNext(MSWord.WdGoToItem.wdGoToPage);
                    finished = (start.Start == end.Start);
                    if (finished) //最后一页
                    {
                        end.Start = document.Content.End;
                    }

                    object oStart = start.Start;
                    object oEnd = end.Start;
                    document.Range(ref oStart, ref oEnd).Delete(ref missing, ref missing); //处理完一页，就删除一页。                    
                }

                ((MSWord._Document)document).Close(ref missing, ref missing, ref missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(document);

                return bmList.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                wordApplicationClass.Quit(ref missing, ref missing, ref missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApplicationClass);
                File.Delete(tmpFilePath); //删除临时文件
            }
        }
        #endregion       
        #endregion
    }

    #region MetafileHelper
    /// <summary>
    /// MetafileHelper 用于从剪切板获取Metafile数据。
    /// </summary>
    internal class MetafileHelper
    {
        [DllImport("user32.dll")]
        static extern bool OpenClipboard(IntPtr hWndNewOwner);
        [DllImport("user32.dll")]
        static extern bool EmptyClipboard();
        [DllImport("user32.dll")]
        static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
        [DllImport("user32.dll")]
        static extern IntPtr GetClipboardData(uint uFormat);
        [DllImport("user32.dll")]
        static extern bool IsClipboardFormatAvailable(uint uFormat);
        [DllImport("user32.dll")]
        static extern bool CloseClipboard();
        [DllImport("gdi32.dll")]
        static extern IntPtr CopyEnhMetaFile(IntPtr hemfSrc, IntPtr hNULL);
        [DllImport("gdi32.dll")]
        static extern IntPtr CopyEnhMetaFileA(IntPtr hemfSrc, string filename);
        [DllImport("gdi32.dll")]
        static extern bool DeleteEnhMetaFile(IntPtr hemf);

        // Metafile mf is set to a state that is not valid inside this function.
        public static bool PutEnhMetafileOnClipboard(IntPtr hWnd, Metafile mf)
        {
            bool bResult = false;
            IntPtr hEMF, hEMF2;
            hEMF = mf.GetHenhmetafile(); // invalidates mf
            if (!hEMF.Equals(new IntPtr(0)))
            {
                hEMF2 = CopyEnhMetaFile(hEMF, new IntPtr(0));
                if (!hEMF2.Equals(new IntPtr(0)))
                {
                    if (OpenClipboard(hWnd))
                    {
                        if (EmptyClipboard())
                        {
                            IntPtr hRes = SetClipboardData(14 /*CF_ENHMETAFILE*/, hEMF2);
                            bResult = hRes.Equals(hEMF2);
                            CloseClipboard();
                        }
                    }
                }
                DeleteEnhMetaFile(hEMF);
            }
            return bResult;
        }

        public static Metafile GetEnhMetafileOnClipboard(IntPtr hWnd)
        {
            if (OpenClipboard(hWnd))
            {
                uint format = 14/*CF_ENHMETAFILE*/;
                if (IsClipboardFormatAvailable(format))
                {
                    IntPtr hRes = GetClipboardData(format);

                    CloseClipboard();

                    if (!hRes.Equals(new IntPtr(0)))
                    {
                        IntPtr hEMF = CopyEnhMetaFile(hRes, new IntPtr(0));
                        Metafile mf = new Metafile(hEMF, true);
                        return mf;
                    }
                }
            }
            return null;
        }

        public static bool IsClipboardFormatAvailableENHMETAFILE(IntPtr hWnd)
        {
            if (OpenClipboard(hWnd))
            {
                uint format = 14/*CF_ENHMETAFILE*/;
                if (IsClipboardFormatAvailable(format))
                {
                    CloseClipboard();
                    return true;
                }
            }
            return false;
        }

        public static bool SaveMetafile(Metafile mf, string filename)
        {
            bool bResult = false;
            IntPtr hEMF, hEMF2;
            hEMF = mf.GetHenhmetafile(); // invalidates mf
            if (!hEMF.Equals(new IntPtr(0)))
            {
                //hEMF = CopyEnhMetaFile(hEMF, new IntPtr(0));
                hEMF2 = CopyEnhMetaFileA(hEMF, filename);
                DeleteEnhMetaFile(hEMF2);
                if (hEMF2 != IntPtr.Zero)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
    #endregion
}
