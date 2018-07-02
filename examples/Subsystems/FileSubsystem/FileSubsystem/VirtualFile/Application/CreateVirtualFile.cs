using System;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace FileSubsystem
{
    public sealed class CreateVirtualFile : Command<VirtualFile>
    {
        private string _name;
        private string _storeKey;
        private long _size;
        private Guid _directoryId;
        private string _extension;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">文件名称 比如 test</param>
        /// <param name="extension">文件的扩展名 例如 txt</param>
        /// <param name="storeKey">文件的存储编号，通过该编号可以从文件存储系统中得到物理文件</param>
        /// <param name="size">文件的大小</param>
        /// <param name="diskId">文件所在的磁盘</param>
        /// <param name="directoryId">文件所在的目录</param>
        public CreateVirtualFile(string name, string extension, string storeKey, long size, Guid directoryId)
        {
            _name = name;
            _storeKey = storeKey;
            _size = size;
            _directoryId = directoryId;
            _extension = extension;
        }

        protected override VirtualFile ExecuteProcedure()
        {
            VirtualDirectory directory = GetDirectory();
            VirtualFile file = BuildVirtualFile();
            var disk = directory.Disk;
            disk.Create(directory, file);
            VirtualDiskCommon.Update(disk);
            return file;
        }

        private VirtualFile BuildVirtualFile()
        {
            VirtualFile file = new VirtualFile(Guid.NewGuid(),_size)
            {
                Name = _name,
                StoreKey = _storeKey,
                Extension = _extension
            };
            return file;
        }

        private VirtualDirectory GetDirectory()
        {
            var directory = VirtualDirectoryCommon.FindById(_directoryId, QueryLevel.Mirroring);
            if (directory.IsEmpty()) throw new BusinessException(string.Format(Strings.NotFoundVirtualDirectory, _directoryId));
            return directory;
        }
    }
}
