namespace CodeArt.Web.WebPages
{
    using System;

    public class ValidateFileEventArgs : UploadedFileEventArgs
    {
        private bool _isValid;
        private bool _skipInternalValidation;

        internal ValidateFileEventArgs(UploadedFile uploadedFile) : base(uploadedFile)
        {
            this._isValid = true;
            this._skipInternalValidation = false;
        }

        public bool IsValid
        {
            get
            {
                return this._isValid;
            }
            set
            {
                this._isValid = value;
            }
        }

        public bool SkipInternalValidation
        {
            get
            {
                return this._skipInternalValidation;
            }
            set
            {
                this._skipInternalValidation = value;
            }
        }
    }
}

