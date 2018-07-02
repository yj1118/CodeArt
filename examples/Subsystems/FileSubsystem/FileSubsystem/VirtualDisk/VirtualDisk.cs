using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace FileSubsystem
{
    [ObjectRepository(typeof(IVirtualDiskRepository))]
    [ObjectValidator(typeof(VirtualDiskValidator))]
    public class VirtualDisk : AggregateRoot<VirtualDisk, Guid>
    {
        [PropertyRepository()]
        [StringLength(0, 25)]
        [StringFormat(StringFormat.Letter | StringFormat.Number)]
        public static readonly DomainProperty MarkedCodeProperty = DomainProperty.Register<string, VirtualDisk>("MarkedCode");

        /// <summary>
        /// 虚拟磁盘的唯一标识符,可以为空
        /// </summary> 
        public string MarkedCode
        {
            get
            {
                return GetValue<string>(MarkedCodeProperty);
            }
            set
            {
                SetValue(MarkedCodeProperty, value);
            }
        }

        [PropertyRepository()]
        [LongRange(0, long.MaxValue)]
        public static readonly DomainProperty SizeProperty = DomainProperty.Register<long, VirtualDisk>("Size", 0);

        /// <summary>
        /// 分配的虚拟磁盘大小，单位字节
        /// </summary>
        public long Size
        {
            get
            {
                return GetValue<long>(SizeProperty);
            }
            private set
            {
                SetValue(SizeProperty, value);
            }
        }


        [PropertyRepository()]
        public static readonly DomainProperty CreateTimeProperty = DomainProperty.Register<DateTime, VirtualDisk>("CreateTime", (owner) => { return DateTime.Now; });

        /// <summary>
        /// 虚拟磁盘的创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get
            {
                return GetValue<DateTime>(CreateTimeProperty);
            }
            set
            {
                SetValue(CreateTimeProperty,value);
            }
        }

        [PropertyRepository()]
        [StringLength(0, 50)]
        public static readonly DomainProperty DescriptionProperty = DomainProperty.Register<string, VirtualDisk>("Description", string.Empty);

        /// <summary>
        /// 磁盘描述
        /// </summary>
        public string Description
        {
            get
            {
                return GetValue<string>(DescriptionProperty);
            }
            set
            {
                SetValue(DescriptionProperty, value);
            }
        }


        [PropertyRepository()]
        public static readonly DomainProperty RootProperty = DomainProperty.Register<VirtualDirectory, VirtualDisk>("Root",(disk)=> { return VirtualDirectory.Empty; });

        /// <summary>
        /// 虚拟磁盘的根目录
        /// </summary>
        public VirtualDirectory Root
        {
            get
            {
                return GetValue<VirtualDirectory>(RootProperty);
            }
            private set
            {
                SetValue(RootProperty, value);
            }
        }


        #region 虚拟磁盘的实际使用空间大小

        [PropertyRepository()]
        public static readonly DomainProperty UsedSizeProperty = DomainProperty.Register<long, VirtualDisk>("UsedSize", (long)0);

        /// <summary>
        /// 已使用的大小
        /// </summary>
        public long UsedSize
        {
            get
            {
                return GetValue<long>(UsedSizeProperty);
            }
            private set
            {
                SetValue(UsedSizeProperty, value);
            }
        }

        /// <summary>
        /// 增加虚拟磁盘的已使用的大小
        /// </summary>
        /// <param name="value"></param>
        private void IncreaseUsedSize(long value)
        {
            if (AddedIsFull(value))//超出总大小
                throw new BusinessException(Strings.DiskSpaceNotEnough);

            this.UsedSize += value;
        }

        private bool AddedIsFull(long value)
        {
            return (this.UsedSize + value) > this.Size;
        }

        /// <summary>
        /// 磁盘空间已满
        /// </summary>
        public bool IsFull
        {
            get
            {
                return this.UsedSize >= this.Size;
            }
        }

        /// <summary>
        /// 减少虚拟磁盘的实际大小
        /// </summary>
        /// <param name="value"></param>
        private void ReduceUsedSize(long value)
        {
            if (IsLessThanZero(value))
                throw new BusinessException(Strings.DiskSpaceCapacityLessZero);
            this.UsedSize -= value;
        }

        private bool IsLessThanZero(long value)
        {
            return this.UsedSize - value < 0;
        }

        #endregion


        /// <summary>
        /// 初始化磁盘信息
        /// </summary>
        /// <returns></returns>
        private void Init(long size)
        {
            if (this.IsEmpty()) return;
            this.UsedSize = 0;
            Allocate(size);
            InitRoot();
        }

        private void InitRoot()
        {
            this.Root = new VirtualDirectory(Guid.NewGuid(), this)
            {
                Name = Strings.DiskRootDirectory,
                CreateTime = DateTime.Now
            };
            var repository = Repository.Create<IVirtualDirectoryRepository>();
            repository.Add(this.Root);
        }

        /// <summary>
        /// 分配大小
        /// </summary>
        /// <param name="newSize"></param>
        internal void Allocate(long newSize)
        {
            if (newSize < this.UsedSize)
                throw new BusinessException(string.Format(Strings.AllocateError, newSize, this.UsedSize));
            this.Size = newSize;
        }

        /// <summary>
        /// 向磁盘中添加文件
        /// </summary>
        /// <param name="directory">文件需要被添加到的目录</param>
        /// <param name="file"></param>
        internal void Create(VirtualDirectory directory, VirtualFile file)
        {
            if (directory.IsEmpty() || file.IsEmpty()) return;
            if (file.IsExists) throw new DomainDrivenException(Strings.FileExists);
            if (!directory.Belong(this)) throw new BusinessException(string.Format(Strings.DirectoryNotBelongDisk, directory.Id, this.Id));
            this.IncreaseUsedSize(file.Size);

            //出于性能考虑，使用仓储接口创建文件
            file.Directory = directory;
            var repository = Repository.Create<IVirtualFileRepository>();
            repository.Add(file);
        }

        /// <summary>
        /// 从磁盘中删除文件
        /// </summary>
        /// <param name="file"></param>
        internal void Delete(VirtualFile file)
        {
            if (file.IsEmpty()) return;
            if (!file.Belong(this)) throw new BusinessException(string.Format(Strings.FileNotBelongDisk, file.Id, this.Id));
            this.ReduceUsedSize(file.Size);

            //出于性能考虑，使用仓储接口删除文件
            var repository = Repository.Create<IVirtualFileRepository>();
            repository.Delete(file);
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="directory"></param>
        internal void Create(VirtualDirectory directory)
        {
            if (directory.IsEmpty()) return;
            if (!directory.Belong(this)) throw new BusinessException(string.Format(Strings.DirectoryNotBelongDisk, directory.Id, this.Id));
            if (directory.Parent.IsEmpty()) directory.Parent = this.Root;
            var repository = Repository.Create<IVirtualDirectoryRepository>();
            repository.Add(directory);
            repository.Update(directory.Parent); //由于涉及到父元素的改动，所以也要进行修改
        }


        internal void Delete(VirtualDirectory directory)
        {
            if (directory.IsEmpty()) return;
            if (!directory.Belong(this)) throw new BusinessException(string.Format(Strings.DirectoryNotBelongDisk, directory.Id, this.Id));

            //出于性能考虑，使用仓储接口删除
            var repository = Repository.Create<IVirtualDirectoryRepository>();
            repository.Delete(directory);
        }

        public static VirtualDisk NewDisk(Guid diskId, string markedCode, long size, string description)
        {
            VirtualDisk disk = new VirtualDisk(diskId)
            {
                MarkedCode = markedCode,
                Description = description
            };
            disk.Init(size);
            return disk;
        }

        public override void OnPreDelete()
        {
            this.Delete(this.Root);//删除磁盘之前，删除根目录
            base.OnPreDelete();
        }

        public VirtualDisk(Guid id, long size)
            : base(id)
        {
            this.Init(size);
            this.OnConstructed();
        }

        [ConstructorRepository]
        public VirtualDisk(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }

        private class VirtualDiskEmpty : VirtualDisk
        {
            public VirtualDiskEmpty()
                : base(Guid.Empty,0)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly VirtualDisk Empty = new VirtualDiskEmpty();

        public const long Size1M = 1024 * 1024;

        /// <summary>
        /// 代表1G的大小
        /// </summary>
        public const long Size1G = 1024 * Size1M;

    }
}
