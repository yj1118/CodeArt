using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CodeArt.IO;
using CodeArt.Util;

namespace CodeArt.Runtime
{
    public static class AssemblyUtil
    {
        /// <summary>
        /// 以不锁定程序集的方式加载程序集
        /// </summary>
        public static Assembly LoadWithNoLock(string assemblyFileName)
        {
            byte[] buffer = File.ReadAllBytes(assemblyFileName);
            return Assembly.Load(buffer);
        }

        /// <summary>
        /// 加载GAC程序集
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static Assembly LoadGACWithNoLock(string assemblyName)
        {
            Assembly reflection = Assembly.ReflectionOnlyLoad(assemblyName);
            byte[] buffer = System.IO.File.ReadAllBytes(reflection.Location);
            return Assembly.Load(buffer);
        }

        /// <summary>
        /// 遍历当前加载的所有程序集
        /// </summary>
        /// <param name="action">请保证action的线程安全</param>
        public static void Each(Action<Assembly> action)
        {
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in _assemblies)
            {
                action(assembly);
            }
        }

        /// <summary>
        /// 并行版
        /// </summary>
        /// <param name="action"></param>
        public static void ParallelEach(Action<Assembly> action)
        {
            Parallel.ForEach(_assemblies, (assembly) =>
            {
                action(assembly);
            });
        }

        public static IEnumerable<T> GetAttributes<T>(Assembly assembly) where T : Attribute
        {
            return assembly.GetCustomAttributes(typeof(T), true).OfType<T>();
        }

        public static T GetAttribute<T>(Assembly assembly) where T : Attribute
        {
            return GetAttributes<T>(assembly).FirstOrDefault();
        }

        /// <summary>
        /// 获取当前应用程序域下所有的程序集中，定义的T的特性的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAttributes<T>() where T : Attribute
        {
            var attrs = new List<T>();
            ParallelEach((assembly) =>
            {
                lock (attrs)
                {
                    attrs.AddRange(GetAttributes<T>(assembly));
                }
            });
            return attrs;
        }


        #region 静态构造

        static AssemblyUtil()
        {
            //主动加载所有程序集，避免GetImplementType方法因程序集没有被加载而造成的BUG
            _assemblies = LoadAssemblies();
        }

        /// <summary>
        /// 当前应用程序加载的所有程序集（不包含匿名寄宿的DynamicMethods程序集）
        /// </summary>
        private static IEnumerable<Assembly> _assemblies;

        private static IEnumerable<string> GetAssemblyFiles()
        {
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            return Directory.EnumerateFiles(directory, "*.dll", SearchOption.AllDirectories);
        }

        private static IEnumerable<Assembly> LoadAssemblies()
        {
            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var dlls = Directory.EnumerateFiles(appDirectory, "*.dll", SearchOption.AllDirectories)
                        .Union(Directory.EnumerateFiles(appDirectory, "*.exe", SearchOption.AllDirectories)); //类似控制台程序的exe里也需要加载分析

            foreach (var fileName in dlls)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(fileName);
                    //var assembly = Assembly.LoadFrom(fileName);//不能使用LoadFile，会重复加载程序集，造成BUG,  LoadFrom不会重复加载
                    //if (!IsAnonymouslyHosted(assembly))
                    //{
                    //    assemblies.Add(assembly);
                    //}
                }
                catch(Exception)
                {
                    //有可能是非.NET程序集，会加载错误，此处忽略掉
                }
            }

            var assemblies = new List<Assembly>();
            var loaded = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in loaded)
            {
                if (!IsAnonymouslyHosted(assembly))
                {
                    assemblies.Add(assembly);
                }
            }

            return assemblies;
        }

        #endregion

        /// <summary>
        /// 遍历当前程序集中所有的类型
        /// </summary>
        /// <param name="action">请保证action的线程安全</param>
        public static void EachType(Action<Type> action)
        {
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in _assemblies)
            {
                Type[] types = GetTypes(assembly);
                foreach (var type in types)
                {
                    action(type);
                }
            }
        }

        public static Assembly Get(string assemblyName)
        {
            return _assemblies.FirstOrDefault((t) => t.GetName().Name == assemblyName);
        }

        /// <summary>
        /// 并行版
        /// </summary>
        /// <param name="action"></param>
        public static void ParallelEachType(Action<Type> action)
        {
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Parallel.ForEach(_assemblies, (assembly) =>
            {
                Type[] types = GetTypes(assembly);
                Parallel.ForEach(types, (type) =>
                {
                    action(type);
                });

            });
        }


        /// <summary>
        /// 获取当前应用程序域中，标记了<typeparamref name="T"/>特性的类型
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypesByAttribute<T>() where T : Attribute
        {
            var attrType = typeof(T);
            var types = new List<Type>();
            ParallelEachType((type) =>
            {
                if (type.IsDefined(attrType, true))
                {
                    lock (types)
                    {
                        types.Add(type);
                    }
                }
            });
            return types;
        }


        /// <summary>
        /// 获取当前应用程序域中，实现了<param name="interfaceType"/>接口的类型
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetImplementTypes(Type interfaceType)
        {
            var implementTypes = new List<Type>();
            ParallelEachType((type) =>
            {
                if (type.ImplementInterface(interfaceType))
                {
                    lock (implementTypes)
                    {
                        implementTypes.Add(type);
                    }
                }
            });
            return implementTypes;
        }

        public static string GetFileName(this Assembly assembly)
        {
            var codeBase = assembly.CodeBase;
            var fileName = codeBase.Substring("file:///".Length);
            fileName = fileName.Replace("/","\\");
            if(fileName.EndsWith(".DLL"))
            {
                fileName = fileName.Substring(0, fileName.Length - ".DLL".Length);
                fileName = string.Format("{0}.dll", fileName);
            }
            return fileName;
        }

        /// <summary>
        /// 获取程序集中，实现了执行接口类型<paramref name="interfaceType"/>的所有类型
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetImplementTypes(this Assembly assembly, Type interfaceType)
        {
            var implementTypes = new List<Type>();
            var types = GetTypes(assembly);
            Parallel.ForEach(types, (type) =>
            {
                if (type.ImplementInterface(interfaceType))
                {
                    lock (implementTypes)
                    {
                        implementTypes.Add(type);
                    }
                }
            });
            return implementTypes;
        }

        /// <summary>
        /// 获取当前应用程序域中，实现了<param name="interfaceType"/>接口的类型(第一个)
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static Type GetImplementType(Type interfaceType)
        {
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in _assemblies)
            {
                Type[] types = GetTypes(assembly);
                foreach (var type in types)
                {
                    if (type.ImplementInterface(interfaceType))
                    {
                        return type;
                    }
                }
            }
            return null;
        }

        public static Type[] GetTypes(Assembly assembly)
        {
            Type[] types = null;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException)
            {
                //var loadEx = (ReflectionTypeLoadException)ex;
                //throw new Exception(loadEx.GetCompleteMessage());
                types = Array.Empty<Type>(); //如果程序集无法获取类型，那么返回空数组
            }
            catch (Exception)
            {
                //throw ex;
                types = Array.Empty<Type>();  //如果程序集无法获取类型，那么返回空数组
            }
            return types;
        }

        /// <summary>
        /// 获取类型定义，不需要填写程序集的名称
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetType(string typeName)
        {
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in _assemblies)
            {
                Type[] types = GetTypes(assembly);
                foreach (var type in types)
                {
                    if (type.FullName.Equals(typeName, StringComparison.CurrentCultureIgnoreCase))
                        return type;
                }
            }
            return null;
        }


        /// <summary>
        /// 创建当前应用程序域中，实现了<typeparamref name="T"/>接口的类型(第一个)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateImplement<T>() where T : class
        {
            Type impType = GetImplementType(typeof(T));
            return impType == null ? null : Activator.CreateInstance(impType) as T;
        }

        /// <summary>
        /// 创建当前应用程序域中，实现了<typeparamref name="T"/>接口的类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> CreateImplements<T>() where T : class
        {
            var impTypes = GetImplementTypes(typeof(T));
            return impTypes.Select<Type, T>((type) =>
            {
                return Activator.CreateInstance(type) as T;
            });
        }

        /// <summary>
        /// 程序集是否为匿名寄宿的DynamicMethods程序集
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static bool IsAnonymouslyHosted(this Assembly assembly)
        {
            return assembly != null && assembly.FullName.StartsWith("Anonymously Hosted DynamicMethods Assembly");
        }


    }
}
