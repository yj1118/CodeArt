using System;

using CodeArt.ModuleNest;
using CodeArt.DTO;
using CodeArt.ServiceModel;
using System.Web;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.XWPF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace Module.Util
{
    public static class ExportWord
    {
        public static DTObject GenerateWord(DTObject arg)
        {
            var localPath = HttpContext.Current.Server.MapPath("/temp");
            var fileName = arg.GetValue<string>("fileName", true) + ".docx";
            var filePath = localPath + "\\" + fileName;

            XWPFDocument doc = new XWPFDocument();

            var contents = arg.GetList("contents", true);

            foreach (var content in contents)
            {
                var type = content.GetValue<string>("type");

                switch (type)
                {
                    case "paragraph":
                        CreateParagraph(doc, content);
                        break;

                    case "table":
                        CreateTable(doc, content);
                        break;

                    default:
                        throw new ApplicationException("生成Word时出现不能识别的数据格式。");
                }
            }
  
            //转为字节数组  
            MemoryStream stream = new MemoryStream();
            doc.Write(stream);
            var buf = stream.ToArray();

            //保存为Word文件  
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }

            DTObject result = DTObject.Create();
            result.SetValue("webPath", "/temp/" + fileName);
            result.SetValue("absolutePath", filePath);

            return result;
        }

        private static ParagraphAlignment convertPA(string input)
        {
            var pa = input.ToLower();
            switch (pa)
            {
                case "left":
                    return ParagraphAlignment.LEFT;
                case "both":
                    return ParagraphAlignment.BOTH;
                case "center":
                    return ParagraphAlignment.CENTER;
                case "distribute":
                    return ParagraphAlignment.DISTRIBUTE;
                case "high_kashida":
                    return ParagraphAlignment.HIGH_KASHIDA;
                case "low_kashida":
                    return ParagraphAlignment.LOW_KASHIDA;
                case "medium_kashida":
                    return ParagraphAlignment.MEDIUM_KASHIDA;
                case "num_tab":
                    return ParagraphAlignment.NUM_TAB;
                case "right":
                    return ParagraphAlignment.RIGHT;
                case "thai_distribute":
                    return ParagraphAlignment.THAI_DISTRIBUTE; 
            }

            return ParagraphAlignment.LEFT;

        }

        private static void CreateParagraph(XWPFDocument doc, DTObject content)
        {
            XWPFParagraph p = doc.CreateParagraph();
            SetParagraph(p, content);
        }

        private static void CreateTable(XWPFDocument doc, DTObject content)
        {
            var row = content.GetValue<int>("rowCount");
            var col = content.GetValue<int>("colCount");
            XWPFTable table = doc.CreateTable(row, col);

            foreach (var tableRow in content.GetList("rows"))
            {
                int rowIndex = tableRow.GetValue<int>("index");
                foreach (var cell in tableRow.GetList("cells"))
                {
                    int cellIndex = cell.GetValue<int>("index");

                    var theCell = table.GetRow(rowIndex).GetCell(cellIndex);

                    var m_Pr = theCell.GetCTTc().AddNewTcPr();
                    m_Pr.tcW = new CT_TblWidth();
                    m_Pr.tcW.w = cell.GetValue<int>("width").ToString();//单元格宽
                    m_Pr.tcW.type = ST_TblWidth.dxa;

                    var paragraph = cell.GetObject("paragraph");
                    XWPFParagraph p = theCell.AddParagraph();
                    SetParagraph(p, paragraph);
                }
            }
        }

        private static void SetParagraph(XWPFParagraph p, DTObject content)
        {
            p.Alignment = convertPA(content.GetValue<string>("alignment"));

            foreach (var run in content.GetList("runs"))
            {
                XWPFRun r = p.CreateRun();
                r.FontFamily = run.GetValue<string>("fontFamily");
                r.FontSize = run.GetValue<int>("fontSize");
                r.IsBold = run.GetValue<bool>("isBold");
                r.SetText(run.GetValue<string>("text"));
            }
        }
    }


    [DTOClass()]
    public class ExportWordInfo
    {
        [DTOMember("fileName")]
        public string FileName { get; set; }

        [DTOMember("contents")]
        public Object[] Contents { get; set; }
    }

    [DTOClass()]
    public class WordParagraph
    {
        [DTOMember("type")]
        public string Type = "paragraph";

        [DTOMember("alignment")]
        public string Alignment { get; set; }

        [DTOMember("runs")]
        public WordRun[] Runs { get; set; }
    }

    [DTOClass()]
    public class WordRun
    {
        [DTOMember("text")]
        public string Text { get; set; }

        [DTOMember("fontFamily")]
        public string FontFamily { get; set; }

        [DTOMember("fontSize")]
        public int FontSize { get; set; }

        [DTOMember("isBold")]
        public bool IsBold { get; set; }
    }

    [DTOClass()]
    public class WordTable
    {
        [DTOMember("type")]
        public string Type = "table";

        [DTOMember("rowCount")]
        public int RowCount { get; set; }

        [DTOMember("colCount")]
        public int ColCount { get; set; }

        [DTOMember("rows")]
        public WordTableRow[] Rows { get; set; }

    }

    [DTOClass()]
    public class WordTableRow
    {
        [DTOMember("index")]
        public int Index { get; set; }

        [DTOMember("cells")]
        public WordTableCell[] Cells { get; set; }

    }

    [DTOClass()]
    public class WordTableCell
    {
        [DTOMember("index")]
        public int Index { get; set; }

        [DTOMember("width")]
        public int Width { get; set; }

        [DTOMember("paragraph")]
        public WordParagraph Paragraph { get; set; }
    }

}
