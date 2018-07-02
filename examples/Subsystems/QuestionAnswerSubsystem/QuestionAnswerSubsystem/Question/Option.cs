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
    /// 选项
    /// </summary>
    [ObjectRepository(typeof(IQuestionRepository))]
    public class Option : EntityObject<Option, int>
    {
        private static readonly DomainProperty ContentProperty = DomainProperty.Register<string, Option>("Content");

        /// <summary>
        /// 选项的内容
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


        [ConstructorRepository()]
        public Option(int id)
            : base(id)
        {
            this.OnConstructed();
        }

        private class OptionEmpty : Option
        {
            public OptionEmpty()
                : base(0)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }

        }

        public static readonly Option Empty = new OptionEmpty();
    }
}
