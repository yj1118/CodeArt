using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;

using CodeArt.AppSetting;
using CodeArt.Util;
using CodeArt.Search;
using CodeArt.Concurrent;
using System.Configuration;
using CodeArt.Diagnostics;

namespace CodeArt.DomainDriven.DataAccess
{
    public static class ESRepository
    {
        private static List<IESRepository> _repositories = new List<IESRepository>();

        internal static void Reigster(IESRepository repository)
        {
            lock(_repositories)
            {
                if(!_repositories.Contains(repository))
                    _repositories.Add(repository);
            }
        }

        internal static IEnumerable<IESRepository> GetAll()
        {

            return _repositories.ToArray();

        }

        //private const string _refreshSessionKey = "ESRepository.Refresh";

        ///// <summary>
        ///// 更新文档时，是否立即刷新，默认刷新，设置为false会提升性能
        ///// </summary>
        //public static bool Refresh
        //{
        //    get
        //    {
        //        return AppSession.GetItem<bool>(_refreshSessionKey, true);
        //    }
        //    set
        //    {
        //        AppSession.SetItem<bool>(_refreshSessionKey, value);
        //    }
        //}

    }


    /// <summary>
    /// TRoot的编号，仅支持string,Guid,long
    /// </summary>
    /// <typeparam name="TRoot"></typeparam>
    /// <typeparam name="TRootDocument"></typeparam>
    public abstract class ESRepository<TRoot, TRootDocument> : Repository<TRoot>, IESRepository
        where TRoot : class, IAggregateRoot
        where TRootDocument : class, IDocument
    {
        #region 静态成员
        private static string IndexName;
        private static string TranIndexName;

        static ESRepository()
        {
            IndexName = GetProductIndexName();
            TranIndexName = GetTranIndexName();
            CreateIndex();
        }

        protected ESRepository()
        {
            ESRepository.Reigster(this);
        }

        /// <summary>
        /// 产品库索引
        /// </summary>
        /// <returns></returns>
        private static string GetProductIndexName()
        {
            //读取配置文件中的索引名称
            var key = string.Format("es-index-{0}", typeof(TRoot).Name);
            var indexName = ConfigurationManager.AppSettings[key];
            return indexName;
        }

        private static string GetTranIndexName()
        {
            //读取配置文件中的索引名称
            var key = string.Format("es-index-{0}-tran", typeof(TRoot).Name);
            var indexName = ConfigurationManager.AppSettings[key];
            return indexName;
        }

        private static void CreateIndex()
        {
            ClientTool.Current.CreateIndex<TRootDocument>(IndexName);
            ClientTool.Current.CreateIndex<TRootDocument>(TranIndexName);
        }
        #endregion

        protected string GetIndexName()
        {
            return IndexName;
        }

        /// <summary>
        /// 将根对象转换为文档对象
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        protected abstract TRootDocument MapDocument(TRoot root);

        /// <summary>
        /// 将文档对象转换为根对象
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        protected abstract TRoot MapRoot(TRootDocument document);

        protected override TRoot PersistFind(object id, QueryLevel level)
        {
            var document = ClientTool.Current.GetDocumentById<TRootDocument>(IndexName, id);
            var obj = MapRoot(document);
            obj.MarkClean();
            return obj;
        }

        #region 新增

        protected override void PersistAddRoot(TRoot obj)
        {
            if (obj.IsEmpty()) return;

            // 数据转换
            var doc = MapDocument(obj);

            //先在事务中加
            doc.OperationType = "c";
            ClientTool.Current.AddDocument(TranIndexName, doc);
            //再在产品库加
            ClientTool.Current.AddDocument(IndexName, doc);
        }

        public override void OnAddCommited(IAggregateRoot obj)
        {
            //删除事务数据
            ClientTool.Current.DeleteDocument<TRootDocument>(TranIndexName, obj.GetIdentity());

            base.OnAddCommited(obj);
        }

        private void RollbackAdd(IAggregateRoot root)
        {
            var id = root.GetIdentity();
            var copy = ClientTool.Current.GetDocumentById<TRootDocument>(TranIndexName, id);
            RollbackAdd(copy);
        }

        private void RollbackAdd(TRootDocument copy)
        {
            if (copy != null)
            {
                //先删产品库
                ClientTool.Current.DeleteDocument<TRootDocument>(IndexName, copy.Id);

                //再删事务
                ClientTool.Current.DeleteDocument<TRootDocument>(TranIndexName, copy.Id);
            }
        }

        #endregion

        #region 修改

        protected override void PersistUpdateRoot(TRoot obj)
        {
            // 数据转换
            var doc = MapDocument(obj);

            var id = obj.GetIdentity();
            //先从产品库中加载老文档
            var copy = ClientTool.Current.GetDocumentById<TRootDocument>(IndexName, id);
            copy.OperationType = "u";
            //将备份文档存入事务库
            ClientTool.Current.AddDocument(TranIndexName, copy);

            //修改产品库
            ClientTool.Current.UpdateDocument(IndexName, doc);


            //var doc = MapDocument(obj);
            //var id = obj.GetIdentity();
            //TRootDocument copy = null;
            //var time1 = TimeMonitor.Oversee(() =>
            //{
            //    //先从产品库中加载老文档
            //    copy = ClientTool.Current.GetDocumentsById<TRootDocument>(IndexName, id);
            //    copy.OperationType = "u";
            //});
            //var s1 = time1.GetTime(0).ElapsedMilliseconds;

            //var time2 = TimeMonitor.Oversee(() =>
            //{
            //    //将备份文档存入事务库
            //    ClientTool.Current.AddDocument(TranIndexName, copy);

            //});
            //var s2 = time2.GetTime(0).ElapsedMilliseconds;

            //var time3 = TimeMonitor.Oversee(() =>
            //{
            //    //修改产品库
            //    ClientTool.Current.UpdateDocument(IndexName, doc);
            //});
            //var s3 = time3.GetTime(0).ElapsedMilliseconds;
        }

        public override void OnUpdateCommited(IAggregateRoot obj)
        {
            //删除事务数据
            ClientTool.Current.DeleteDocument<TRootDocument>(TranIndexName, obj.GetIdentity());

            base.OnUpdateCommited(obj);
        }


        private void RollbackUpdate(IAggregateRoot root)
        {
            var id = root.GetIdentity();
            //从事务库中加载修改前的数据
            var copy = ClientTool.Current.GetDocumentById<TRootDocument>(TranIndexName, id);
            RollbackUpdate(copy);
        }


        private void RollbackUpdate(TRootDocument copy)
        {
            if (copy != null)
            {
                //先恢复产品库
                ClientTool.Current.UpdateDocument<TRootDocument>(IndexName,copy);

                //再删事务
                ClientTool.Current.DeleteDocument<TRootDocument>(TranIndexName, copy.Id);
            }
        }

        #endregion

        #region 删除

        protected override void PersistDeleteRoot(TRoot obj)
        {
            var id = obj.GetIdentity();

            //先从库中加载要删除的文档
            var copy = ClientTool.Current.GetDocumentById<TRootDocument>(IndexName, id);

            copy.OperationType = "d";
            //将备份文档存入事务库
            ClientTool.Current.AddDocument(TranIndexName, copy);

            //删除产品库中的数据
            ClientTool.Current.DeleteDocument<TRootDocument>(IndexName, id);
        }

        public override void OnDeleteCommited(IAggregateRoot obj)
        {
            //删除事务数据
            ClientTool.Current.DeleteDocument<TRootDocument>(TranIndexName, obj.GetIdentity());

            base.OnDeleteCommited(obj);
        }

        private void RollbackDelete(IAggregateRoot root)
        {
            var id = root.GetIdentity();
            //从事务库中加载删除前的数据
            var copy = ClientTool.Current.GetDocumentById<TRootDocument>(TranIndexName, id);
            RollbackDelete(copy);
        }

        private void RollbackDelete(TRootDocument copy)
        {
            if (copy != null)
            {
                //防止没有删除，先删除
                ClientTool.Current.DeleteDocument<TRootDocument>(IndexName, copy.Id);

                //再添加
                ClientTool.Current.AddDocument(IndexName, copy);

                //最后删事务
                ClientTool.Current.DeleteDocument<TRootDocument>(TranIndexName, copy.Id);
            }
        }

        #endregion

        public override void OnRollback(object sender, RepositoryRollbackEventArgs e)
        {

            switch(e.Action)
            {
                case RepositoryAction.Add:
                    {
                        this.RollbackAdd(e.Target);
                        break;
                    }
                case RepositoryAction.Update:
                    {
                        this.RollbackUpdate(e.Target);
                        break;
                    }
                case RepositoryAction.Delete:
                    {
                        this.RollbackDelete(e.Target);
                        break;
                    }
            }

            base.OnRollback(sender, e);
        }

        public void Restore()
        {
            var ids = ClientTool.Current.GetDocumentIds<TRootDocument>(TranIndexName);

            foreach(var id in ids)
            {
                var doc = ClientTool.Current.GetDocumentById<TRootDocument>(TranIndexName, id);
                switch(doc.OperationType)
                {
                    case "c": RollbackAdd(doc); break;
                    case "u": RollbackUpdate(doc); break;
                    case "d": RollbackDelete(doc); break;
                }
            }

        }
    }
}
