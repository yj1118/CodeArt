namespace CodeArt.Web.WebPages
{
    using System;

    public class UploadedFileEventArgs : EventArgs
    {
        private UploadedFile _uploadedFile;

        internal UploadedFileEventArgs(UploadedFile uploadedFile)
        {
            this._uploadedFile = uploadedFile;
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

