using CodeArt.AppSetting;
using CodeArt.DTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public static class Logable
    {
        internal static void Using(ICommand command, Action action)
        {
            var commandType = command.GetType();
            var tip = LogableAttribute.GetTip(commandType);
            if (tip == null)
            {
                //不参与日志记录
                action();
                return;
            }

            var context = Current = new LogContext(tip.ActionName);
            action();
            CommitLog(tip, context);
        }

        /// <summary>
        /// 提交日志
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="action"></param>
        /// <param name="fileIds"></param>
        /// <param name="group"></param>
        private static void CommitLog(LogableAttribute tip, LogContext context)
        {
            if (tip == null) return;

            DTObject arg = DTObject.Create();
            var content = context.Content;
            CreateObserver("action", tip.ActionName);

            //多租户作为观察者之一
            long tenantId = AppSession.TenantId;
            if (tenantId > 0)
            {
                //content["tenantId"] = tenantId;
                CreateObserver("tenant", tenantId.ToString());
            }

            //负责人作为观察者之一
            string principalId = AppSession.PrincipalId;
            if (!string.IsNullOrEmpty(principalId))
            {
                //content["principalId"] = principalId;
                CreateObserver("principal", principalId);
            }

            var observers = context.Observers;
            foreach (var observer in observers)
            {
                CreateObserver(observer.Type, observer.Id);
            }

            arg.SetValue("action", context.ActionName);
            arg.SetObject("content", content);
            arg.SetValue("createTime", DateTime.Now);

            DomainMessageProvider.Notice(AddLogEventName, arg, SyncLog);

            void CreateObserver(string type, string id)
            {
                var item = arg.CreateAndPush("observers");
                item["type"] = type;
                item["id"] = id;
            }
        }

        /// <summary>
        /// 添加日志观察者
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        public static void AddObserver(string type, string id)
        {
            if (!ExistCurrent()) return;
            Current.AddObserver(type, id);
        }

        #region 写入日志内容

        /// <summary>
        /// 向日志内容中写入成员
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="member"></param>
        public static void Write(string memberName, DTObject member)
        {
            if (!ExistCurrent()) return;
            Current.Write(memberName, member);
        }

        public static void Write(string memberName, object member)
        {
            if (!ExistCurrent()) return;
            Current.Write(memberName, member);
        }

        /// <summary>
        /// 写入一个没有值空成员
        /// </summary>
        /// <param name="memberName"></param>
        public static void Write(string memberName)
        {
            if (!ExistCurrent()) return;
            Current.Write(memberName, string.Empty);
        }

        public static void Write(string schemaCode, DomainObject target)
        {
            if (!ExistCurrent() || target.IsEmpty()) return;
            var memberName = target.ObjectType.Name;
            var member = DTObject.Create(schemaCode, target);
            Current.Write(memberName, member);
        }

        public static void Write(DomainObject target)
        {
            if (!ExistCurrent() || target.IsEmpty()) return;
            var tip = ObjectLogableAttribute.GetTip(target.ObjectType);
            if (tip == null) throw new BusinessException(string.Format("对象{0}没有标记ObjectLogable特性", target.ObjectType.Name));

            var memberName = target.ObjectType.Name;
            var member = DTObject.Create(tip.SchemaCode, target);
            Current.Write(memberName, member);
        }


        #region 属性更改

        internal static void Write(DomainProperty property, object newValue, object oldValue)
        {
            if (!ExistCurrent() || property.LogableTip == null) return;

            var tip = property.LogableTip;
            var propertyName = string.IsNullOrEmpty(tip.Name) ? property.Name : tip.Name;
            switch (tip.Level)
            {
                case TrackLevel.Slim:
                    {
                        Write(propertyName);
                    }
                    break;
                case TrackLevel.Normal:
                    {
                        Write(propertyName, Map(newValue, tip));
                    }
                    break;
                case TrackLevel.Pro:
                    {
                        DTObject content = DTObject.Create();
                        content["newValue"] = Map(newValue, tip);
                        content["oldValue"] = Map(oldValue, tip);

                        Write(propertyName, content);
                    }
                    break;
            }
        }

        private static object Map(object value, PropertyLogableAttribute propertyTip)
        {
            if (value == null) return null;

            var collection = value as IList;

            if (collection != null)
            {
                DTObject dto = DTObject.Create();
                //if (string.IsNullOrEmpty(propertyTip.SchemaCode))
                //{
                //    dto.Push("rows")

                //    row["value"] = obj.ToString();
                //}
                //else
                //{
                //    row.Load(propertyTip.SchemaCode, obj);
                //}

                dto.Push("rows", collection, (row, obj) =>
                {
                    if (string.IsNullOrEmpty(propertyTip.SchemaCode))
                    {
                        row.SetValue(obj.ToString());
                        //row["value"] = obj.ToString();
                    }
                    else
                    {
                        row.Load(propertyTip.SchemaCode, obj);
                    }
                });
                return dto.GetList("rows");
            }
            else
            {
                if (string.IsNullOrEmpty(propertyTip.SchemaCode))
                {
                    return value.ToString();
                    //DTObject dto = DTObject.Create();
                    //dto["value"] = value.ToString();
                    //return dto;
                }
                else
                {
                    return DTObject.Create(propertyTip.SchemaCode, value);
                }
            }
        }

        #endregion

        #endregion


        #region 基于当前应用程序会话的数据上下文


        private const string _sessionKey = "LogContext.Current";

        /// <summary>
        /// 获取或设置当前会话的数据上下文
        /// </summary>
        private static LogContext Current
        {
            get
            {
                var logContext = AppSession.GetItem<LogContext>(_sessionKey);
                if (logContext == null)
                    throw new InvalidOperationException("LogContext.Current为null,无法使用日志上下文对象");
                return logContext;
            }
            set
            {
                AppSession.SetItem<LogContext>(_sessionKey, value);
            }
        }

        private static bool ExistCurrent()
        {
            return AppSession.GetItem<LogContext>(_sessionKey) != null;
        }


        /// <summary>
        /// 日志上下文
        /// </summary>
        private class LogContext
        {
            public string ActionName
            {
                get;
                private set;
            }

            public DTObject Content
            {
                get;
                private set;
            }

            private List<(string Type, string Id)> _observers;

            public IEnumerable<(string Type, string Id)> Observers
            {
                get
                {
                    if (_observers == null) return Array.Empty<(string Type, string Id)>();
                    return _observers;
                }
            }

            public void AddObserver(string type, string id)
            {
                _observers.Add((type, id));
            }


            public void Write(string memberName, DTObject member)
            {
                this.Content[memberName] = member;
            }

            public void Write(string memberName, object member)
            {
                this.Content[memberName] = member;
            }

            public LogContext(string actionName)
            {
                this.Content = DTObject.Create();
                this.ActionName =  actionName;
            }
        }


        #endregion



        /// <summary>
        /// 新增领域日志
        /// </summary>
        public const string AddLogEventName = "d:AddDomainLog";

        /// <summary>
        /// 修改领域日志
        /// </summary>
        public const string UpdateLogEventName = "d:UpdateDomainLog";

        /// <summary>
        /// 删除领域日志
        /// </summary>
        public const string DeleteLogEventName = "d:DeleteDomainLog";

        private const string _syncLogSessionKey = "Command.SyncLog";

        /// <summary>
        /// 为当前会话指定同步记录日志
        /// </summary>
        public static bool SyncLog
        {
            get
            {
                return AppSession.GetItem<bool>(_syncLogSessionKey, false);
            }
            set
            {
                AppSession.SetItem<bool>(_syncLogSessionKey, value);
            }
        }



        //private void DeleteLog(ObjectLogableAttribute tip, Guid logId)
        //{
        //    if (tip == null) return;

        //    DTObject arg = DTObject.Create();

        //    arg.SetValue("Id", logId);

        //    DomainMessageProvider.Notice(DomainObject.DeleteLogEventName, arg, DomainObject.SyncLog);
        //}

        ///// <summary>
        ///// 更新日志仅对业务级的日志，内置的属性变化日志是不能更改的
        ///// </summary>
        ///// <param name="tip"></param>
        ///// <param name="action"></param>
        //private void UpdateLog(ObjectLogableAttribute tip, Guid logId, DTObject newValue, IEnumerable<Guid> fileIds)
        //{
        //    if (tip == null) return;

        //    DTObject arg = DTObject.Create();
        //    arg.SetValue("Id", logId);

        //    var content = DTObject.Create();

        //    if (newValue != null && !newValue.IsEmpty())
        //    {
        //        content["Action.New"] = newValue;
        //    }

        //    if (fileIds != null)
        //    {
        //        arg.SetValue("FileIds", fileIds);
        //    }
        //    arg.SetObject("Content", content);

        //    DomainMessageProvider.Notice(DomainObject.UpdateLogEventName, arg, DomainObject.SyncLog);
        //}

    }


}
