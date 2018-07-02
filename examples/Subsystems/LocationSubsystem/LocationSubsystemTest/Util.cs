using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Dapper;

using CodeArt.DomainDriven.DataAccess;
using LocationSubsystem;


namespace LocationSubsystemTest
{
    public static class Util
    {
        public static Location CreateLocation(string name, long parentId)
        {
            return CreateLocation(name, null, null, parentId);
        }


        public static Location CreateLocation(string name, string markedCode = null, int? sortNumber = null, long? parentId = null, int? childCount = null)
        {
            var cmd = new CreateLocation(name);
            if (markedCode != null) cmd.MarkedCode = markedCode;
            if (sortNumber != null) cmd.SortNumber = sortNumber;
            if (parentId != null) cmd.ParentId = parentId;
            return cmd.Execute();
        }


        public static void AssertLocation(long id, (string name, string markedCode, int sortNumber, long parentId, int childCount) expected)
        {
            DataPortal.Direct<Location>((conn) =>
            {
                {
                    var result = conn.QuerySingle("select * from dbo.location where id=@id", new { id });
                    Assert.AreEqual(expected.name, result.Name);
                    Assert.AreEqual(expected.markedCode, result.MarkedCode);
                    Assert.AreEqual(expected.sortNumber, result.SortNumber);
                    Assert.AreEqual(expected.parentId, result.ParentId);
                }

                {
                    var result = conn.QuerySingle("select count(*) as number from dbo.location where parentId=@id", new { id });
                    Assert.AreEqual(expected.childCount, result.number);
                }

            });
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="allChildCount">包括孙级等所有的子节点个数</param>
        public static void AssertLocationAllChilds(long id, int allChildCount)
        {
            DataPortal.Direct<Location>((conn) =>
            {
                var result = conn.QuerySingle("select lft,rgt from dbo.location where id=@id", new { id });
                var count = conn.QuerySingle("select count(*) as number from dbo.location where lft>@lft and rgt<@rgt", new { lft = result.lft, rgt = result.rgt });
                Assert.AreEqual(allChildCount, count.number);
            });
        }

        public static void AssertLocationPath(string path, Location location)
        {
            Assert.AreEqual(path, location.Path.GetText());
        }

        public static void AssertNotExistLocation(long id)
        {
            DataPortal.Direct<Location>((conn) =>
            {
                var exist = conn.QuerySingle("select count(*) as number from dbo.location where id=@id", new { id });
                Assert.AreEqual(0, exist.number);
            });
        }

        public static (int Left,int Right) GetLocationLR(long id)
        {
            int lft = 0, rgt = 0;
            DataPortal.Direct<Location>((conn) =>
            {
                var result = conn.QuerySingle("select lft,rgt from dbo.location where id=@id", new { id });
                lft = result.lft;
                rgt = result.rgt;
            });
            return (lft, rgt);
        }

        public static int GetLocationAllChildsCount((int Left, int Right) lr)
        {
            int count = 0;
            DataPortal.Direct<Location>((conn) =>
            {
                var result = conn.QuerySingle("select count(*) as number from dbo.location where lft>@lft and rgt<@rgt", new { lft=lr.Left, rgt=lr.Right });
                count = result.number;
            });
            return count;
        }

    }
}
