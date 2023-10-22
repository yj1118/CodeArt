using Minio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;

namespace RPC.Common.Minio
{
    public class Store
    {
        private bool _useSSL;
        private string _domain;
        private MinioClient _client;


        private void TryCreateBucket(string bucket)
        {
            var policy = string.Format("{{\"Version\":\"2012-10-17\", \"Statement\":[{{\"Effect\":\"Allow\", \"Principal\":{{\"AWS\":[\"*\"]}},\"Action\":[\"s3:GetBucketLocation\",\"s3:ListBucket\",\"s3:ListBucketMultipartUploads\"],\"Resource\":[\"arn:aws:s3:::{0}\"]}},{{\"Effect\":\"Allow\",\"Principal\":{{\"AWS\":[\"*\"]}}, \"Action\":[\"s3:ListMultipartUploadParts\",\"s3:PutObject\",\"s3:AbortMultipartUpload\",\"s3:DeleteObject\",\"s3:GetObject\"],\"Resource\":[\"arn:aws:s3:::{0}/*\"]}}]}}", bucket);

            var exist = _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucket)).Result;
            if (exist) return;

            _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket).WithLocation("cn-wuhan-1")).Wait();
            _client.SetPolicyAsync(new SetPolicyArgs().WithBucket(bucket).WithPolicy(policy)).Wait();
        }

        private static string GetKey(string key, string name, string bucket)
        {
            return !string.IsNullOrEmpty(key) ? name : string.Format("{0}_{1}", bucket, name);
        }

        private static string GetSaveName(string key, string extension)
        {
            if (!string.IsNullOrEmpty(key)) return string.IsNullOrEmpty(extension) ? key : string.Format("{0}{1}", key, extension);
            return string.IsNullOrEmpty(extension) ? Guid.NewGuid().ToString() : string.Format("{0}{1}", Guid.NewGuid().ToString(), extension);
        }

        public string Save(string fileName, string bucket)
        {
            return Save(fileName, bucket, string.Empty, string.Empty);
        }

        public string Save(string fileName, string bucket, string key, string extension)
        {
            TryCreateBucket(bucket);
            var name = string.Empty;
            if (!string.IsNullOrEmpty(key)) name = key;
            else
            {
                var extname = string.IsNullOrEmpty(extension) ? Path.GetExtension(fileName) : extension;
                name = string.Format("{0}{1}", Guid.NewGuid().ToString(), extname);
            }

            PutObjectArgs args = new PutObjectArgs().WithBucket(bucket).WithObject(name).WithContentType("application/octet-stream").WithFileName(fileName);
            _client.PutObjectAsync(args).Wait();
            return GetKey(key, name, bucket);
        }

        public string SaveStream(Stream stream, string bucket, string key, string extension)
        {
            TryCreateBucket(bucket);
            var name = GetSaveName(key, extension);
            var args = new PutObjectArgs().WithBucket(bucket).WithObject(name).WithStreamData(stream).WithObjectSize(stream.Length).WithContentType("application/octet-stream");
            _client.PutObjectAsync(args).Wait();
            return GetKey(key, name, bucket);
        }

        private string _Load(string key, string category)
        {
            var (bucket, name) = ParseKey(key, category);
            var url = string.Format("{0}://{1}/{2}/{3}", _useSSL ? "https" : "http", _domain, bucket, name);
            return url;
        }

        public string Load(string key)
        {
            return _Load(key, string.Empty);
        }

        public string Load(string key, string category)
        {
            return _Load(key, category);
        }

        private Stream _LoadStream(string key, string category)
        {
            var (bucket, name) = ParseKey(key, category);
            Stream result = null;
            _client.GetObjectAsync(new GetObjectArgs().WithBucket(bucket).WithObject(name).WithCallbackStream((stream) =>
            {
                result = stream;
            })).Wait();

            return result;
        }

        public Stream LoadStream(string key)
        {
            return _LoadStream(key, string.Empty);
        }

        public Stream LoadStream(string key, string category)
        {
            return _LoadStream(key, category);
        }

        private bool _Exist(string key, string category)
        {
            try
            {
                var (bucket, name) = ParseKey(key, category);
                var exist = _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucket)).Result;
                if (!exist) return false;

                _client.StatObjectAsync(new StatObjectArgs().WithBucket(bucket).WithObject(name)).Wait();
                return true;
            }
            catch
            {
                return false;
            }
        }


        public bool Exist(string key)
        {
            return _Exist(key, string.Empty);
        }

        public bool Exist(string key, string category)
        {
            return _Exist(key, category);
        }


        private static (string Bucket, string Name) ParseKey(string key, string category)
        {
            if (!string.IsNullOrEmpty(category)) return (category, key);
            var reg = new Regex("(.*)(^[^_]*)_(.*)");
            var result = reg.Match(key).Groups;
            var bucket = result[2].Value;
            var name = result[3].Value;

            return (bucket, name);
        }

        private static (bool UseSSL, string Domain) ParseAddress(string address)
        {
            Regex reg = new Regex("(http|https)://(.*):(.[^/]+)");
            var result = reg.Match(address).Groups;
            var useSSL = result[1].Value == "https";
            var domain = result[2].Value;
            var port = result[3].Value;
            return (useSSL, string.Format("{0}:{1}", domain, port));
        }


        public Store(string address, string accessKey, string secretKey)
        {
            var (UseSSL, Domain) = ParseAddress(address);
            this._useSSL = UseSSL;
            this._domain = Domain;
            this._client = new MinioClient().WithEndpoint(Domain).WithCredentials(accessKey, secretKey);
            if (UseSSL) this._client.WithSSL();
        }
    }
}
