using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CodeArt;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace FileSubsystem
{
    [SafeAccess]
    internal sealed class VirtualDiskValidator : ObjectValidator<VirtualDisk>
    {
        public VirtualDiskValidator()
        {

        }

        protected override void Validate(VirtualDisk obj, ValidationResult result)
        {
            Validator.CheckPropertyRepeated(obj, VirtualDisk.MarkedCodeProperty, result);

            //if (ExistsDisk(obj))
            //{
            //    result.AddError("已创建了虚拟磁盘 id:" + obj.Id + "，markedCode:" + obj.MarkedCode + " 不能重复创建!");
            //    return;
            //}
        }


        //private bool ExistsDisk(VirtualDisk disk)
        //{
        //    VirtualDisk target = VirtualDiskCommon.FindById(disk.Id, QueryLevel.Single);
        //    if (target.IsEmpty() || target.Equals(disk)) return false;
        //    if (!string.IsNullOrEmpty(disk.MarkedCode))
        //    {
        //        target = VirtualDiskCommon.FindByMarkedCode(disk.MarkedCode, QueryLevel.Single);
        //        if (target.IsEmpty() || target.Equals(disk)) return false;
        //    }
        //    return true;
        //}

    }
}
