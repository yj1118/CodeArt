using System;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace FileSubsystem
{
    public sealed class UpdateVirtualDirectory : Command<VirtualDirectory>
    {
        private Guid _dirId;

        public string Name
        {
            get;
            set;
        }

        public UpdateVirtualDirectory(Guid dirId)
        {
            _dirId = dirId;
        }

        protected override VirtualDirectory ExecuteProcedure()
        {
            VirtualDirectory dir = LoadDirectory();
            if (this.Name != null) dir.Name = this.Name;
            var repository = Repository.Create<IVirtualDirectoryRepository>();
            repository.Update(dir);
            return dir;
        }

        private VirtualDirectory LoadDirectory()
        {
            var dir = VirtualDirectoryCommon.FindById(_dirId, QueryLevel.Single);
            if (dir.IsEmpty()) throw new BusinessException(string.Format(Strings.NotFoundVirtualDirectory, _dirId));
            return dir;
        }
    }
}
