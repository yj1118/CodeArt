namespace CodeArt.Web.WebPages
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using CodeArt.Web.WebPages;

    internal class RequestEntity
    {
        private string _clientFileName = null;
        private string _fileContentType = null;
        private string[] _headers;
        private string _inputFieldName = null;
        private bool _isRadUploadEntity = false;
        private StringBuilder _textContent = null;
        private UploadedFile _uploadedFile = null;
        private BodyWriter _writer;

        public RequestEntity(string rawHeaders, Encoding encoding)
        {
            this._headers = rawHeaders.Replace("\r\n", "|").Split(new char[] { '|' });
            Group group = Regex.Match(this.ContentDispositionHeaderValue, "\\s?filename=\"(?<filename>[^\"]*).*$", RegexOptions.IgnoreCase).Groups["filename"];
            this._clientFileName = (group == null) ? string.Empty : group.Value.Trim(new char[] { '"' });
            Group group2 = Regex.Match(this.ContentDispositionHeaderValue, @"\s?name=(?<name>[^;]*).*$", RegexOptions.IgnoreCase).Groups["name"];
            this._inputFieldName = (group2 == null) ? string.Empty : group2.Value.Trim(new char[] { '"' });

            if(_clientFileName == "blob" || string.IsNullOrEmpty(_clientFileName))
            {
                _clientFileName = _inputFieldName;
            }

            this._isRadUploadEntity = false;
            if (this.ClientFileName.Length > 0)
            {
                this._isRadUploadEntity = true;
            }
            this._fileContentType = this.IsRadUploadEntity ? this.ContentTypeHeaderValue.Trim() : string.Empty;
            this._writer = (this.IsRadUploadEntity && (this.FileContentType.Length > 0)) ? this.CreateFileBodyWriter() : this.CreateTextBodyWriter(encoding);
        }

        public void Close()
        {
            this._writer.Close();
            if (this._uploadedFile != null)
            {
                this._uploadedFile.SetContentLength(this.CurrentSize);
                var repositoryWriter = this._writer as RepositoryBodyWriter;
                if (repositoryWriter != null)
                {
                    this._uploadedFile.ServerKey = repositoryWriter.ServerKey;
                }
            }
        }

        private BodyWriter CreateFileBodyWriter()
        {
            var repository = UploadRepositoryFactory.Create();
            var writer = new RepositoryBodyWriter(repository,this.ClientFileName);
            this._uploadedFile = new RepositoryUploadFile(this.InputFieldName, this.ClientFileName, this.FileContentType, writer.TempKey, repository);
            return writer;
        }

        private BodyWriter CreateTextBodyWriter(Encoding encoding)
        {
            this._textContent = new StringBuilder();
            this._textContent.Append(this.GetBoundary());
            this._textContent.Append("\r\n");
            this._textContent.Append(this._headers[1]);
            this._textContent.Append("\r\n\r\n");
            return new TextBodyWriter(encoding);
        }

        private string GetBoundary()
        {
            return this._headers[0];
        }

        private string GetHeaderValue(string header)
        {
            int index = header.IndexOf(':');
            return ((index > -1) ? header.Substring(Math.Min(index + 1, header.Length)).Trim() : string.Empty);
        }

        public void Write(byte[] buffer, int index, int count)
        {
            this._writer.Write(buffer, index, count);
        }

        public string ClientFileName
        {
            get
            {
                return this._clientFileName;
            }
        }

        private string ContentDispositionHeaderValue
        {
            get
            {
                return this.GetHeaderValue((this._headers.Length > 1) ? this._headers[1] : string.Empty);
            }
        }

        private string ContentTypeHeaderValue
        {
            get
            {
                return this.GetHeaderValue((this._headers.Length > 2) ? this._headers[2] : string.Empty);
            }
        }

        public int CurrentSize
        {
            get
            {
                return this._writer.Size;
            }
        }

        public string FileContentType
        {
            get
            {
                return this._fileContentType;
            }
        }

        public string InputFieldName
        {
            get
            {
                return this._inputFieldName;
            }
        }

        public bool IsFileEntity
        {
            get
            {
                return (this.ClientFileName.Length > 0);
            }
        }

        public bool IsRadUploadEntity
        {
            get
            {
                return this._isRadUploadEntity;
            }
        }

        public string RadUploadClientFileName
        {
            get
            {
                return (this.IsRadUploadEntity ? this.ClientFileName : string.Empty);
            }
        }

        public string TextContent
        {
            get
            {
                if (this._writer is RepositoryBodyWriter)
                {
                    return string.Empty;
                }
                this._textContent.Append(this._writer.Content);
                this._textContent.Append("\r\n");
                return this._textContent.ToString();
            }
        }

        public UploadedFile UploadedFile
        {
            get
            {
                return this._uploadedFile;
            }
        }
    }
}

