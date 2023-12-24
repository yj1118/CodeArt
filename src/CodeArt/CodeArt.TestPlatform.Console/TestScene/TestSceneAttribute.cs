using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.TestPlatform
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class TestSceneAttribute : Attribute
    {
        /// <summary>
        /// 场景名称
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        public CleanupLevel CleanupLevel
        {
            get;
            set;
        }

        public bool AutoCleanupScene
        {
            get
            {
                return (this.CleanupLevel & CleanupLevel.Scene) == CleanupLevel.Scene;
            }
        }

        public bool AutoCleanupCase
        {
            get
            {
                return (this.CleanupLevel & CleanupLevel.Case) == CleanupLevel.Case;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">场景名称</param>
        public TestSceneAttribute(string name)
        {
            this.Name = name;
            this.CleanupLevel = CleanupLevel.None;
        }


        #region 辅助
        
        /// <summary>
        /// 获取测试对象，该方法不会缓存结果
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public static ITestScene GetScene(string sceneName)
        {
            ITestScene scene = null;
            if (_scenes.TryGetValue(sceneName, out scene))
            {
                return scene;
            }
            return null;
        }

        private static IEnumerable<TestSceneAttribute> GetAttributes(Type type)
        {
            return type.GetCustomAttributes(typeof(TestSceneAttribute), true).OfType<TestSceneAttribute>();
        }

        private static Dictionary<string, ITestScene> _scenes = new Dictionary<string, ITestScene>(StringComparer.OrdinalIgnoreCase);

        static TestSceneAttribute()
        {
            var types = AssemblyUtil.GetImplementTypes(typeof(ITestScene));
            foreach (var type in types)
            {
                if (type.IsAbstract || type.IsInterface) continue;
                var attrs = GetAttributes(type);
                foreach (var attr in attrs)
                {
                    if (_scenes.ContainsKey(attr.Name))
                        throw new TestException(string.Format("重复的测试类{0}", attr.Name));
                    SafeAccessAttribute.CheckUp(type);
                    var scene = Activator.CreateInstance(type) as ITestScene;
                    if (scene == null) throw new TypeMismatchException(type, typeof(ITestScene));

                    _scenes.Add(attr.Name, scene);
                }
            }
        }

        /// <summary>
        /// 获取当前应用程序由TestClassAttribute标记的所有测试类的名称集合
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetNames()
        {
            return _scenes.Keys;
        }

        public static IEnumerable<ITestScene> GetScenes()
        {
            return _scenes.Values.AsEnumerable();
        }

        #endregion

    }
}

