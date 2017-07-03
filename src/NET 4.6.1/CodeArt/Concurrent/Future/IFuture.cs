using System;
using System.Linq;

namespace CodeArt.Concurrent
{
    /// <summary>
    /// 封装在将来某段时间完成的操作
    /// </summary>
    public interface IFuture
    {
        /// <summary>
        /// 等待future完成
        /// </summary>
        void Wait();

        /// <summary>
        /// 获取当前状态,不会阻塞线程
        /// </summary>
        FutureStatus Status { get; }

        /// <summary>
        /// 获取错误.如果实例没有完成,那么将阻塞线程.
        /// </summary>
        /// <value>如果没有错误,值为null</value>
        Exception Error { get; }

        /// <summary>
        /// 指示当future成功完成时执行action
        /// 如果没有完成则不执行
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        void OnComplete(Action onComplete);

        /// <summary>
        /// 重置future的所有状态，该方法可以令future重用
        /// </summary>
        void Start();

    }

    /// <summary>
    /// 封装在future中进行计算的值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFuture<out T> : IFuture
    {
        /// <summary>
        /// 获取结果.如果实例没有完成,那么将阻塞线程.
        /// </summary>
        /// <value>结果</value>
        /// <exception cref="Exception">
        ///		<para>当future完成时出错 <see cref="IFuture.Error"/> 将被抛出.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///		<para>future被取消执行.</para>
        /// </exception>
        T Result { get; }
    }
}
