namespace CodeArt.Web.WebPages
{
    using System;
    using System.Web;
    using CodeArt.Web.WebPages;

    public sealed class UploadContext
    {
        private RequestEntity _currentEntity = null;
        private int _currentEntityIndex = -1;
        private int _requestLength = 0;
        private DateTime _startTime = DateTime.Now;
        private int _uploadedBytes = 0;
        private UploadedFileCollection _uploadedFiles = new UploadedFileCollection();
        private int _uploadedFilesCount = -1;

        internal UploadContext(int requestLength)
        {
            this._requestLength = requestLength;
        }

        private const string _contextKey = "__uploadContext";

        internal static UploadContext GetUploadContext(HttpContext context)
        {
            return context.Items[_contextKey] as UploadContext;
        }

        internal static void RemoveUploadContext(HttpContext context)
        {
            context.Items.Remove(_contextKey);
        }

        internal static void SetUploadContext(HttpContext context, UploadContext uploadContext)
        {
            context.Items[_contextKey] = uploadContext;
        }

        public static UploadContext Current
        {
            get
            {
                return GetUploadContext(HttpContext.Current);
            }
        }

        internal RequestEntity CurrentEntity
        {
            get
            {
                return this._currentEntity;
            }
            set
            {
                if (this._currentEntity != null)
                {
                    this._uploadedBytes += this._currentEntity.CurrentSize;
                }
                this._currentEntity = value;
                this._currentEntityIndex++;
                if (this._currentEntity.IsFileEntity)
                {
                    this._uploadedFilesCount++;
                }
            }
        }

        internal int ElapsedMilliseconds
        {
            get
            {
                return DateTime.Now.Subtract(this._startTime).Seconds;
            }
        }

        internal bool IsUploadInProgress
        {
            get
            {
                return (this.CurrentEntity != null);
            }
        }

        internal int RequestLength
        {
            get
            {
                return this._requestLength;
            }
        }

        internal int UploadedBytes
        {
            get
            {
                return (this._uploadedBytes + ((this.CurrentEntity == null) ? 0 : this.CurrentEntity.CurrentSize));
            }
        }

        public UploadedFileCollection UploadedFiles
        {
            get
            {
                return this._uploadedFiles;
            }
            set
            {
                this._uploadedFiles = value;
            }
        }

        internal int UploadedFilesCount
        {
            get
            {
                return Math.Max(0, this._uploadedFilesCount);
            }
        }
    }
}

