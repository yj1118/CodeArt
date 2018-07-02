namespace CodeArt.Web.WebPages
{
    using System;
    using System.Collections;
    using System.Reflection;
    using CodeArt.Web.WebPages;

    public sealed class UploadedFileCollection : CollectionBase
    {
        internal UploadedFileCollection()
        {
        }

        internal UploadedFile Add(UploadedFile obj)
        {
            base.InnerList.Add(obj);
            return obj;
        }

        internal UploadedFileCollection GetForRadUpload(string radUploadClientID)
        {
            UploadedFileCollection files = new UploadedFileCollection();
            foreach (UploadedFile file in base.InnerList)
            {
                if (file.InputFieldName.StartsWith(radUploadClientID))
                {
                    files.Add(file);
                }
            }
            return files;
        }

        internal UploadedFile Remove(UploadedFile obj)
        {
            foreach (UploadedFile file in base.InnerList)
            {
                if (file == obj)
                {
                    base.InnerList.Remove(obj);
                    return obj;
                }
            }
            return obj;
        }

        public UploadedFile this[int index]
        {
            get
            {
                return (UploadedFile) base.InnerList[index];
            }
        }

        public UploadedFile this[string id]
        {
            get
            {
                foreach (UploadedFile file in base.InnerList)
                {
                    if (file.InputFieldName == id)
                    {
                        return file;
                    }
                }
                return null;
            }
        }
    }
}

