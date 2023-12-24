namespace CodeArt.Web.WebPages
{
    using System;
    using System.IO;
    using System.Web;

    internal sealed class RequestStream : Stream
    {
        private long _position = 0L;
        private byte[] _preloadedEntityBody;
        private bool _reachedEndOfRequest = false;
        private HttpWorkerRequest _workerRequest;

        public RequestStream(HttpWorkerRequest workerRequest)
        {
            this._workerRequest = workerRequest;
            if (this._workerRequest.GetPreloadedEntityBody() != null)
            {
                this._preloadedEntityBody = new byte[this._workerRequest.GetPreloadedEntityBody().Length];
                Buffer.BlockCopy(this._workerRequest.GetPreloadedEntityBody(), 0, this._preloadedEntityBody, 0, this._preloadedEntityBody.Length);
            }
            else
            {
                this._preloadedEntityBody = new byte[0];
            }
        }

        private void AdjustCount(ref int count)
        {
            if ((this.Position + ((long) count)) > this.Length)
            {
                count = (int) (this.Length - this.Position);
                this._reachedEndOfRequest = true;
            }
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public static long GetRequestLength(HttpWorkerRequest workerRequest)
        {
            return long.Parse(workerRequest.GetKnownRequestHeader(11));
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!this._workerRequest.IsClientConnected())
            {
                return -1;
            }
            long position = this.Position;
            if (this.IsInPreloaded)
            {
                this.ReadPreloadedEntityBody(buffer, offset, count);
            }
            else
            {
                this.ReadEntityBody(buffer, offset, count);
            }
            return (int) (this.Position - position);
        }

        private void ReadEntityBody(byte[] buffer, int offset, int count)
        {
            this.AdjustCount(ref count);
            byte[] buffer2 = new byte[count];
            int num = this._workerRequest.ReadEntityBody(buffer2, count);
            while (num != count)
            {
                int num2 = this._workerRequest.ReadEntityBody(buffer2, num, count - num);
                if (num2 == 0)
                {
                    break;
                }
                num += num2;
            }
            Buffer.BlockCopy(buffer2, 0, buffer, offset, num);
            this._position += num;
        }

        private void ReadPreloadedEntityBody(byte[] buffer, int offset, int count)
        {
            int num = Math.Min(count, this.RemainingPreloadedBytes);
            Buffer.BlockCopy(this._preloadedEntityBody, (int) this.Position, buffer, offset, num);
            this._position += num;
            if (!this._workerRequest.IsEntireEntityBodyIsPreloaded())
            {
                int num2 = count - num;
                if (num2 > 0)
                {
                    this.ReadEntityBody(buffer, offset + num, num2);
                }
            }
            else if (this.Position >= this._preloadedEntityBody.Length)
            {
                this._reachedEndOfRequest = true;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        private bool IsInPreloaded
        {
            get
            {
                return (this.RemainingPreloadedBytes > 0);
            }
        }

        public override long Length
        {
            get
            {
                return GetRequestLength(this._workerRequest);
            }
        }

        public override long Position
        {
            get
            {
                return this._position;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public bool ReachedEndOfRequest
        {
            get
            {
                return this._reachedEndOfRequest;
            }
        }

        private int RemainingPreloadedBytes
        {
            get
            {
                return Math.Max(0, this._preloadedEntityBody.Length - ((int) this.Position));
            }
        }
    }
}

