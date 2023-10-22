using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.ServiceModel;
using CodeArt.Util;

namespace RPC.Common
{
    public static class FileServiceUtil
    {
        public static DTObject CreateDirectory(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("createDirectory", (g) =>
            {
                g.DiskId = arg.RootId;
                g.DirectoryId = arg.DirectoryId;
                g.Name = arg.Name;
            });

            return data;
        }

        public static DTObject DeleteExplorerItems(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("deleteExplorerItems", (g) =>
            {
                g.DiskId = arg.RootId;
                g.DirectoryId = arg.DirectoryId;
                g.Items = arg.Items;
            });

            return data;
        }

        public static DTObject GetExplorerItems(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getExplorerItems", (g) =>
            {
                g.DirectoryId = arg.DirectoryId;
                g.Name = arg.Name;
                g.PageSize = arg.PageSize;
                g.PageIndex = arg.PageIndex;
            });

            data.Each("rows", (row) =>
            {
                row["CreateTime"] = row.GetValue<DateTime>("CreateTime").Humanize();
            });

            return data;
        }

        public static DTObject GetRootDirectory(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("GetRootDirectory", (g) =>
            {
                if (arg.UserId != null) g.UserId = arg.UserId;
                if (arg.DiskId != null) g.DiskId = arg.DiskId;
                if (arg.Directory != null) g.Directory = arg.Directory;
            });

            return data;
        }

        public static DTObject GetVirtualDirectory(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("GetVirtualDirectory", (g) =>
            {
                g.Id = arg.Id;
            });

            return data;
        }

        public static DTObject MoveFiles(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("MoveFiles", (g) =>
            {
                g.DirectoryId = arg.DirectoryId;
                g.Ids = arg.Ids;
            });

            return data;
        }

        public static DTObject RenameExplorerItem(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("renameExplorerItem", (g) =>
            {
                g.DiskId = arg.RootId;
                g.Name = arg.Name;
                g.Id = arg.Id;
                g.Type = arg.Type;
            });

            return data;
        }
    }
}
