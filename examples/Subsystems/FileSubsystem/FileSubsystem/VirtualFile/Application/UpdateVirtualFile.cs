using System;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace FileSubsystem
{
    public sealed class UpdateVirtualFile : Command<VirtualFile>
    {
        private Guid _fileId;

        public string Name
        {
            get;
            set;
        }

        public Guid? DirectoryId
        {
            get;
            set;
        }

        public long? Size
        {
            get;
            set;
        }


        public UpdateVirtualFile(Guid fileId)
        {
            _fileId = fileId;
        }

        protected override VirtualFile ExecuteProcedure()
        {
            VirtualFile file = LoadFile();
            if (this.Name != null) file.Name = this.Name;
            if (DirectoryIsUpdated(file)) file.Directory = GetDirectory();

            Save(file);
            return file;
        }

        private bool DirectoryIsUpdated(VirtualFile file)
        {
            return this.DirectoryId != null && this.DirectoryId.Value != file.Directory.Id;
        }

        private VirtualFile LoadFile()
        {
            VirtualFile file = VirtualFileCommon.FindById(_fileId, QueryLevel.Mirroring);
            if (file.IsEmpty()) throw new BusinessException(string.Format(Strings.NotFoundVirtualFile, _fileId));
            return file;
        }

        private VirtualDirectory GetDirectory()
        {
            var directory = VirtualDirectoryCommon.FindById(this.DirectoryId.Value, QueryLevel.Mirroring);
            if (directory.IsEmpty()) throw new BusinessException(string.Format(Strings.NotFoundVirtualDirectory, this.DirectoryId.Value));

            return directory;
        }

        private void Save(VirtualFile file)
        {
            var repository = Repository.Create<IVirtualFileRepository>();
            repository.Update(file);
        }

    }
}
