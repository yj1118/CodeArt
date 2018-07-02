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
    /// 回答
    /// </summary>
    [ObjectRepository(typeof(IAnswerRepository))]
    [ObjectValidator(typeof(AnswerSpecification))]
    public class Answer : AggregateRoot<Answer, Guid>
    {
        public static readonly DomainProperty QuestionProperty = DomainProperty.Register<Question, Answer>("Question");

        /// <summary>
        /// 问题
        /// </summary>
        [PropertyRepository()]
        [NotEmpty()]
        public Question Question
        {
            get
            {
                return GetValue<Question>(QuestionProperty);
            }
            set
            {
                SetValue(QuestionProperty, value);
            }
        }


        public static readonly DomainProperty ContentProperty = DomainProperty.Register<string, Answer>("Content");

        /// <summary>
        /// 回答的内容
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

      

        public Answer(Guid id, Question question,string content)
            : base(id)
        {
            this.Question = question;
            this.Content = content;
            this.OnConstructed();
        }

        [ConstructorRepository()]
        public Answer(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }

        private class AnswerEmpty : Answer
        {
            public AnswerEmpty()
                : base(Guid.Empty, Question.Empty, string.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly Answer Empty = new AnswerEmpty();
    }
}
