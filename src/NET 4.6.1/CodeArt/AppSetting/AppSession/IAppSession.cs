using System;
using System.Threading;

namespace CodeArt.AppSetting
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAppSession : IDisposable
    {
        /// <summary>
        /// 指示回话是否初始化
        /// </summary>
        bool Initialized { get; }

        /// <summary>
        /// 初始化应用回话（由于受.net framework底层技术的制约，我们不得不在使用回话之前初始化一次）
        /// </summary>
        void Initialize();

        object GetItem(string name);

        void SetItem(string name, object value);

        bool ContainsItem(string name);
    }
}
