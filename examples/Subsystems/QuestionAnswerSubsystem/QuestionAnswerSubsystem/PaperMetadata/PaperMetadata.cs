using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using CodeArt.DomainDriven;
using CodeArt.Util;

namespace QuestionAnswerSubsystem
{
    /// <summary>
    /// 试卷原型，由多个问题的定义组成
    /// </summary>
    [Remotable("{id,name,markedCode}")]
    [ObjectRepository(typeof(IPaperMetadataRepository))]
    [ObjectValidator(typeof(PaperMetadataSpecification))]
    public class PaperMetadata : AggregateRoot<PaperMetadata, Guid>
    {
        public static readonly DomainProperty NameProperty = DomainProperty.Register<string, PaperMetadata>("Name");

        /// <summary>
        /// 原型的名称
        /// </summary>
        [PropertyRepository()]
        [NotEmpty()]
        [StringLength(1, 500)]
        public string Name
        {
            get
            {
                return GetValue<string>(NameProperty);
            }
            set
            {
                SetValue(NameProperty, value);
            }
        }

        [PropertyRepository()]
        [StringLength(0, 50)]
        [ASCIIString]
        [StringFormat(StringFormat.Letter | StringFormat.Number)]
        public static readonly DomainProperty MarkedCodeProperty = DomainProperty.Register<string, PaperMetadata>("MarkedCode");

        /// <summary>
        /// 引用标识
        /// </summary>
        public string MarkedCode
        {
            get
            {
                return GetValue<string>(MarkedCodeProperty);
            }
            set
            {
                SetValue(MarkedCodeProperty, value);
            }
        }

        /// <summary>
        /// 问题集合
        /// </summary>
        [PropertyRepository(Lazy = true)]
        [List(Max = 50)]
        private static readonly DomainProperty QuestionsProperty = DomainProperty.RegisterCollection<Question, PaperMetadata>("Questions");

        private DomainCollection<Question> _Questions
        {
            get
            {
                return GetValue<DomainCollection<Question>>(QuestionsProperty);
            }
            set
            {
                SetValue(QuestionsProperty, value);
            }
        }

        public IEnumerable<Question> Questions
        {
            get
            {
                return _Questions;
            }
            private set
            {
                this._Questions = new DomainCollection<Question>(QuestionsProperty, value);
            }
        }

        public void AddDefinition(Question definition)
        {
            _Questions.Add(definition);
        }

        public void RemoveDefinition(Question definition)
        {
            _Questions.Remove(definition);
        }


        public PaperMetadata(Guid id, string name, string markedCoded)
            : base(id)
        {
            this.Name = name;
            this.MarkedCode = markedCoded;
            MountEvents();
            this.OnConstructed();
        }

        [ConstructorRepository()]
        public PaperMetadata(Guid id)
            : base(id)
        {
            MountEvents();
            this.OnConstructed();
        }

        #region 挂载事件

        private void MountEvents()
        {
            this.PreDelete += OnPreDelete;
        }

        private void OnPreDelete(object sender, RepositoryEventArgs e)
        {
            //删除试卷原型下的问题定义
            var repository = Repository.Create<IQuestionRepository>();
            foreach (var question in _Questions)
            {
                repository.Delete(question);
            }
        }

        #endregion

        private class PaperMetadataEmpty : PaperMetadata
        {
            public PaperMetadataEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly PaperMetadata Empty = new PaperMetadataEmpty();
    }
}