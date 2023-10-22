using System;
using System.Threading.Tasks;

namespace CodeArt.Diagnostics
{
    public static class WorkMachine
    {
        /// <summary>
        /// 执行异步工作<paramref name="times"/>次
        /// </summary>
        /// <param name="action"></param>
        /// <param name="times">需要执行的次数</param>
        public static void DoAsyncWork(Action action, int times)
        {
            Task[] tasks = new Task[times];
            for (int i = 0; i < times; i++)
            {
                Task task = new Task(action);
                tasks[i] = task;
                task.Start(); //异步执行
            }
            Task.WaitAll(tasks);
        }
        
        /// <summary>
        /// 执行同步工作
        /// </summary>
        /// <param name="taskCount"></param>
        /// <param name="action"></param>
        public static void DoSyncWork(Action action, int taskCount)
        {
            for (int i = 0; i < taskCount; i++)
            {
                action();
            }
        }


        public static void DoAsyncWork(params Action[] actions)
        {
            Task[] tasks = new Task[actions.Length];
            for (int i = 0; i < actions.Length; i++)
            {
                Task task = new Task(actions[i]);
                tasks[i] = task;
                task.Start(); //异步执行
            }
            Task.WaitAll(tasks);
        }

        public static void DoSyncWork(params Action[] actions)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i]();
            }
        }

    }
}
