using System;
using System.Collections.Generic;
using CodeArt.DomainDriven;

using CodeArt.DomainDriven.DataAccess;
using CodeArt.Concurrent;

namespace AccountSubsystem
{
    [SafeAccess]
    public class SqlOrganizationRepository : SqlRepository<Organization>, IOrganizationRepository
    {
        public static readonly SqlOrganizationRepository Instance = new SqlOrganizationRepository();
    }
}
