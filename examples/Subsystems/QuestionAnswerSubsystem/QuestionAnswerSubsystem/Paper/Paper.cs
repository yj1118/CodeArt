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
    /// 试卷
    /// </summary>
    [Remotable()]
    [ObjectRepository(typeof(IPaperRepository))]
    [ObjectValidator(typeof(PaperSpecification))]
    public class Paper : AggregateRoot<Paper, Guid>
    {
        public static readonly DomainProperty MetadataProperty = DomainProperty.Register<PaperMetadata, Paper>("Metadata");

        /// <summary>
        /// 试卷所属的原型
        /// </summary>
        [PropertyRepository()]
        [NotEmpty()]
        public PaperMetadata Metadata
        {
            get
            {
                return GetValue<PaperMetadata>(MetadataProperty);
            }
            set
            {
                SetValue(MetadataProperty, value);
            }
        }

        /// <summary>
        /// 问题集合
        /// </summary>
        [PropertyRepository(Lazy = true)]
        [List(Max = 50)]
        private static readonly DomainProperty AnswersProperty = DomainProperty.RegisterCollection<Answer, Paper>("Answers");

        private DomainCollection<Answer> _Answers
        {
            get
            {
                return GetValue<DomainCollection<Answer>>(AnswersProperty);
            }
            set
            {
                SetValue(AnswersProperty, value);
            }
        }

        public IEnumerable<Answer> Answers
        {
            get
            {
                return _Answers;
            }
            set
            {
                this._Answers = new DomainCollection<Answer>(AnswersProperty, value);
            }
        }

        public Answer GetAnswer(Guid id)
        {
            return _Answers.FirstOrDefault((answer) => answer.Id == id) ?? Answer.Empty;
        }

        public void AddAnswer(Answer answer)
        {
            _Answers.Add(answer);
        }

        public Paper(Guid id, PaperMetadata metadata)
            : base(id)
        {
            this.Metadata = metadata;
            MountEvents();
            this.OnConstructed();
        }

        [ConstructorRepository()]
        public Paper(Guid id)
            : base(id)
        {
            MountEvents();
            this.OnConstructed();
        }

        #region 挂载事件

        private void MountEvents()
        {
            this.PreAdd += OnPreAdd;
            this.PreUpdate += OnPreUpdate;
            this.PreDelete += OnPreDelete;
        }

        private void OnPreAdd(object sender, RepositoryEventArgs e)
        {
            var repository = Repository.Create<IAnswerRepository>();
            foreach (var answer in _Answers)
            {
                repository.Add(answer);
            }
        }


        private void OnPreUpdate(object sender, RepositoryEventArgs e)
        {
            if (this.IsPropertyChanged(AnswersProperty))
            {
                var repository = Repository.Create<IAnswerRepository>();
                foreach (var answer in _Answers)
                {
                    if (answer.IsNew)
                    {
                        repository.Add(answer);
                        continue;
                    }

                    if (answer.IsDirty)
                    {
                        repository.Update(answer);
                        continue;
                    }
                }
            }
        }

        private void OnPreDelete(object sender, RepositoryEventArgs e)
        {
            //删除试卷原型下的问题定义
            var repository = Repository.Create<IAnswerRepository>();
            foreach (var answer in _Answers)
            {
                repository.Delete(answer);
            }
        }

        #endregion

        private class PaperEmpty : Paper
        {
            public PaperEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly Paper Empty = new PaperEmpty();
    }
}
