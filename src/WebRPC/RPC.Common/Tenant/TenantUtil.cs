using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;
using RPC.Common;
using CodeArt.AppSetting;

namespace RPC.Common
{
    /// <summary>
    /// 
    /// </summary>
    [SafeAccess()]
    public static class TenantUtil
    {
        public static DTObject GetUserTenants(Action<dynamic> action, string ownerType)
        {
            DTObject arg = DTObject.Create();
            action(arg.Dynamic);
            return GetUserTenants(arg, ownerType);
        }

        public static DTObject GetUserTenants(DTObject arg,string ownerType)
        {
            arg.Dynamic.OwnerType = ownerType;
            DTObject data = null;
            if (arg.Valid("Slim") || arg.Valid("Simple"))
            {
                data = ServiceContext.InvokeDynamic("getTenants", (g) =>
                {
                    g.OwnerType = arg.Dynamic.OwnerType;
                    g.UserId = arg.Dynamic.Id ?? AppSession.PrincipalId;
                    g.Slim = true;
                });
            }
            else
            {
                data = ServiceContext.InvokeDynamic("getTenants", (g) =>
                {
                    g.OwnerType = arg.Dynamic.OwnerType;
                    g.UserId = AppSession.PrincipalId;
                    g.UserTenants = true;
                });
            }

            if (!data.IsEmpty())
            {
                data.Each("rows", (row) =>
                {
                    //因为我们经常遇到的需求是实体编号id，租户编号TenantId，所以先自动转换
                    row.Transform("Id=>TenantId");
                    row.Transform("Id=OwnerId");
                });
            }

            return data;
        }

        public static DTObject GetTenantMemberPage(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("GetTenantMemberPage", (g) =>
            {
                g.Name = arg.Name;
                g.Slim = arg.Slim;
                g.TenantId = AppSession.TenantId;
                g.PageIndex = arg.PageIndex;
                g.PageSize = arg.PageSize;
            });

            return data;
        }

        public static DTObject GetTenantMemberRequestPage(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getTenantMemberRequestPage", (g) =>
            {
                g.TenantId = AppSession.TenantId;
                g.ApplicantName = arg.ApplicantName;
                g.PageIndex = arg.PageIndex;
                g.PageSize = arg.PageSize;
            });

            return data;
        }

        /// <summary>
        /// 获得租户成员可以分配的角色
        /// </summary>
        /// <param name="platformId"></param>
        /// <returns></returns>
        public static DTObject FindTenantMemberRoles(long platformId, bool slim)
        {
           return ServiceContext.InvokeDynamic("findTenantMemberRoles", (g) =>
            {
                g.TenantId = AppSession.TenantId;
                g.PlatformId = platformId;
                g.Slim = slim;
            });
        }

        
        public static DTObject SetTenantMemberRoles(dynamic arg)
        {
            return ServiceContext.InvokeDynamic("setTenantMemberRoles", (g) =>
            {
                g.TenantId = AppSession.TenantId;
                g.AccountId = arg.AccountId;
                g.RoleIds = arg.RoleIds;
            });
        }
        
        public static long BelongTenant(string ownerType,long ownerId,dynamic userId)
        {
            //判断userId是否为租户拥有者marketerId的成员
            var tenant = ServiceContext.InvokeDynamic("belongTenant", (g) =>
            {
                g.OwnerType = ownerType;
                g.OwnerId = ownerId;
                g.UserId = userId;
            });

            return tenant.GetValue<long>("id");
        }


        public static long GetTenantId(string ownerType, long ownerId)
        {
            //判断userId是否为租户拥有者marketerId的成员
            var tenant = ServiceContext.InvokeDynamic("getTenant", (g) =>
            {
                g.OwnerType = ownerType;
                g.OwnerId = ownerId;
            });

            return tenant.GetValue<long>("id");
        }
        public static DTObject AgreeJoinTenant(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("agreeJoinTenant", (g) =>
            {
                g.Id = arg.RequestId;
                g.RoleIds = arg.RoleIds;
            });

            return data;
        }


        public static DTObject ApplyJoinTenant(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("applyJoinTenant", (g) =>
            {
                g.InvitorId = arg.InvitorId;
                g.ApplicantId = arg.ApplicantId;
                g.TenantId = arg.TenantId;
                g.RoleIds = arg.RoleIds;
            });

            return data;
        }

        public static DTObject DeleteTenantMember(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("deleteTenantMember", (g) =>
            {
                g.TenantId = AppSession.TenantId;
                g.UserId = arg.UserId;
            });

            return data;
        }

        public static DTObject AddTenantMember(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("addTenantMember", (g) =>
            {
                g.TenantId = AppSession.TenantId;
                g.UserId = arg.UserId;
            });

            return data;
        }

        public static DTObject AddTenantMember(string ownerType, long ownerId, dynamic userId)
        {
            var data = ServiceContext.InvokeDynamic("addTenantMember", (g) =>
            {
                g.OwnerType = ownerType;
                g.OwnerId = ownerId;
                g.UserId = userId;
            });

            return data;
        }

        public static DTObject RefuseJoinTenant(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("refuseJoinTenant", (g) =>
            {
                g.Id = arg.RequestId;
            });

            return data;
        }

        public static DTObject RemoveTenantMemberRequest(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("removeTenantMemberRequest", (g) =>
            {
                g.Id = arg.RequestId;
            });

            return data;
        }

        #region GetTenantMemberDetail

        /// <summary>
        /// 获得租户成员的详细信息
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static DTObject GetTenantMemberDetail(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getUser", (g) =>
            {
                g.Id = arg.UserId;
                g.TenantId = AppSession.TenantId;
                g.Auth = true;
            });

            data.Transform("auth.roles.id=>value;auth.roles.name=>text");
            CheckTenantCreator(data, arg.GetValue<long>("UserId"));

            return data;
        }

        private static void CheckTenantCreator(DTObject data, long userId)
        {
            var arg = ServiceContext.InvokeDynamic("getTenant", (g) =>
            {
                g.TenantId = AppSession.TenantId;
                g.Creator = true;
            });

            data["IsCreator"] = arg.GetValue<long>("creator.id") == userId ? true : false;
        }

        #endregion

    }
}


