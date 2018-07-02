namespace CodeArt.Web.WebPages
{
    using System;
    using System.IO;
    using CodeArt.IO;
    using CodeArt.Util;
    using CodeArt.Web.WebPages;

    internal sealed class RepositoryUploadFile : UploadedFile
    {
        private long _contentLength;
        private string _contentType;
        private string _fileName;
        private string _inputFieldName;
        private IUploadRepository _repository;

        public string TempKey
        {
            get;
            private set;
        }

        public override string ServerKey
        {
            get;
            internal set;
        }

        internal RepositoryUploadFile(string inputFieldName, string fileName, string contentType, string tempKey,IUploadRepository repository)
        {
            this._contentType = contentType;
            this._fileName = fileName.EqualsIgnoreCase("blob") ? inputFieldName : fileName;
            this._inputFieldName = inputFieldName;
            this.TempKey = tempKey;
            _repository = repository;
        }

        /// <summary>
        /// 删除临时文件
        /// </summary>
        private void Close()
        {
            _repository.Delete(this.TempKey);
        }

        internal override void Delete()
        {
            this.Close();
        }

        //~RepositoryUploadFile()
        //{
        //    this.Close();
        //}

        public override string GetExtension()
        {
            if (!string.IsNullOrEmpty(this.ServerKey)) return GetExtension(this.ServerKey);
            if (!string.IsNullOrEmpty(this.TempKey)) return GetExtension(this.TempKey);
            return base.GetExtension();
        }

        private string GetExtension(string key)
        {
            var pos = key.LastIndexOf('.');
            if (pos > -1) return key.Substring(pos);
            throw new WebException(string.Format(Strings.FileKeyErrorCanNotGetExtension, key));
        }


        internal override void SetContentLength(int length)
        {
            this._contentLength = length;
        }

        public override long ContentLength
        {
            get
            {
                return this._contentLength;
            }
        }

        public override string ContentType
        {
            get
            {
                return this._contentType;
            }
        }

        public override string FileName
        {
            get
            {
                return this._fileName;
            }
        }

        internal override string InputFieldName
        {
            get
            {
                return this._inputFieldName;
            }
        }
    }
}

