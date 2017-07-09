using CodeArt.Concurrent;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 
    /// </summary>
    internal class ScheduledAction
    {
        public IAggregateRoot Target
        {
            get;
            private set;
        }

        public ScheduledActionType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// 显示指定target的映射类型
        /// 避免继承造成的影响
        /// </summary>
        public IPersistRepository Repository
        {
            get;
            private set;
        }

        public bool Expired
        {
            get;
            private set;
        }

        private ScheduledAction()
        {

        }

        /// <summary>
        /// 标示该行为已过期（也就是已执行过）
        /// </summary>
        public void MarkExpired(bool expired = true)
        {
            this.Expired = expired;
        }

        /// <summary>
        /// 固定规则验证
        /// </summary>
        /// <returns></returns>
        public ValidationResult Validate()
        {
            return this.Target.Validate();
        }


        #region 对象池

        private static PoolWrapper<ScheduledAction> _pool;


        public static ScheduledAction Borrow(IAggregateRoot target, IPersistRepository repository, ScheduledActionType type)
        {
            var action = _pool.Borrow();
            action.Target = target;
            action.Repository = repository;
            action.Type = type;
            action.Expired = false;
            return action;
        }

        public static void Return(ScheduledAction action)
        {
            _pool.Return(action);
        }

        static ScheduledAction()
        {
            _pool = new PoolWrapper<ScheduledAction>(() =>
            {
                return new ScheduledAction();
            }, (sa, phase) =>
            {
                return true;
            }, new PoolConfig()
            {
                MaxRemainTime = 300 //5分钟之内未被使用，就移除
            });
        }

        #endregion
    }

    public enum ScheduledActionType : byte
    {
        Create = 1,
        Delete = 2,
        Update = 3,
    }
}
