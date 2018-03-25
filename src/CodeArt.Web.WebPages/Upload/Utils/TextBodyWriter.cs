namespace CodeArt.Web.WebPages
{
    using System;
    using System.Text;

    internal class TextBodyWriter : BodyWriter
    {
        private StringBuilder _body = new StringBuilder();
        private Encoding _encoding;

        public TextBodyWriter(Encoding encoding)
        {
            this._encoding = encoding;
        }

        public override void Close()
        {
        }

        public override void Write(byte[] buffer, int index, int count)
        {
            this._body.Append(this._encoding.GetString(buffer, index, count));
            base._size += count;
        }

        public override string Content
        {
            get
            {
                return this._body.ToString();
            }
        }
    }
}

