using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CodeArt.IO;

namespace CodeArt.Office
{
    public static class DocumentFactory
    {
        public static IDocument Create(string fileName)
        {
            var e = IOUtil.GetExtension(fileName).ToLower();
            switch(e)
            {
                case "doc":
                case "docx":
                    return new Word(fileName);
                case "ppt":
                case "pptx":
                    return new PPT(fileName);
                case "pdf":
                    return new Pdf(fileName);
                case "xls":
                case "xlsx":
                    return new Excel(fileName);
            }
            return null;
        }
    }
}
