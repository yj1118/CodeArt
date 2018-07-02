using System;
using System.Web;
using System.Text;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 考虑到性能问题，没有读取锁
    /// </summary>
    public static class ClientDeviceRegister
    {
        /// <summary>
        /// 移动端检测器，请保证<paramref name="detector"/>是单例的
        /// </summary>
        /// <param name="detector"></param>
        public static void RegisterDetector(IDeviceDetector detector)
        {
            SafeAccessAttribute.CheckUp(detector.GetType());
            _instance = detector;
        }

        public static IDeviceDetector CreateDetector()
        {
            if (_instance == null) return RegexDeviceDetector.Instance;
            return _instance;
        }

        private static IDeviceDetector _instance = null;

    }


}
