using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DTO;
using FileSubsystem;

namespace UserSubsystem
{
    [Event("DeleteUser")]
    public class DeleteUserEvent : DomainEvent
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        [EventArg()]
        public Guid Id
        {
            get;
            set;
        }

        #region 备份

        /// <summary>
        /// 用户名
        /// </summary>
        [EventArg()]
        public string Name
        {
            get;
            set;
        }

        [EventArg()]
        public Sex Sex
        {
            get;
            set;
        }

        [EventArg()]
        public string MobileNumber
        {
            get;
            set;
        }


        [EventArg()]
        public string AccountName
        {
            get;
            set;
        }

        [EventArg()]
        public string Password
        {
            get;
            set;
        }


        [EventArg()]
        public Guid[] RoleIds
        {
            get;
            set;
        }

        #endregion

        public DeleteUserEvent()
        {

        }

        protected override void RaiseImplement()
        {
            var user = UserCommon.FindById(this.Id, QueryLevel.None);
            if (user.IsEmpty()) return;

            //记录数据，留待恢复使用
            this.Name = user.Name;
            this.MobileNumber = user.MobileNumber;
            this.AccountName = user.Account.Name;
            this.Password = user.Account.Password;
            this.RoleIds = user.Account.Roles.Select((t) => t.Id).ToArray();
            this.Sex = user.Sex;


            var cmd = new DeleteUser(this.Id);
            cmd.Execute();
        }

        protected override void ReverseImplement()
        {
            if (this.Id == Guid.Empty) return;
            //这里我们不做还原操作，主要原因是
            //1.涉及到磁盘空间的升级，这主要体现在磁盘空间的还原机制上，由于磁盘有可能很多文件，所以不可能删除后还还原
            //  所以磁盘空间不用还原，但是要标示没有引用了，这样才能再次分配给恢复的user，但是工作量比较大，以后再升级
            //2.一般来说，删除操作失败，就算删除了部分数据，由于有空对象的概念，所以就算不全部恢复也问题不大

        }
    }
}
