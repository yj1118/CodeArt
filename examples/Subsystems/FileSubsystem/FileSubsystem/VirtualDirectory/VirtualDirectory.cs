using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace FileSubsystem
{
    [ObjectRepository(typeof(IVirtualDirectoryRepository))]
    [ObjectValidator(typeof(VirtualDirectoryValidator))]
    public class VirtualDirectory : AggregateRoot<VirtualDirectory, Guid>
    {
        [PropertyRepository(Lazy = true)]
        [NotEmpty]
        public static readonly DomainProperty DiskProperty = DomainProperty.Register<VirtualDisk, VirtualDirectory>("Disk", (directory) => { return VirtualDisk.Empty; });


        /// <summary>
        /// 目录所属的磁盘
        /// </summary>
        public VirtualDisk Disk
        {
            get
            {
                return GetValue<VirtualDisk>(DiskProperty);
            }
            private set
            {
                SetValue(DiskProperty, value);
            }
        }

        [PropertyRepository()]
        [NotEmpty]
        [StringLength(1, 256)]
        public static readonly DomainProperty NameProperty = DomainProperty.Register<string, VirtualDirectory>("Name", string.Empty);


        /// <summary>
        /// 名称
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
        public static readonly DomainProperty CreateTimeProperty = DomainProperty.Register<DateTime, VirtualDirectory>("CreateTime", DateTime.Now);

        /// <summary>
        /// 目录的创建时间
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
        public static readonly DomainProperty IsSystemProperty = DomainProperty.Register<bool, VirtualDirectory>("IsSystem", false);

        /// <summary>
        /// 是否为系统目录
        /// </summary>
        public bool IsSystem
        {
            get
            {
                return GetValue<bool>(IsSystemProperty);
            }
            set
            {
                SetValue(IsSystemProperty, value);
            }
        }

        #region 子目录

        /// <summary>
        /// 子目录
        /// </summary>
        [PropertyRepository(Lazy = true)]
        [List(Max = 50)]
        public static readonly DomainProperty ChildsProperty = DomainProperty.RegisterCollection<VirtualDirectory, VirtualDirectory>("Childs");

        private DomainCollection<VirtualDirectory> _Childs
        {
            get
            {
                return GetValue<DomainCollection<VirtualDirectory>>(ChildsProperty);
            }
            set
            {
                SetValue(ChildsProperty, value);
            }
        }

        /// <summary>
        /// 子目录
        /// </summary>
        public IEnumerable<VirtualDirectory> Childs
        {
            get
            {
                return _Childs;
            }
        }

        /// <summary>
        /// 高效的获得直系子目录的个数
        /// </summary>
        /// <returns></returns>
        public int GetChildCountSlim()
        {
            return VirtualDirectoryCommon.GetChildCount(this.Id);
        }

        #endregion


        #region 父目录


        [PropertyRepository(Lazy = true)]
        [PropertySet("SetParent")]
        public static readonly DomainProperty ParentProperty = DomainProperty.Register<VirtualDirectory, VirtualDirectory>("Parent", (obj) => { return VirtualDirectory.Empty; });


        public VirtualDirectory Parent
        {
            get
            {
                return GetValue<VirtualDirectory>(ParentProperty);
            }
            set
            {
                SetValue(ParentProperty, value);
            }
        }
        
        protected virtual void SetParent(VirtualDirectory value)
        {
            if (value == null) return;

            if (value.IsEmpty()) this.Parent = value;
            else
            {
                bool existChild = value.Childs.Contains(this);

                if (!existChild)
                {
                    value._Childs.Add(this);
                    this.Parent = value;
                }
            }
        }

        #endregion 

        #region 路径

        /// <summary>
        /// 路径，该属性的值自动生成，不用存储
        /// </summary>
        [PropertyGet("GetPath")]
        public static readonly DomainProperty PathProperty = DomainProperty.Register<VirtualDirectoryPath, VirtualDirectory>("Path",(directory)=> { return VirtualDirectoryPath.Empty; });

        public VirtualDirectoryPath Path
        {
            get
            {
                return GetValue<VirtualDirectoryPath>(PathProperty);
            }
        }

        protected virtual VirtualDirectoryPath GetPath()
        {
            IVirtualDirectoryRepository repository = Repository.Create<IVirtualDirectoryRepository>();
            var parents = repository.FindParents(this.Id);
            return new VirtualDirectoryPath(parents);
        }

        #endregion

        /// <summary>
        /// 是否为虚拟磁盘的根目录
        /// </summary>
        public bool IsRoot
        {
            get
            {
                return this.Disk.Root.Equals(this);
            }
        }

        //private List<VirtualFile> FriendFiles
        //{
        //    get
        //    {
        //        return GetValue<List<VirtualFile>>(FilesProperty);
        //    }
        //}

        //public ReadOnlyCollection<VirtualFile> Files
        //{
        //    get
        //    {
        //        return this.FriendFiles.AsReadOnly();
        //    }
        //}

        #region 清理目录

        private void Clear()
        {
            //获取子目录（以镜像的方式），不包括孙级
            var childs = VirtualDirectoryCommon.FindChilds(this.Id, QueryLevel.Mirroring);
            foreach (var child in childs)
            {
                this.Disk.Delete(child); //删除子目录
            }

            while(true)
            {
                var files = GetFiles(10);//使用查询方式，可以及时清理内存；没有锁定文件，因为经过分析后，发现增、删、改、转移这4项操作，不会与此处发生严重的并发冲突，不会导致有脏数据
                if (files.Count() == 0) break;
                foreach (var file in files)
                {
                    this.Disk.Delete(file); //删除文件
                }
            }
        }

        /// <summary>
        /// 获取目录下的文件，可以指定每次获取多少个文件
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private IEnumerable<VirtualFile> GetFiles(int count)
        {
            return VirtualFileCommon.FindFilesByDirectory(this.Id, count);
        }

        #endregion

        public bool Belong(VirtualDisk disk)
        {
            return this.Disk.Equals(disk);
        }

        

        ///// <summary>
        ///// 根据路径查找文件
        ///// </summary>
        ///// <returns></returns>
        //public VirtualFile FindFile(string[] path)
        //{
        //    if (path.Length == 0) return VirtualFile.Empty;
        //    if (path.Length == 1)
        //    {
        //        var name = path[0];
        //        var file = this.Files.FirstOrDefault((item) =>
        //        {
        //            return item.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
        //        });
        //        return file ?? VirtualFile.Empty;
        //    }
        //    else
        //    {
        //        var name = path[0];
        //        var dir = this.Childs.FirstOrDefault((item) =>
        //        {
        //            return item.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
        //        });
        //        if (dir.IsEmpty()) return VirtualFile.Empty;
        //        var childPath = new string[path.Length - 1];
        //        path.CopyTo(childPath, 1);
        //        return dir.FindFile(childPath);
        //    }
        //}

        /// <summary>
        /// 根据名称找到直系目录
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public VirtualDirectory FindDirectory(string name)
        {
            var target = this.Childs.FirstOrDefault((t) =>
            {
                return t.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
            });
            return target == null ? VirtualDirectory.Empty : target;
        }

        public VirtualDirectory(Guid id,VirtualDisk disk)
            :base(id)
        {
            this.Disk = disk;
            this.OnConstructed();
        }

        [ConstructorRepository]
        public VirtualDirectory(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }

        public override void OnPreDelete()
        {
            this.Clear(); //删除该目录之前，清空目录数据
            base.OnPreDelete();
        }


        private class VirtualDirectoryEmpty : VirtualDirectory
        {
            public VirtualDirectoryEmpty()
                : base(Guid.Empty, VirtualDisk.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly VirtualDirectory Empty = new VirtualDirectoryEmpty();
        
    }
}
