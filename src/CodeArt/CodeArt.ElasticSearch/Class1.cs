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

namespace CodeArt.ElasticSearch
{
    public class Class1
    {
        private string _serversString;

        private Pool<ElasticClientWrapper> _clientPool;

        public Class1(string serversString)
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
                result = client.IndexExists(indexsString).Exists;
            });

            return result;
        }

        public bool ExistIndex(string indexName)
        {
            var result = false;
            this.Using((client) =>
            {
                result = client.IndexExists(indexName).Exists;
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
                result = client.DeleteIndex(descriptor).Acknowledged;
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
        public void CreateIndex(string indexName)
        {
            ArgumentAssert.IsNotNull(indexName, "indexName");

            // 获取客户端
            this.Using((client) =>
            {
                // 创建索引
                IIndexState indexState = new IndexState()
                {
                    Settings = new IndexSettings()
                    {
                        NumberOfReplicas = 1,//副本数
                        NumberOfShards = 5//分片数
                    }
                };

                // 创建索引
                client.CreateIndex(indexName, p => p.InitializeUsing(indexState));
                //var ret = client.CreateIndex(indexName, p => p.InitializeUsing(indexState));
                //result = ret.Acknowledged;
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
        public void AddDocument<T>(string indexName, T document) where T : class
        {
            if (string.IsNullOrEmpty(indexName) || document == null)
                throw new ApplicationException("索引名称或文档不能为空");

            var result = false;

            this.Using((client) =>
            {
                var response = client.Index<T>(document, descriptor => descriptor.Index(indexName));
                result = response.IsValid;
            });

            if (!result)
                throw new ApplicationException(string.Format("向索引{0}添加文档失败", indexName));
        }

        /// <summary>
        /// 删除文档
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public void DeleteDocument<T>(string indexName, Id id) where T : class
        {
            if (string.IsNullOrEmpty(indexName) || id == null)
                throw new ApplicationException("索引名称或数据ID不能为空");

            var result = false;

            this.Using((client) =>
            {
                DocumentPath<T> deletePath = new DocumentPath<T>(id);
                var response = client.Delete(deletePath, descriptor => descriptor.Index(indexName));
                result = response.IsValid;
            });

            //if (!result)
            //    throw new ApplicationException(string.Format("从索引{0}删除文档失败", indexName));
        }

        #endregion

        #region 查询文档

        public (IEnumerable<T> Documents, long Total) GetDocuments<T>(string indexName,
                                                int pageIndex, int pageSize,
                                                Func<SearchDescriptor<T>, SearchDescriptor<T>> getDescriptor) where T : class
        {
            // 转换参数
            var from = pageSize * (pageIndex - 1);
            var size = pageSize;

            IReadOnlyCollection<T> docs = null;
            long total = 0;

            // 获取客户端
            this.Using((client) =>
            {
                var index = Indices.Parse(indexName);
                Func<SearchDescriptor<T>, ISearchRequest> selector = s => getDescriptor(s.Index(index).From(from).Size(size));
                var response = client.Search<T>(selector);
                docs = response.Documents;
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
                                              Func<QueryContainerDescriptor<T>, QueryContainer> query) where T : class
        {
            return GetDocuments<T>(indexName, pageIndex, pageSize, (descriptor) =>
            {
                return descriptor.Query((q) => query(q));
            });
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


        public static readonly Class1 Current;

        static Class1()
        {
            var serverString = ConfigurationManager.AppSettings["esServers"];
            if (string.IsNullOrEmpty(serverString)) return;
            Current = new Class1(serverString);
        }

    }
}
