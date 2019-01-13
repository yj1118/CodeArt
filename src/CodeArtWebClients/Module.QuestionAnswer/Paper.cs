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
    [TemplateCode("Template", "Module.QuestionAnswer.Paper.html,Module.QuestionAnswer")]
    [TemplateCode("ItemTemplate", "Module.QuestionAnswer.Question.html,Module.QuestionAnswer")]
    public class Paper : ItemsControl
    {
        public static readonly DependencyProperty RenderQuestionProperty = DependencyProperty.Register<bool, Paper>("RenderQuestion", () => { return false; });

        public bool RenderQuestion
        {
            get
            {
                return (bool)GetValue(RenderQuestionProperty);
            }
            set
            {
                SetValue(RenderQuestionProperty, value);
            }
        }


        public static readonly DependencyProperty DisabledProperty = DependencyProperty.Register<string, Paper>("Disabled", () => { return "false"; });

        public string Disabled
        {
            get
            {
                return (string)GetValue(DisabledProperty);
            }
            set
            {
                SetValue(DisabledProperty, value);
            }
        }

        public static readonly DependencyProperty MetadataProperty = DependencyProperty.Register<string, Paper>("Metadata", () => { return string.Empty; });

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

        public static readonly DependencyProperty MetadataIdProperty = DependencyProperty.Register<string, Paper>("MetadataId", () => { return string.Empty; });

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

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register<string, Paper>("Label", () => { return string.Empty; });

        public string Label
        {
            get
            {
                return GetValue(LabelProperty) as string;
            }
            set
            {
                SetValue(LabelProperty, value);
            }
        }


        public static readonly DependencyProperty FieldProperty = DependencyProperty.Register<string, Paper>("Field", () => { return "Paper"; });

        public string Field
        {
            get
            {
                return GetValue(FieldProperty) as string;
            }
            set
            {
                SetValue(FieldProperty, value);
            }
        }


        public Paper()
        {
        }

     
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if(this.RenderQuestion)
            {
                this.PreRender += OnPreRender;
            }
        }

        private void OnPreRender(object sender, object e)
        {
            if (string.IsNullOrEmpty(this.Metadata) && string.IsNullOrEmpty(this.MetadataId)) return;

            var data = ServiceContext.InvokeDynamic("getPaperMetadataDetail", (arg) =>
            {
                if (!string.IsNullOrEmpty(this.Metadata)) arg.MetadataMarkedCode = this.Metadata;
                else if (!string.IsNullOrEmpty(this.MetadataId)) arg.MetadataId = this.MetadataId;
            });
            if (data.IsEmpty()) return;
            this.MetadataId = data.GetValue<string>("id");

            int index = 0;
            string disable = this.Disabled;

            data.Each("questions", (question) =>
            {
                index++;
                var type = question.GetValue<string>("type");
                var id = question.GetValue<string>("id");
                var content = question.GetValue<string>("content");
                var required = question.GetValue<bool>("required");
                Question item = new Question() { Type = type, Disabled = disable, Text = string.Format("{0}.{1}", index, content), Value = id,Required = required };
                if (type != "3")
                {
                    //不是问答题
                    question.Each("options", (t) =>
                    {
                        var value = t.GetValue<string>("id");
                        var text = t.GetValue<string>("content");
                        var option = new Option() { Text = text, Value = value };
                        item.Options.Add(option);
                    });
                }
                this.Items.Add(item);
            });
        }


        public override void OnInit()
        {
            //输入组件默认就会加入到表单中，写了该属性，就会打印 data-form=''
            if (!this.Attributes.Contains("form"))
            {
                this.Attributes.SetValue(this, "form", "");
            }
            base.OnInit();
            this.RegisterScriptAction("Load", this.Load);
        }

        private IScriptView Load(ScriptView view)
        {
            var sender = view.GetSender<PaperSE>();

            //var paperMetadataId = sender.PaperMetadataId;
            //var paperMetadataMarkedCode = sender.PaperMetadataMarkedCode;
            //var disabled = this.Disabled;

            //for(var i=0;i<100;i++)
            //{
            //    try
            //    {
            //        var temp = paperMetadataId != null ? GetPaperByMetadataId(paperMetadataId, disabled)
            //                                 : GetPaperByMetadataMarkedCode(paperMetadataMarkedCode, disabled);
            //    }
            //    catch(Exception ex)
            //    {

            //    }
            //}

            var data = sender.PaperMetadataId != null ? GetPaperByMetadataId(sender.PaperMetadataId, this.Disabled)
                                                     : GetPaperByMetadataMarkedCode(sender.PaperMetadataMarkedCode, this.Disabled);
            return new DataView(data);
        }


        private static LazyIndexer<string, Func<string, DTObject>> _getPaperByMetadataId = new LazyIndexer<string, Func<string, DTObject>>((metadataId) =>
        {
            return LazyIndexer.Init<string, DTObject>((disabled) =>
            {
                return GetPaperByMetadataId(metadataId, disabled);
            });
        });

        private static DTObject GetPaperByMetadataId(string metadataId,string disabled)
        {
            var paperCode = string.Format("<!DOCTYPE xaml><qa:Paper xmlns:qa=\"http://schemas.codeart.cn/web/xaml/qa\"  metadataId=\"{0}\" disabled=\"{1}\" renderQuestion=\"true\" />", metadataId, disabled);
            var data = DTObject.Create();
            var code = XamlUtil.GetCode(paperCode, true);
            data.SetValue("code", code);
            return data.AsReadOnly();
        }


        private static LazyIndexer<string, Func<string, DTObject>> _getPaperByMetadataMarkedCode = new LazyIndexer<string, Func<string, DTObject>>((metadataMarkedCode) =>
        {
            return LazyIndexer.Init<string, DTObject>((disabled) =>
            {
                return GetPaperByMetadataMarkedCode(metadataMarkedCode, disabled);
            });
        });

        private static DTObject GetPaperByMetadataMarkedCode(string metadataMarkedCode, string disabled)
        {
            var paperCode = string.Format("<!DOCTYPE xaml><qa:Paper xmlns:qa=\"http://schemas.codeart.cn/web/xaml/qa\"  metadata=\"{0}\" disabled=\"{1}\" renderQuestion=\"true\" />", metadataMarkedCode, disabled);
            var data = DTObject.Create();
            var code = XamlUtil.GetCode(paperCode, true);
            data.SetValue("code", code);
            return data.AsReadOnly();
        }

        /// <summary>
        /// 清空试卷原型的缓存
        /// </summary>
        public static void ClearMetadata()
        {
            _getPaperByMetadataId.Clear();
            _getPaperByMetadataMarkedCode.Clear();

            PaperReader.ClearMetadata();
        }


        static Paper()
        { }
    }
}
   