namespace CodeArt.Web.WebPages
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Web;

    public abstract class UploadedFile
    {
        protected UploadedFile()
        {
        }

        internal abstract void Delete();
        public virtual string GetExtension()
        {
            return Path.GetExtension(this.FileName);
        }

        public string GetFieldValue(string fieldName)
        {
            string str = new Regex(@"^([\w\d]+)file(\d+)$").Replace(this.InputFieldName, string.Format("$1{0}$2", fieldName));
            return HttpContext.Current.Request.Form[str];
        }

        public bool GetIsFieldChecked(string fieldName)
        {
            string fieldValue = this.GetFieldValue(fieldName);
            return ((fieldValue != null) && (fieldValue.Length > 0));
        }

        public string GetName()
        {
            return Path.GetFileName(this.FileName);
        }

        public string GetNameWithoutExtension()
        {
            return Path.GetFileNameWithoutExtension(this.FileName);
        }

        internal abstract void SetContentLength(int length);

        public abstract long ContentLength { get; }

        public abstract string ContentType { get; }

        public abstract string FileName { get; }

        public abstract string ServerKey { get; internal set; }

        internal abstract string InputFieldName { get; }
    }
}

