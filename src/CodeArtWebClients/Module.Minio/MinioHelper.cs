using Minio;
using Minio.DataModel;
using Minio.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module.Minio
{
    public class MinioHelper : IDisposable
    {
        private MinioClient _client;

        private (string EndPoint, int Port, string AccessKey, string SecretKey) GetConfig()
        {
            const string configHost = "minio_host";
            string endPoint = ConfigurationManager.AppSettings[configHost];
            //if (endPoint == null) throw new ApplicationException(string.Format(Strings.NotSetupMinio, configHost));

            const string configPort = "minio_port";
            string port = ConfigurationManager.AppSettings[configPort];
            //if (port == null) throw new ApplicationException(string.Format(Strings.NotSetupMinio, configPort));

            const string configAccessKey = "minio_accessKey";
            string accessKey = ConfigurationManager.AppSettings[configAccessKey];
            //if (accessKey == null) throw new ApplicationException(string.Format(Strings.NotSetupMinio, configAccessKey));

            const string configSecretKey = "minio_secretKey";
            string secretKey = ConfigurationManager.AppSettings[configSecretKey];
            //if (secretKey == null) throw new ApplicationException(string.Format(Strings.NotSetupMinio, configSecretKey));

            return (endPoint, int.Parse(port), accessKey, secretKey);
        }

        public MinioHelper(string endPoint = null, int port = 0, string accessKey = null, string secretKey = null)
        {
            if (string.IsNullOrEmpty(endPoint) || port == 0 ||
                string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
            {
                var config = GetConfig();
                endPoint = config.EndPoint;
                port = config.Port;
                accessKey = config.AccessKey;
                secretKey = config.SecretKey;
            }

            _client = new MinioClient()
                    .WithEndpoint(endPoint, port)
                    .WithCredentials(accessKey, secretKey)
                    .Build();
        }

        #region 操作存储桶

        /// <summary>
        /// 判断桶存不存在
        /// </summary>
        public async Task<bool> BucketExistsAsync(string bucketName)
        {
            try
            {
                var args = new BucketExistsArgs().WithBucket(bucketName);
                var found = await _client.BucketExistsAsync(args);
                return found;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
                return false;
            }
        }

        /// <summary>
        /// 创建存储桶
        /// </summary>
        public async Task<bool> MakeBucket(string bucketName)
        {
            try
            {
                bool found = await this.BucketExistsAsync(bucketName);
                if (found) return true;

                await _client.MakeBucketAsync(new MakeBucketArgs()
                                            .WithBucket(bucketName)
                                            );
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
                return false;
            }
        }

        /// <summary>
        /// 列出所有的存储桶
        /// </summary>
        /// <returns></returns>
        public async Task<ListAllMyBucketsResult> ListBucketsAsync()
        {
            try
            {
                return await _client.ListBucketsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
                return null;
            }
        }

        /// <summary>
        /// 删除桶
        /// </summary>
        public async Task RemoveBucketAsync(string bucketName)
        {
            try
            {
                bool found = await this.BucketExistsAsync(bucketName);
                if (!found) return;

                await _client.RemoveBucketAsync(
                    new RemoveBucketArgs()
                    .WithBucket(bucketName)
                );
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }
        }

        /// <summary>
        /// 列出存储桶中的所有对象
        /// </summary>
        public async Task<IObservable<Item>> ListObjectsAsync(string bucketName, string prefix = null, bool recursive = true)
        {
            try
            {
                bool found = await this.BucketExistsAsync(bucketName);
                if (!found) return null;

                var listArgs = new ListObjectsArgs()
                        .WithBucket(bucketName)
                        .WithPrefix(prefix)
                        .WithRecursive(recursive);

                var observable = await Task.Run(() => _client.ListObjectsAsync(listArgs));
                return observable;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
                return null;
            }
        }

        /// <summary>
        /// 列出存储桶中未完整上传的对象
        /// </summary>
        public async Task<IObservable<Upload>> ListIncompleteUploads(string bucketName, string prefix = null, bool recursive = true)
        {
            try
            {
                bool found = await this.BucketExistsAsync(bucketName);
                if (!found) return null;

                var listArgs = new ListIncompleteUploadsArgs()
                    .WithBucket(bucketName)
                    .WithPrefix(prefix)
                    .WithRecursive(recursive);

                var observable = _client.ListIncompleteUploads(listArgs);
                return observable;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
                return null;
            }
        }

        #endregion

        #region 存储桶策略

        /// <summary>
        /// 获取存储桶的访问策略
        /// </summary>
        public async Task<string> GetPolicy(string bucketName)
        {
            try
            {
                bool found = await this.BucketExistsAsync(bucketName);
                if (!found) return null;

                var args = new GetPolicyArgs()
                    .WithBucket(bucketName);
                string policyJson = await _client.GetPolicyAsync(args);

                return policyJson;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
                return null;
            }
        }

        /// <summary>
        /// 针对存储桶设置访问策略
        /// 访问策略类似字符串  $@"{{""Version"":""2012-10-17"",""Statement"":[{{""Action"":[""s3:GetBucketLocation""],""Effect"":""Allow"",""Principal"":{{""AWS"":[""*""]}},""Resource"":[""arn:aws:s3:::{bucketName}""],""Sid"":""""}},{{""Action"":[""s3:ListBucket""],""Condition"":{{""StringEquals"":{{""s3:prefix"":[""foo"",""prefix/""]}}}},""Effect"":""Allow"",""Principal"":{{""AWS"":[""*""]}},""Resource"":[""arn:aws:s3:::{bucketName}""],""Sid"":""""}},{{""Action"":[""s3:GetObject""],""Effect"":""Allow"",""Principal"":{{""AWS"":[""*""]}},""Resource"":[""arn:aws:s3:::{bucketName}/foo*"",""arn:aws:s3:::{bucketName}/prefix/*""],""Sid"":""""}}]}}";
        /// </summary>
        public async Task<bool> SetPolicy(string bucketName, string policyJson)
        {
            try
            {
                bool found = await this.BucketExistsAsync(bucketName);
                if (!found) return false;

                var args = new SetPolicyArgs()
                    .WithBucket(bucketName)
                    .WithPolicy(policyJson);
                await _client.SetPolicyAsync(args);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
                return false;
            }
        }

        #endregion

        #region 操作文件对象

        /// <summary>
        /// 从桶下载文件到本地
        /// </summary>
        public async Task<bool> GetFileObject(string bucketName, string objectName, string filePath, ServerSideEncryption sse = null)
        {
            try
            {
                bool found = await this.BucketExistsAsync(bucketName);
                if (!found) return false;

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                var args = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithFile(filePath)
                    .WithServerSideEncryption(sse);

                await _client.GetObjectAsync(args).ConfigureAwait(false);
                return true;
            }
            catch (MinioException e)
            {
                Console.WriteLine($"Exception: {e}");
                return false;
            }
        }

        /// <summary>
        /// 上传本地文件至存储桶
        /// </summary>
        public async Task<bool> PutFileObject(string bucketName, string objectName, string filePath,
            string contentType = "application/octet-stream", Dictionary<string, string> metaData = null)
        {
            try
            {
                bool found = await this.BucketExistsAsync(bucketName);
                if (!found) return false;

                var args = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithContentType(contentType)
                    .WithHeaders(metaData)
                    .WithFileName(filePath);
                await _client.PutObjectAsync(args).ConfigureAwait(false);

                return true;
            }
            catch (MinioException e)
            {
                Console.WriteLine($"Exception: {e}");
                return false;
            }
        }

        #endregion


        #region Presigned操作

        /// <summary>
        /// 为HTTP GET操作生成一个预签名的URL。即使存储桶是私有的，浏览器/移动客户端也可能指向该URL直接下载对象。
        /// 该预签名URL可以具有关联的到期时间（以秒为单位），在此时间之后，它将不再起作用。默认有效期设置为1天
        /// </summary>
        public async Task<string> PresignedGetObjectUrl(string bucketName, string objectName, int expiry = 3600 * 24)
        {
            try
            {
                bool found = await this.BucketExistsAsync(bucketName);
                if (!found) return null;

                //var reqParams = new Dictionary<string, string> { { "response-content-type", "application/json" } };
                var reqParams = new Dictionary<string, string> { };
                var args = new PresignedGetObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                        .WithExpiry(expiry)
                        .WithHeaders(reqParams);
                var url = await _client.PresignedGetObjectAsync(args);
                return url;

            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
                return null;
            }
        }

        /// <summary>
        /// 生成一个给HTTP PUT请求用的presigned URL。浏览器/移动端的客户端可以用这个URL进行上传，即使其所在的存储桶是私有的。
        /// 这个presigned URL可以设置一个失效时间，默认值是1天。
        /// </summary>
        public async Task<string> PresignedPutObjectUrl(string bucketName, string objectName, int expiry = 3600 * 24)
        {
            try
            {
                bool found = await this.BucketExistsAsync(bucketName);
                if (!found) return null;

                var args = new PresignedPutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithExpiry(expiry);
                var url = await _client.PresignedPutObjectAsync(args);
                return url;

            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
                return null;
            }
        }

        #endregion


        #region 操作对象

        /// <summary>
        /// 返回对象数据的流
        /// </summary>
        public async Task<bool> GetObjectAsync(string bucketName, string objectName, Action<Stream> callback)
        {
            try
            {
                bool found = await this.BucketExistsAsync(bucketName);
                if (!found) return false;

                var objectStatArgs = new StatObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName);
                await _client.StatObjectAsync(objectStatArgs);

                var getObjectArgs = new GetObjectArgs().WithBucket(bucketName).WithObject(objectName).WithCallbackStream(callback);

                await _client.GetObjectAsync(getObjectArgs);
                return true;
            }
            catch (MinioException e)
            {
                Console.WriteLine($"Exception: {e}");
                return false;
            }
        }

        /// <summary>
        /// 下载对象指定区域的字节数组做为流。offset和length都必须传
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetObjectAsync(string bucketName, string objectName, long offset, long length, Action<Stream> callback)
        {
            try
            {
                bool found = await this.BucketExistsAsync(bucketName);
                if (!found) return false;

                var objectStatArgs = new StatObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName);
                await _client.StatObjectAsync(objectStatArgs);

                var getObjectArgs = new GetObjectArgs().
                    WithBucket(bucketName).
                    WithObject(objectName).
                    WithOffsetAndLength(offset, length).
                    WithCallbackStream(callback);

                await _client.GetObjectAsync(getObjectArgs);
                return true;
            }
            catch (MinioException e)
            {
                Console.WriteLine($"Exception: {e}");
                return false;
            }
        }


        /// <summary>
        /// 通过Stream上传对象
        /// </summary>
        public async Task<bool> PutObjectAsync(string bucketName, string objectName, Stream data, long size,
            string contentType = "application/octet-stream", Dictionary<string, string> metaData = null)
        {
            try
            {
                bool found = await this.BucketExistsAsync(bucketName);
                if (!found) return false;

                var args = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(data)
                    .WithObjectSize(size)
                    .WithContentType(contentType)
                    .WithHeaders(metaData);

                await _client.PutObjectAsync(args);
                return true;
            }
            catch (MinioException e)
            {
                Console.WriteLine($"Exception: {e}");
                return false;
            }
        }

        /// <summary>
        /// 获取对象的元数据
        /// </summary>
        public async Task<ObjectStat> StatObjectAsync(string bucketName, string objectName)
        {
            try
            {
                bool found = await this.BucketExistsAsync(bucketName);
                if (!found) return null;

                var objectStatArgs = new StatObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName);
                var stat = await _client.StatObjectAsync(objectStatArgs);
                return stat;
            }
            catch (MinioException e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 从objectName指定的对象中将数据拷贝到destObjectName指定的对象
        /// </summary>
        public async Task<bool> CopyObject(string srcBucketName, string srcObjectName, string destBucketName, string destObjectName,
            CopyConditions copyConditions = null, ServerSideEncryption sseSrc = null, ServerSideEncryption sseDest = null)
        {
            try
            {
                bool foundSrc = await this.BucketExistsAsync(srcBucketName);
                bool foundDest = await this.BucketExistsAsync(destBucketName);

                if (!foundSrc || !foundDest) return false;
                var cpSrcArgs = new CopySourceObjectArgs()
                    .WithBucket(srcBucketName)
                    .WithObject(srcObjectName)
                    .WithCopyConditions(copyConditions)
                    .WithServerSideEncryption(sseSrc);
                var args = new CopyObjectArgs()
                    .WithBucket(destBucketName)
                    .WithObject(destObjectName)
                    .WithCopyObjectSource(cpSrcArgs)
                    .WithServerSideEncryption(sseDest);

                await _client.CopyObjectAsync(args);

                return true;
            }
            catch (MinioException e)
            {
                Console.WriteLine($"Exception: {e}");
                return false;
            }
        }

        /// <summary>
        /// 删除一个对象
        /// </summary>
        public async Task<bool> RemoveObject(string bucketName, string objectName)
        {
            try
            {
                bool found = await this.BucketExistsAsync(bucketName);
                if (!found) return false;

                var args = new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName);

                await _client.RemoveObjectAsync(args);
                return true;
            }
            catch (MinioException e)
            {
                Console.WriteLine($"Exception: {e}");
                return false;
            }
        }

        /// <summary>
        /// 删除多个对象
        /// </summary>
        public async Task<IObservable<DeleteError>> RemoveObjects(string bucketName, List<string> objects)
        {
            try
            {
                bool found = await this.BucketExistsAsync(bucketName);
                if (!found || objects == null || objects.Count == 0) return null;

                var args = new RemoveObjectsArgs()
                    .WithBucket(bucketName)
                    .WithObjects(objects);

                IObservable<DeleteError> result = await _client.RemoveObjectsAsync(args);
                return result;
            }
            catch (MinioException e)
            {
                Console.WriteLine($"Exception: {e}");
                return null;
            }
        }

        /// <summary>
        /// 删除一个未完整上传的对象
        /// </summary>
        public async Task<bool> RemoveIncompleteUpload(string bucketName, string objectName)
        {
            try
            {
                bool found = await this.BucketExistsAsync(bucketName);
                if (!found) return false;

                var args = new RemoveIncompleteUploadArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName);

                await _client.RemoveIncompleteUploadAsync(args);

                return true;
            }
            catch (MinioException e)
            {
                Console.WriteLine($"Exception: {e}");
                return false;
            }
        }

        #endregion


        public void Dispose()
        {

        }

    }
}
