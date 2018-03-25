using System;

namespace CodeArt.Web.WebPages
{
    internal abstract class BodyWriter
    {
        protected int _size = 0;
        protected UploadedFile _uploadedFile = null;

        public abstract void Close();
        public abstract void Write(byte[] buffer, int index, int count);

        public abstract string Content { get; }

        public int Size
        {
            get
            {
                return this._size;
            }
        }
    }
}

