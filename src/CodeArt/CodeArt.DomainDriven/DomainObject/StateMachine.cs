using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    internal sealed class StateMachine
    {
        #region 状态

        private Status _status = Status.Dirty | Status.New | Status.Changed;


        /// <summary>
        /// 是否为脏对象
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return (_status & Status.Dirty) == Status.Dirty;
            }
            private set
            {
                _status &= ~Status.Dirty;
                if (value) _status |= Status.Dirty;
            }
        }

        /// <summary>
        /// 内存中新创建的对象
        /// </summary>
        public bool IsNew
        {
            get
            {
                return (_status & Status.New) == Status.New;
            }
            private set
            {
                _status &= ~Status.New;
                if (value) _status |= Status.New;
            }
        }

        /// <summary>
        /// 对象是否被改变
        /// </summary>
        public bool IsChanged
        {
            get
            {
                return (_status & Status.Changed) == Status.Changed;
            }
            private set
            {
                _status &= ~Status.Changed;
                if (value) _status |= Status.Changed;
            }
        }

        public void MarkDirty()
        {
            this.IsDirty = true;
        }

        public void MarkNew()
        {
            this.IsNew = true;
        }

        public void MarkChanged()
        {
            this.IsChanged = true;
        }

        /// <summary>
        /// 设置对象为干净的
        /// </summary>
        public void MarkClean()
        {
            this.IsDirty = false;
            this.IsNew = false;
            this.IsChanged = false;
            _propertyChangedRecord.Clear();
        }

        private Dictionary<string, bool> _propertyChangedRecord = new Dictionary<string, bool>();

        public void SetPropertyChanged(string propertyName)
        {
            _propertyChangedRecord[propertyName] = true;
            MarkDirty();
            MarkChanged();
        }

        public void ClearPropertyChanged(string propertyName)
        {
            if(_propertyChangedRecord.Remove(propertyName))
            {
                if(_propertyChangedRecord.Count == 0)
                {
                    this.IsDirty = false;
                    this.IsChanged = false;
                }
            }
        }

        /// <summary>
        /// 判断属性是否被更改
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool IsPropertyChanged(string propertyName)
        {
            //如果是一个内存中新建的对象或者属性确实被改变了，我们认为属性被改变
            return _propertyChangedRecord.ContainsKey(propertyName);
        }

        /// <summary>
        /// 仅仅只是属性<paramref name="propertyName"/>发生了改变
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool OnlyPropertyChanged(string propertyName)
        {
            return _propertyChangedRecord.Count == 1 && this.IsPropertyChanged(propertyName);
        }

        #endregion

        public StateMachine()
        {
        }

        private StateMachine(Status status, Dictionary<string, bool> changedRecord)
        {
            _status = status;
            _propertyChangedRecord = changedRecord;
        }

        public StateMachine Clone()
        {
            return new StateMachine(_status, new Dictionary<string, bool>(_propertyChangedRecord));
        }

        /// <summary>
        /// 合并状态，将目标已更改的属性更新到自身数据中
        /// </summary>
        /// <param name="target"></param>
        public void Combine(StateMachine target)
        {
            var record = target._propertyChangedRecord;
            foreach (var p in record)
            {
                var propertyName = p.Key;
                if (!this.IsPropertyChanged(propertyName))
                    this.SetPropertyChanged(p.Key);
            }
        }


        private enum Status : byte
        {
            /// <summary>
            /// 对象是否为脏的
            /// </summary>
            Dirty = 0x1,
            /// <summary>
            /// 对象是否为新建的
            /// </summary>
            New = 0x2,
            /// <summary>
            /// 对象是否被改变
            /// </summary>
            Changed = 0x4
        }
    }
}
