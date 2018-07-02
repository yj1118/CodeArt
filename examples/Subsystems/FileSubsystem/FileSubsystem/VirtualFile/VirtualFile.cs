using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace FileSubsystem
{
    [Remotable()]
    [ObjectRepository(typeof(IVirtualFileRepository))]
    [ObjectValidator(typeof(VirtualFileValidator))]
    public class VirtualFile : AggregateRoot<VirtualFile, Guid>
    {
        [PropertyRepository()]
        [NotEmpty()]
        [StringLength(1, 100)]
        public static readonly DomainProperty NameProperty = DomainProperty.Register<string, VirtualFile>("Name");

        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name
        {
            get
            {
                return GetValue<string>(NameProperty);
            }
            set
            {
                SetValue(NameProperty, value);
            }
        }


        [PropertyRepository()]
        public static readonly DomainProperty CreateTimeProperty = DomainProperty.Register<DateTime, VirtualFile>("CreateTime", (owner) => { return DateTime.Now; });

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
                SetValue(CreateTimeProperty, value);
            }
        }

        [PropertyRepository()]
        [LongRange(0, long.MaxValue)]
        public static readonly DomainProperty SizeProperty = DomainProperty.Register<long, VirtualFile>("Size", 0);

        /// <summary>
        /// 文件的大小（Byte数）,大小不能改变，如果要改变，请删除文件再保存新的文件
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
        [NotEmpty]
        [StringLength(1, 100)]
        public static readonly DomainProperty StoreKeyProperty = DomainProperty.Register<string, VirtualFile>("StoreKey", string.Empty);

        /// <summary>
        /// 文件对应于存储系统的Key
        /// </summary>
        public string StoreKey
        {
            get
            {
                return GetValue<string>(StoreKeyProperty);
            }
            set
            {
                SetValue(StoreKeyProperty, value);
            }
        }

        [PropertyRepository()]
        [StringLength(0, 20)]
        [ASCIIString]
        [PropertySet("SetExtension")]
        public static readonly DomainProperty ExtensionProperty = DomainProperty.Register<string, VirtualFile>("Extension", string.Empty);

        /// <summary>
        /// 文件的扩展名
        /// </summary>
        public string Extension
        {
            get
            {
                return GetValue<string>(ExtensionProperty);
            }
            set
            {
                SetValue(ExtensionProperty, value);
            }
        }


        private void SetExtension(string extension)
        {
            if (!string.IsNullOrEmpty(extension))
                extension = extension.Trim('.');
            this.Extension = extension;
        }



        [PropertyRepository()]
        [NotEmpty]
        public static readonly DomainProperty DirectoryProperty = DomainProperty.Register<VirtualDirectory, VirtualFile>("Directory", (obj) => { return VirtualDirectory.Empty; });

        /// <summary>
        /// 文件所属的目录
        /// </summary>
        public VirtualDirectory Directory
        {
            get
            {
                return GetValue<VirtualDirectory>(DirectoryProperty);
            }
            set
            {
                SetValue(DirectoryProperty, value);
            }
        }

        /// <summary>
        /// 文件所属的虚拟磁盘
        /// </summary>
        public VirtualDisk Disk
        {
            get
            {
                return this.Directory.Disk;
            }
        }

        public bool Belong(VirtualDisk disk)
        {
            return this.Disk.Equals(disk);
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        public bool IsExists
        {
            get
            {
                return !this.Disk.IsEmpty();
            }
        }

        [ConstructorRepository]
        public VirtualFile(Guid id,long size)
            : base(id)
        {
            this.Size = size;
            this.OnConstructed();
        }

        [ConstructorRepository]
        public VirtualFile(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }

        private class VirtualFileEmpty : VirtualFile
        {
            public VirtualFileEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly VirtualFile Empty = new VirtualFileEmpty();
    }
}
