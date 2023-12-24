namespace CodeArt.Web.WebPages
{
    using System;
    using System.Text;
    using CodeArt.Web.WebPages;

    internal class RequestParser
    {
        private byte[] _boundary;
        private byte[] _buffer;
        private Encoding _encoding;
        private RequestEntityCollection _entities = new RequestEntityCollection();
        private StringBuilder _headerBuilder = new StringBuilder();
        private byte[] _headerEndDelimiter;
        private bool _processingHeader = true;
        private RequestStream _request;
        private byte[] _textContent = null;
        private UploadContext _uploadContext;
        private UploadedFileCollection _uploadedFiles;
        private int CHUNK_SIZE = 0;

        public RequestParser(byte[] boundary, Encoding encoding, UploadContext uploadContext)
        {
            this._boundary = boundary;
            this._encoding = encoding;
            this._headerEndDelimiter = encoding.GetBytes("\r\n\r\n");
            this._uploadContext = uploadContext;
            if (Configuration.IsDefaultChunkSize)
            {
                this.CHUNK_SIZE = Math.Max(Configuration.ChunkSize, Math.Min(Configuration.MAX_CHUNK_SIZE * 100, uploadContext.RequestLength) / 100);
            }
            else
            {
                this.CHUNK_SIZE = Configuration.ChunkSize;
            }
            this._buffer = new byte[this.CHUNK_SIZE + this.HelperBlockSize];
        }

        private void ClearHeaderBuilder()
        {
            this._headerBuilder.Remove(0, this._headerBuilder.Length);
        }

        private void FillBuffer()
        {
            this.CHUNK_SIZE = this._request.Read(this._buffer, this.HelperBlockSize, this.CHUNK_SIZE);
        }

        private int GetBodyPosition(int headerPosition)
        {
            return this.IndexOf(this._headerEndDelimiter, this._buffer, headerPosition);
        }

        private int GetBoundaryPosition(int bodyPosition)
        {
            return this.IndexOf(this._boundary, this._buffer, bodyPosition);
        }

        private int GetChunkRelativeEnd(int end)
        {
            return (this.IsInChunk(end) ? end : this.CHUNK_SIZE);
        }

        private int GetChunkRelativeStart(int start)
        {
            return (this.IsInChunk(start) ? start : 0);
        }

        private string GetHeaderPart(int headerPosition, int bodyPosition)
        {
            return this._encoding.GetString(this._buffer, headerPosition, bodyPosition - headerPosition);
        }

        private int GetProcessedPartEnd(int index)
        {
            return (this.ProcessingHeader ? this.GetBodyPosition(index) : this.GetBoundaryPosition(index));
        }

        private int GetProcessedPartStart(int index)
        {
            return (this.ProcessingHeader ? this.GetBoundaryPosition(index) : ((index == this.CHUNK_SIZE) ? -1 : this.GetBodyPosition(index)));
        }

        private int IndexOf(byte[] pattern, byte[] buffer, int start)
        {
            int index = 0;
            int num2 = Array.IndexOf<byte>(buffer, pattern[0], start);
            if (num2 != -1)
            {
                while ((num2 + index) < buffer.Length)
                {
                    if (buffer[num2 + index] == pattern[index])
                    {
                        index++;
                        if (index == pattern.Length)
                        {
                            return num2;
                        }
                    }
                    else
                    {
                        num2 = Array.IndexOf<byte>(buffer, pattern[0], num2 + index);
                        if (num2 == -1)
                        {
                            return num2;
                        }
                        index = 0;
                    }
                }
            }
            return num2;
        }

        private void InitBuffer()
        {
            this._request.Read(this._buffer, 0, this.CHUNK_SIZE + this.HelperBlockSize);
        }

        private bool IsInChunk(int position)
        {
            return ((position > -1) && (position <= this.CHUNK_SIZE));
        }

        private void MoveHelperBlock()
        {
            Buffer.BlockCopy(this._buffer, this.CHUNK_SIZE, this._buffer, 0, this.HelperBlockSize);
        }

        public void ProcessRequest(RequestStream request)
        {
            bool flag;
            this._request = request;
            this.InitBuffer();
            this.ProcessingHeader = true;
            int start = 0;
            int position = 0;
        Label_00A6:
            flag = true;
            while (this.IsInChunk(position))
            {
                position = this.GetProcessedPartEnd(this.GetChunkRelativeStart(start));
                this.WriteProcessedPart(start, position);
                start = this.GetProcessedPartStart(this.GetChunkRelativeEnd(position));
            }
            this.MoveHelperBlock();
            if (!this._request.ReachedEndOfRequest)
            {
                this.FillBuffer();
                if (this.CHUNK_SIZE < 0)
                {
                    if (this.CurrentEntity != null)
                    {
                        this.CurrentEntity.Close();
                    }
                }
                else
                {
                    position = 0;
                    goto Label_00A6;
                }
            }
        }

        private void UpdateUploadProgress()
        {
            this._uploadContext.CurrentEntity = this.CurrentEntity;
        }

        private void WriteProcessedPart(int start, int end)
        {
            int chunkRelativeStart = this.GetChunkRelativeStart(start);
            int chunkRelativeEnd = this.GetChunkRelativeEnd(end);
            bool flag = chunkRelativeEnd == end;
            if (this.ProcessingHeader)
            {
                this._headerBuilder.Append(this.GetHeaderPart(chunkRelativeStart, chunkRelativeEnd));
                if (flag)
                {
                    this.ProcessingHeader = false;
                }
            }
            else
            {
                if (start == chunkRelativeStart)
                {
                    this._entities.Add(new RequestEntity(this._headerBuilder.ToString(), this._encoding));
                    this.UpdateUploadProgress();
                    this.ClearHeaderBuilder();
                    chunkRelativeStart += this._headerEndDelimiter.Length;
                }
                int num3 = chunkRelativeEnd - chunkRelativeStart;
                int count = num3 - Math.Min(num3, flag ? 2 : 0);
                this.CurrentEntity.Write(this._buffer, chunkRelativeStart, count);
                if (flag)
                {
                    this.CurrentEntity.Close();
                    this.ProcessingHeader = true;
                }
            }
        }

        private int BoundarySize
        {
            get
            {
                return this._boundary.Length;
            }
        }

        private RequestEntity CurrentEntity
        {
            get
            {
                return ((this._entities.Count == 0) ? null : this._entities[this._entities.Count - 1]);
            }
        }

        private int HelperBlockSize
        {
            get
            {
                return (this.BoundarySize - 1);
            }
        }

        private bool ProcessingHeader
        {
            get
            {
                return this._processingHeader;
            }
            set
            {
                this._processingHeader = value;
            }
        }

        public byte[] TextContent
        {
            get
            {
                if (this._textContent == null)
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (RequestEntity entity in this._entities)
                    {
                        builder.Append(entity.TextContent);
                    }
                    builder.Append(this._encoding.GetString(this._boundary));
                    builder.Append("--\r\n");
                    this._textContent = this._encoding.GetBytes(builder.ToString());
                }
                return this._textContent;
            }
        }

        public UploadedFileCollection UploadedFiles
        {
            get
            {
                if (this._uploadedFiles == null)
                {
                    this._uploadedFiles = new UploadedFileCollection();
                    foreach (RequestEntity entity in this._entities)
                    {
                        if (entity.UploadedFile != null)
                        {
                            this._uploadedFiles.Add(entity.UploadedFile);
                        }
                    }
                }
                return this._uploadedFiles;
            }
        }
    }
}

