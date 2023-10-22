using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using Dapper;

namespace CodeArt.DomainDriven
{
    public abstract class DataObject<TKey> : IDataObject
    {
        public TKey Id
        {
            get;
            protected set;
        }

        public DataObject()
        {
        }

        public DataObject(TKey id)
        {
            this.Id = id;
        }

        public virtual void Load(IDataReader reader)
        {
            if(reader.Read())
            {
                LoadImpl(reader);
                _isEmpty = false;
                this.ClearDirty();
            }
            else
            {
                _isEmpty = true;
            }
        }

        protected abstract void LoadImpl(IDataReader reader);

        private bool _isEmpty;

        public virtual bool IsEmpty()
        {
            return _isEmpty;
        }


        public bool IsDirty
        {
            get;
            private set;
        }

        protected void MarkDirty()
        {
            this.IsDirty = true;
        }

        public void ClearDirty()
        {
            this.IsDirty = false;
        }

    }
}
