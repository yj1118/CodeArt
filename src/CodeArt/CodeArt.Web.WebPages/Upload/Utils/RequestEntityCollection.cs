namespace CodeArt.Web.WebPages
{
    using System;
    using System.Collections;
    using System.Reflection;

    internal class RequestEntityCollection : CollectionBase
    {
        public RequestEntity Add(RequestEntity obj)
        {
            base.InnerList.Add(obj);
            return obj;
        }

        public void Insert(int index, RequestEntity obj)
        {
            base.InnerList.Insert(index, obj);
        }

        public void Remove(RequestEntity obj)
        {
            base.InnerList.Remove(obj);
        }

        public RequestEntity this[int index]
        {
            get
            {
                return (RequestEntity) base.InnerList[index];
            }
            set
            {
                base.InnerList[index] = value;
            }
        }
    }
}

