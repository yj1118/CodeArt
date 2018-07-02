using System;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace FileSubsystem
{
    public sealed class UpdateVirtualDisk : Command<VirtualDisk>
    {
        private Guid _diskId;
       
        public long? Size
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string MarkedCode
        {
            get;
            set;
        }

        public UpdateVirtualDisk(Guid diskId)
        {
            _diskId = diskId;
        }

        protected override VirtualDisk ExecuteProcedure()
        {
            VirtualDisk disk = LoadDisk();
            if (this.Size != null) disk.Allocate(this.Size.Value);
            if (this.Description != null) disk.Description = this.Description;
            if (this.MarkedCode != null) disk.MarkedCode = this.MarkedCode;

            var repository = Repository.Create<IVirtualDiskRepository>();
            repository.Update(disk);

            return disk;
        }

        private VirtualDisk LoadDisk()
        {
            VirtualDisk disk = VirtualDiskCommon.FindById(_diskId, QueryLevel.Single);
            if (disk.IsEmpty()) throw new BusinessException(string.Format(Strings.NotFoundVirtualDisk, _diskId));
            return disk;
        }
    }
}
