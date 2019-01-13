using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

using CodeArt;
using CodeArt.Util;
using CodeArt.Web;

namespace Module.WebUI
{
    public static class Principal
    {
        public static void Login(object id, string name,string email, int keepMinutes)
        {
            Logout();
            Cookie.SetItem("online", "true", keepMinutes);//session默认情况下是永久保存的
                                                          //cookie默认情况下是关闭浏览器就失效的，我们需要默认情况下，登录系统的用户
                                                          //关闭浏览器后，需要重新登录系统
            Principal.Id = id;
            Principal.Name = name;
            Principal.Email = email;
        }

        public static void Login(object id, string name,string email)
        {
            Login(id, name, email, 0);
        }

        public static void Logout()
        {
            Cookie.RemoveItem("online");
            //Session.Dispose(); 不能释放，因为Principal是基于session的，但是session不是仅为Principal服务的
        }

        public static T GetId<T>()
        {
            var value = Principal.Id;
            if (value == null) throw new WebException("没有找到principal_id的值");
            return (T)value;
        }

        private static void CheckLogin()
        {
            if (!IsLogin)
                throw new UserUIException("尚未登录");
        }


        public static object Id
        {
            get
            {
                return GetItem<object>("id");
            }
            set
            {
                SetItem("id", value);
            }
        }

        public static string Name
        {
            get
            {
                return GetItem<string>("name") ?? string.Empty;
            }
            set
            {
                SetItem("name", value);
            }
        }

        public static string Email
        {
            get
            {
                return GetItem<string>("email") ?? string.Empty;
            }
            set
            {
                SetItem("email", value);
            }
        }

        public static string Photo
        {
            get
            {
                return GetItem<string>("photo") ?? string.Empty;
            }
            set
            {
                SetItem("photo", value);
            }
        }

        public static bool IsLogin
        {
            get
            {
                return Cookie.GetItem("online") != null;
            }
        }

        public static Role[] Roles
        {
            get
            {
                return GetItem<Role[]>("roles") ?? Array.Empty<Role>();
            }
            set
            {
                SetItem("roles", value);
            }
        }

        public static T GetItem<T>(string name)
        {
            if(!IsLogin) return default(T);
            var item = Session.GetItem(string.Format("principal_{0}", name));
            if (item == null) return default(T);
            return (T)item;
        }

        public static void SetItem(string name, object value)
        {
            Session.SetItem(string.Format("principal_{0}", name), value);
        }


        [Serializable]
        public sealed class Role
        {
            /// <summary>
            /// 角色编号
            /// </summary>
            public object Id { get; set; }

            /// <summary>
            /// 角色的唯一标识码
            /// </summary>
            public string MarkedCode { get; set; }

            public T GetId<T>()
            {
                return this.Id == null ? default(T) : (T)this.Id;
            }
        }

        public static bool InRole(string markedCode)
        {
            return Roles.FirstOrDefault((t) =>
            {
                return t.MarkedCode.EqualsIgnoreCase(markedCode);
            }) != null;
        }

    }
}