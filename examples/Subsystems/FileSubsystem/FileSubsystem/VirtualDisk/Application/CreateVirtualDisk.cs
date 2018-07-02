using System;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace FileSubsystem
{
    public sealed class CreateVirtualDisk : Command<VirtualDisk>
    {
        private Guid _diskId;
        private long _size;
        public string MarkedCode
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public CreateVirtualDisk(Guid diskId, long size)
        {
            _diskId = diskId;
            _size = size;
        }

        protected override VirtualDisk ExecuteProcedure()
        {
            var disk = new VirtualDisk(_diskId, _size)
            {
                MarkedCode = this.MarkedCode ?? string.Empty,
                Description = this.Description ?? string.Empty
            };

            var repository = Repository.Create<IVirtualDiskRepository>();
            repository.Add(disk);

            return disk;
        }
    }
}
