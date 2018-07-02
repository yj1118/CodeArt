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
    /// 问题
    /// </summary>
    [ObjectRepository(typeof(IQuestionRepository))]
    [ObjectValidator(typeof(QuestionSpecification))]
    public class Question : AggregateRoot<Question, Guid>
    {
        public static readonly DomainProperty ContentProperty = DomainProperty.Register<string, Question>("Content");

        /// <summary>
        /// 问题的内容
        /// </summary>
        [PropertyRepository()]
        [NotEmpty()]
        [StringLength(1, 500)]
        public string Content
        {
            get
            {
                return GetValue<string>(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        public static readonly DomainProperty TypeProperty = DomainProperty.Register<QuestionType, Question>("Type");

        /// <summary>
        /// 题型
        /// </summary>
        [PropertyRepository()]
        [NotEmpty()]
        public QuestionType Type
        {
            get
            {
                return GetValue<QuestionType>(TypeProperty);
            }
            set
            {
                SetValue(TypeProperty, value);
            }
        }

        #region 选项（如果是选择题）

        /// <summary>
        /// 选项（如果是选择题）
        /// </summary>
        [PropertyRepository(Lazy = true)]
        [List(Max = 50)]
        private static readonly DomainProperty OptionsProperty = DomainProperty.RegisterCollection<Option, Question>("Options");

        private DomainCollection<Option> _Options
        {
            get
            {
                return GetValue<DomainCollection<Option>>(OptionsProperty);
            }
            set
            {
                SetValue(OptionsProperty, value);
            }
        }

        public IEnumerable<Option> Options
        {
            get
            {
                return _Options;
            }
            set
            {
                this._Options = new DomainCollection<Option>(OptionsProperty, value);
            }
        }

        #endregion

        public Question(Guid id, QuestionType type, string content)
            : base(id)
        {
            this.Type = type;
            this.Content = content;
            this.OnConstructed();
        }

        [ConstructorRepository()]
        public Question(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }

        private class QuestionDefinitionEmpty : Question
        {
            public QuestionDefinitionEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly Question Empty = new QuestionDefinitionEmpty();
    }
}
