using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DTO;
using CodeArt.EasyMQ.Event;

namespace QuestionAnswerSubsystem
{
    [Event("SetPaperEvent")]
    public class SetPaperEvent : DomainEvent
    {
        [EventArg()]
        public Guid Id
        {
            get;
            set;
        }


        [EventArg()]
        public DTObject Paper
        {
            get;
            set;
        }


        public SetPaperEvent()
        {

        }

        protected override void RaiseImplement()
        {
            Guid? id = this.Paper.Dynamic.Id;
            Guid metadataId = this.Paper.Dynamic.MetadataId;
            DTObjects answers = this.Paper.Dynamic.Answers;

            if (id == null)
            {
                var para = answers.Select((item) =>
                {
                    return (item.GetValue<Guid>("questionId"), item.GetValue<string>("content"));
                });

                var cmd = new CreatePaper(metadataId, para);
                var paper = cmd.Execute();
                this.Id = paper.Id;
            }
            else
            {
                var para = answers == null ? null : answers.Select((item) =>
                {
                    return (item.GetValue<Guid>("id",Guid.Empty), item.GetValue<Guid>("questionId"), item.GetValue<string>("content"));
                });

                var cmd = new UpdatePaper(id.Value, para);
                var paper = cmd.Execute();
                this.Id = paper.Id;
            }
        }

        protected override void ReverseImplement()
        {
            if(this.Id != Guid.Empty)
            {
                DeletePaper cmd = new DeletePaper(this.Id);
                cmd.Execute();
            }
        }
    }
}
