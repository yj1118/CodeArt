using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

using Elasticsearch.Net;
using Nest;

using CodeArt;
using CodeArt.Concurrent;
using System.Threading;
using CodeArt.Log;
using CodeArt.DTO;

namespace CodeArt.Search
{
    public class ClientTool
    {
        private string _serversString;

        private Pool<ElasticClientWrapper> _clientPool;

        public ClientTool(string serversString)
        {
            _serversString = serversString;

            _clientPool = new Pool<ElasticClientWrapper>(() =>
            {
                return CreateElasticClient(_serversString);
            }, (writer, phase) =>
            {
                return true;
            }, new PoolConfig()
            {
                MaxRemainTime = 300 //闲置时间300秒
            });
        }

        public void Using(Action<ElasticClient> action)
        {
            using (var temp = _clientPool.Borrow())
            {
                var item = temp.Item;
                action(item.Client);
            }
        }

        #region 操作索引

        /// <summary>
        /// 判断一个或多个索引是否存在
        /// </summary>
        public bool ExistIndexes(IEnumerable<string> indexes)
        {
            AssertIndexes(indexes);

            var result = false;
            this.Using((client) =>
            {
                string indexsString = string.Join(",", indexes);
                result = client.Indices.Exists(indexsString).Exists;
            });

            return result;
        }

        /// <summary>
        /// 判断一个索引是否存在
        /// </summary>
        public bool ExistIndex(string indexName)
        {
            var result = false;
            this.Using((client) =>
            {
                result = client.Indices.Exists(indexName).Exists;
            });

            return result;
        }

        /// <summary>
        /// 删除单个或多个索引(如果多个索引中有不存在的索引，执行失败)
        /// </summary>
        public bool DeleteIndexes(IEnumerable<string> indexes)
        {
            AssertIndexes(indexes);

            var result = false;

            this.Using((client) =>
            {
                string indexsString = string.Join(",", indexes);
                var descriptor = new DeleteIndexDescriptor(indexsString);
                result = client.Indices.Delete(descriptor).Acknowledged;
            });

            return result;
        }

        public static void AssertIndexes(IEnumerable<string> indexes)
        {
            if (indexes != null)
                indexes = indexes.Where(s => !string.IsNullOrEmpty(s));

            if (indexes == null || indexes.Count() == 0)
                throw new ApplicationException("索引项不能为空");
        }

        /// <summary>
        /// 创建索引，对于已经创建的不会重复创建
        /// </summary>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public void CreateIndex<T>(string indexName) where T : class
        {
            ArgumentAssert.IsNotNull(indexName, "indexName");

            if (ExistIndex(indexName))
                return;

            // 获取客户端
            this.Using((client) =>
            {
                // 创建索引
                IIndexState indexState = new IndexState()
                {
                    Settings = new IndexSettings()
                    {
                        NumberOfReplicas = 1,//副本数
                        NumberOfShards = 1  //分片数
                    }
                };

                // 创建索引
                //var descriptor = new CreateIndexDescriptor(indexName).InitializeUsing(indexState).Mappings(ms => ms.Map<T>(m => m.AutoMap()));
                //client.CreateIndex(descriptor);
                Func<CreateIndexDescriptor, ICreateIndexRequest> func = x => x.InitializeUsing(indexState).Map<T>(m => m.AutoMap());
                client.Indices.Create(indexName, func);
            });
        }

        #endregion

        #region 操作文档

        /// <summary>
        /// 添加一条数据到指定的索引，类型由T指定
        /// 如果Mapping中已有对应的类型，则会与mapping一致（类型变化会自动修改mapping）
        /// 如果Mapping中没有对于类型，会生成对应mapping
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public void AddDocument<T>(string indexName, T document) where T : class, IDocument
        {
            if (string.IsNullOrEmpty(indexName) || document == null)
                throw new ApplicationException("索引名称或文档不能为空");

            var result = false;

            // 再入库
            this.Using((client) =>
            {
                var response = client.Index<T>(document, descriptor => descriptor.Index(indexName).Refresh(Refresh.True));
                //var response = client.Index<T>(document, descriptor => descriptor.Index(indexName);
                result = response.IsValid;

                if (!result)
                    throw new UserUIException(string.Format("向索引{0}添加文档失败，{1}", indexName, response.OriginalException.Message));
            });


        }

        /// <summary>
        /// 批量添加多条数据到指定的索引，类型由T指定
        /// 返回true表示全部添加成功，返回false表示中间出现了错误,
        /// 错误不会抛出，而是写入日志， 以免中断程序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="document"></param>
        public bool BulkDocuments<T>(string indexName, IEnumerable<T> documents) where T : class, IDocument
        {
            if (string.IsNullOrEmpty(indexName) || documents == null)
                throw new ApplicationException("索引名称或文档不能为空");

            const int size = 100;
            Exception exception = null;

            this.Using((client) =>
            {
                var bulkAll = client.BulkAll<T>(documents, f => f.MaxDegreeOfParallelism(8).BackOffTime(TimeSpan.FromSeconds(10))
                                                                        .BackOffRetries(2).Size(size).RefreshOnCompleted().Index(indexName)
                                                                        .BufferToBulk((r, buffer) => r.IndexMany(buffer)));
                var countdownEvent = new CountdownEvent(1);

                void OnCompleted()
                {
                    countdownEvent.Signal();
                }

                var bulkAllObserver = new BulkAllObserver(
                    (response) =>
                    {
                    },
                    ex =>
                    {
                        exception = ex;
                        countdownEvent.Signal();
                    },
                    OnCompleted);

                bulkAll.Subscribe(bulkAllObserver);

                countdownEvent.Wait();

            });

            if (exception != null)
            {
                // 错误写入日志，不抛出异常
                Logger.Fatal(exception);
                return false;
            }
            else
            {
                return true;
            }
        }

        public void DeleteDocument<T>(string indexName, object id) where T : class, IDocument
        {
            DeleteDocument<T>(indexName, (Id)id.ToString());
        }

        /// <summary>
        /// 删除文档
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private void DeleteDocument<T>(string indexName, Id id) where T : class,IDocument
        {
            if (string.IsNullOrEmpty(indexName) || id == null)
                throw new ApplicationException("索引名称或数据ID不能为空");

            var result = false;

            this.Using((client) =>
            {
                DocumentPath<T> deletePath = new DocumentPath<T>(id);
                var response = client.Delete(deletePath, descriptor => descriptor.Index(indexName).Refresh(Refresh.True));
                //var response = client.Delete(deletePath, descriptor => descriptor.Index(indexName);
                result = response.IsValid;
            });
        }

        /// <summary>
        /// 更新文档，没入库的文档不会做任何操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="id"></param>
        /// <param name="document"></param>
        private bool UpdateDocument<T>(string indexName, Id id, T document) where T : class
        {
            if (string.IsNullOrEmpty(indexName) || document == null)
                throw new ApplicationException("索引名称或文档不能为空");

            var result = false;

            this.Using((client) =>
            {
                var docPath = new DocumentPath<T>(id);
                var ret = client.Update(docPath, descriptor => descriptor.Index(indexName).Doc(document).Refresh(Refresh.True));
                //var ret = client.Update(docPath, descriptor => descriptor.Index(indexName).Doc(document);
                result = ret.IsValid;
            });

            return result;
        }

        public bool UpdateDocument<T>(string indexName, T document) where T : class, IDocument
        {
           return UpdateDocument<T>(indexName, document.Id, document);
        }

        #endregion

        #region 查询文档

        public T GetDocumentById<T>(string indexName, object id) where T : class
        {
            using (var temp = ListPool<Id>.Borrow())
            {
                var ids = temp.Item;
                ids.Add(id.ToString());
                var result = GetDocumentsByIds<T>(indexName, ids);
                return result.Documents.FirstOrDefault();
            }
        }

        private (IEnumerable<T> Documents, long Count) GetDocumentsByIds<T>(string indexName, IEnumerable<Id> ids) where T : class
        {
            IReadOnlyCollection<T> docs = null;
            long count = 0;

            QueryContainer idsQuery = new IdsQuery { Values = ids };

            this.Using((client) =>
            {
                var index = Indices.Parse(indexName);

                var response = client.Search<T>(s => s.Index(index)
                        .Query(q => idsQuery));

                docs = response.Documents;
                count = response.Total;
            });

            return (docs, count);
        }

        public (IEnumerable<T> Documents, long Count) GetDocumentsByIds<T>(string indexName, IEnumerable<object> ids) where T : class
        {
            (IEnumerable<T> Documents, long Count) result;

            using (var temp = ListPool<Id>.Borrow())
            {
                var nIds = temp.Item;
                foreach(var id in ids)
                {
                    nIds.Add(id.ToString());
                }

                result = GetDocumentsByIds<T>(indexName, nIds);
            }

            return result;
        }

        public (IEnumerable<T> Documents, long Total) GetDocuments<T>(string indexName,
                                                int pageIndex, int pageSize,
                                                Func<SearchDescriptor<T>, SearchDescriptor<T>> getDescriptor) where T : class
        {
            // 转换参数
            var from = pageSize * (pageIndex - 1);
            var size = pageSize;

            List<T> docs = new List<T>();

            long total = 0;

            // 获取客户端
            this.Using((client) =>
            {
                var index = Indices.Parse(indexName);
                Func<SearchDescriptor<T>, ISearchRequest> selector = s => getDescriptor(s.Index(index).From(from).Size(size));
                var response = client.Search<T>(selector);
                var hits = response.Hits;
                foreach (var hit in hits)
                {
                    var data = hit.Source as Dictionary<string, object>;
                    if (data != null)
                    {
                        data.Add("id", hit.Id);
                        T doc = (T)(object)data;
                        docs.Add(doc);
                    }
                }
                total = response.Total;
            });

            return (docs, total);
        }

        /// <summary>
        /// 多条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public (IEnumerable<T> Documents, long Total) GetDocuments<T>(string indexName,
                                              int pageIndex, int pageSize,
                                              Func<QueryContainerDescriptor<T>, QueryContainer> query, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort) where T : class
        {
            return GetDocuments<T>(indexName, pageIndex, pageSize, (descriptor) =>
            {
                if(sort != null)
                {
                    return descriptor.Query((q) => query(q)).Sort(sort);
                }
                return descriptor.Query((q) => query(q));
            });
        }


        public (IEnumerable<T> Documents, long Total) GetDocuments<T>(string indexName,
                                                Func<SearchDescriptor<T>, SearchDescriptor<T>> getDescriptor) where T : class
        {
            IReadOnlyCollection<T> docs = null;
            long total = 0;

            // 获取客户端
            this.Using((client) =>
            {
                var index = Indices.Parse(indexName);
                Func<SearchDescriptor<T>, ISearchRequest> selector = s => getDescriptor(s.Index(index));
                var response = client.Search<T>(selector);
                total = response.Total;

                selector = s => getDescriptor(s.Index(index).From(0).Size((int)total));
                response = client.Search<T>(selector);
                docs = response.Documents;
            });

            return (docs, total);
        }

        /// <summary>
        /// 多条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public (IEnumerable<T> Documents, long Total) GetDocuments<T>(string indexName,
                                              Func<QueryContainerDescriptor<T>, QueryContainer> query, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort) where T : class
        {
            return GetDocuments<T>(indexName, (descriptor) =>
            {
                if (sort != null)
                {
                    return descriptor.Query((q) => query(q)).Sort(sort);
                }
                return descriptor.Query((q) => query(q));
            });
        }


        /// <summary>
        /// 获取索引库中全部id,该方法尽量不要使用，请用翻页查数据，该方法仅用于数据总数比较少的获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public IEnumerable<Id> GetDocumentIds<T>(string indexName) where T : class
        {
            List<Id> ids = new List<Id>();

            Time time = "30s"; // 内置最大处理时间

            this.Using((client) =>
            {
                var index = Indices.Parse(indexName);

                var scanResults = client.Search<T>(s => s.Index(index).From(0).Size(1).MatchAll().Scroll(time));

                if (scanResults.Documents.Any())
                {
                    foreach (var doc in scanResults.Documents)
                    {
                        var data = doc as IDocument;
                        ids.Add(data.Id);
                    }

                    var scrollId = scanResults.ScrollId;
                    var results = client.Scroll<T>(time, scrollId);
                    while (results.Documents.Any())
                    {
                        var docs = results.Documents;

                        foreach (var doc in docs)
                        {
                            var data = doc as IDocument;
                            ids.Add(data.Id);
                        }

                        results = client.Scroll<T>(time, scrollId);
                    }
                }
            });

            return ids;
        }

        #endregion

        #region 只查询文档的个数

        public long GetDocumentCount<T>(string indexName, Func<CountDescriptor<T>, CountDescriptor<T>> getDescriptor) where T : class
        {
            long count = 0;

            // 获取客户端
            this.Using((client) =>
            {
                var index = Indices.Parse(indexName);

                Func<CountDescriptor<T>, ICountRequest> selector = s => getDescriptor(s.Index(index));

                var response = client.Count<T>(selector);

                count = response.Count;
            });

            return count;
        }

        public long GetDocumentCount<T>(string indexName, Func<QueryContainerDescriptor<T>, QueryContainer> query) where T : class
        {
            return GetDocumentCount<T>(indexName, (descriptor) =>
            {
                return descriptor.Query((q) => query(q));
            });
        }

        #endregion

        #region 文档的聚合操作

        public (IEnumerable<DTObject> Aggregations, long Total) GetAggregations<T>(string indexName,
                                        Func<SearchDescriptor<T>, SearchDescriptor<T>> getDescriptor, 
                                        Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>> getAggDescriptorn) where T : class
        {
            long total = 0;
            List<DTObject> aggs = new List<DTObject>();

            // 获取客户端
            this.Using((client) =>
            {
                var index = Indices.Parse(indexName);
                Func<SearchDescriptor<T>, ISearchRequest> selector = s => getDescriptor(s.Index(index).Size(0).Aggregations(getAggDescriptorn));

                var response = client.Search<T>(selector);
                var hits = response.Aggregations;

                foreach (var hit in hits)
                {
                    DTObject agg = DTObject.Create();
                    List<DTObject> datas = new List<DTObject>();
                    agg.SetValue("name", hit.Key);
                    var ba = hit.Value as BucketAggregate;
                    foreach (var item in ba.Items)
                    {
                        var bucket = item as KeyedBucket<dynamic>;

                        DTObject data = DTObject.Create();
                        data.SetValue("key", bucket.Key);
                        data.SetValue("value", bucket.DocCount);
                        datas.Add(data);
                    }
                    agg.SetValue("datas", datas);
                    aggs.Add(agg);
                }
                total = response.Total;
            });

            return (aggs, total);
        }


        #endregion


        private static ElasticClientWrapper CreateElasticClient(string serversString)
        {
            ArgumentAssert.IsNotNullOrEmpty(serversString, "serversString");

            var servers = serversString.Split(';').Select((server) =>
            {
                CheckServer(server);
                return server;
            });

            var nodes = servers.Select((server) => new Uri(server));

            var connectionPool = new SniffingConnectionPool(nodes);
            var settings = new ConnectionSettings(connectionPool);

            var client = new ElasticClient(settings);
            return new ElasticClientWrapper(client);

            void CheckServer(string server)
            {
                Regex regex = new Regex(@"http+://[^\s]*");
                Match match = regex.Match(server);

                if (!match.Success)
                {
                    throw new ApplicationException("ElasticSearch的服务端地址错误:" + server);
                }
            }
        }


        private sealed class ElasticClientWrapper : IDisposable
        {
            public ElasticClient Client
            {
                get;
                private set;
            }

            public ElasticClientWrapper(ElasticClient client)
            {
                this.Client = client;
            }

            public void Dispose()
            {
                this.Client.ConnectionSettings.ConnectionPool.Dispose();
                this.Client.ConnectionSettings.Dispose();
            }
        }


        public static readonly ClientTool Current;

        static ClientTool()
        {
            var serverString = SearchConfiguration.Current.GetDefatultServer();
            if (string.IsNullOrEmpty(serverString)) return;
            Current = new ClientTool(serverString);
        }

    }
}
