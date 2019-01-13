using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.XamlControls.Metronic;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.ServiceModel;
using CodeArt.Util;
using CodeArt.Web.WebPages;


namespace Module.QuestionAnswer
{
    [TemplateCode("Template", "Module.QuestionAnswer.PaperReader.html,Module.QuestionAnswer")]
    public class PaperReader : Control
    {
        public static readonly DependencyProperty MetadataProperty = DependencyProperty.Register<string, PaperReader>("Metadata", () => { return string.Empty; });

        /// <summary>
        /// 引用是试卷原型的markedCode
        /// </summary>
        public string Metadata
        {
            get
            {
                return (string)GetValue(MetadataProperty);
            }
            set
            {
                SetValue(MetadataProperty, value);
            }
        }

        public static readonly DependencyProperty MetadataIdProperty = DependencyProperty.Register<string, PaperReader>("MetadataId", () => { return string.Empty; });

        /// <summary>
        /// 引用是试卷原型的编号
        /// </summary>
        public string MetadataId
        {
            get
            {
                return (string)GetValue(MetadataIdProperty);
            }
            set
            {
                SetValue(MetadataIdProperty, value);
            }
        }

        private IScriptView Load(ScriptView view)
        {
            var sender = view.GetSender<PaperSE>();

            var data = sender.PaperMetadataId != null ? _getPaperByMetadataId.Get(sender.PaperMetadataId)
                                                     : _getPaperByMetadataMarkedCode.Get(sender.PaperMetadataMarkedCode);

            return new DataView(data);
        }


        private static LazyIndexer<string, DTObject> _getPaperByMetadataMarkedCode = new LazyIndexer<string, DTObject>((metadataMarkedCode) =>
        {
            return ServiceContext.InvokeDynamic("getPaperMetadataDetail", (arg) =>
            {
                arg.MetadataMarkedCode = metadataMarkedCode;
            }).AsReadOnly();
        });

        private static LazyIndexer<string, DTObject> _getPaperByMetadataId = new LazyIndexer<string, DTObject>((metadataId) =>
        {
            return ServiceContext.InvokeDynamic("getPaperMetadataDetail", (arg) =>
            {
                arg.MetadataId = metadataId;
            }).AsReadOnly();
        });

        internal static void ClearMetadata()
        {
            _getPaperByMetadataId.Clear();
            _getPaperByMetadataMarkedCode.Clear();
        }

        public PaperReader()
        {
        }

        static PaperReader()
        { }
    }
}
   