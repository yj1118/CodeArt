using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using Dapper;

namespace CodeArt.DomainDriven
{
    public interface IDataRepository
    {
        void Initialize();
    }

    public abstract class DataRepository : IDataRepository
    {
        protected DataRepository()
        {
        }

        protected DataConnection Connection
        {
            get
            {
                return DataContext.Current.Connection;
            }
        }

        public abstract void Initialize();
    }
}
