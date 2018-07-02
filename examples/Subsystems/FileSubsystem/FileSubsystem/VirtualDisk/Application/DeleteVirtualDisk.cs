using System;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace FileSubsystem
{
    public sealed class DeleteVirtualDisk : Command
    {
        private Guid _diskId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diskId"></param>
        public DeleteVirtualDisk(Guid diskId)
        {
            _diskId = diskId;
        }

        protected override void ExecuteProcedure()
        {
            var repository = Repository.Create<IVirtualDiskRepository>();
            var disk = repository.Find(_diskId, QueryLevel.Mirroring);
            repository.Delete(disk);
        }
    }
}
