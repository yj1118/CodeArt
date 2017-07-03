using System;
using System.Reflection.Emit;

namespace CodeArt.Runtime.IL
{
    /// <summary>
    ///		<para>提供多种逻辑操作</para>
    ///		<para>每种操作都包含一个分支判断(分支判断会根据结果执行目标指令)和一个比较操作（比较操作会将比较的结果存入堆栈）</para>
    /// </summary>
    public sealed class LogicOperator
    {
        /// <summary>
        /// 堆栈顶部的值是 false、null或者0
        /// </summary>
        public static LogicOperator IsNull { get; private set; }

        /// <summary>
        /// 堆栈顶部的值是 false、null或者0
        /// </summary>
        public static LogicOperator IsFalse { get; private set; }

        /// <summary>
        /// 堆栈顶部的值是 true、不为null或者不为0
        /// </summary>
        public static LogicOperator IsTrue { get; private set; }

        /// <summary>
        /// 堆栈顶部的两个值相等
        /// </summary>
        public static LogicOperator AreEqual { get; private set; }

        /// <summary>
        /// 堆栈顶部的两个值不相等
        /// </summary>
        public static LogicOperator AreNotEqual { get; private set; }

        /// <summary>
        /// 堆栈转换行为依次为：
        /// 1. 将value1推送到堆栈上
        /// 2. 将value2推送到堆栈上
        /// 3. value2 和 value1 从堆栈中弹出; 如果 value1 大于 value2, 状态为true.
        /// </summary>
        public static LogicOperator GreaterThan { get; private set; }

        /// <summary>
        /// 堆栈转换行为依次为：
        /// 1. 将value1推送到堆栈上
        /// 2. 将value2推送到堆栈上
        /// 3. value2 和 value1 从堆栈中弹出; 如果 value1 小于 value2, 状态为true.
        /// </summary>
        public static LogicOperator LessThan { get; private set; }

        /// <summary>
        /// 堆栈转换行为依次为：
        /// 1. 将value1推送到堆栈上
        /// 2. 将value2推送到堆栈上
        /// 3. value2 和 value1 从堆栈中弹出; 如果 value1 大于或等于 value2, 状态为true.
        /// </summary>
        public static LogicOperator GreaterThanOrEqualTo { get; private set; }

        /// <summary>
        /// 堆栈转换行为依次为：
        /// 1. 将value1推送到堆栈上
        /// 2. 将value2推送到堆栈上
        /// 3. value2 和 value1 从堆栈中弹出; 如果 value1 小于或等于 value2, 状态为true.
        /// </summary>
        public static LogicOperator LessThanOrEqualTo { get; private set; }

        /// <summary>
        /// <para>设置op1的相反IL操作为op2</para>
        /// <para>设置op2的相反IL操作为op1</para>
        /// </summary>
        /// <param name="op1"></param>
        /// <param name="op2"></param>
        private static void SetOpposites(LogicOperator op1, LogicOperator op2)
        {
            op1._opposite = op2;
            op2._opposite = op1;
        }

        static LogicOperator()
        {
            var op1 = new LogicOperator
            {
                //从堆栈中弹出value；如果 value 为 true、非空或非零，则将控制转移到目标指令l
                _branchWriter = (w, l) => w.Emit(OpCodes.Brtrue, l),
                _compareWriter = w => { },//计算堆栈顶部的值，如果是true，那就是true，因此不需要额外的判断
                ArgumentCount = 1
            };
            var op2 = new LogicOperator
            {
                //如果 value 为 false、空引用或零，则将控制转移到目标指令l
                _branchWriter = (w, l) => w.Emit(OpCodes.Brfalse, l),
                //求补（取反）
                _compareWriter = w => { NotBoolean(w); },
                ArgumentCount = 1
            };

            SetOpposites(op1, op2);

            IsTrue = op1;
            IsFalse = IsNull = op2;

            op1 = new LogicOperator
            {
                //从堆栈中弹出 value2 和 value1；如果 value1 等于 value2，则执行分支操作l
                _branchWriter = (w, l) => w.Emit(OpCodes.Beq, l),
                //从堆栈中弹出值2和值1，值1值2相比。
                //如果值1等于值2，被压入堆栈，否则为0压入堆栈。
                _compareWriter = w => w.Emit(OpCodes.Ceq),
                ArgumentCount = 2
            };
            op2 = new LogicOperator
            {
                //將 value2 和 value1 自堆栈取出，如果 value1 不等于 value2，则会执行分支操作l
                _branchWriter = (wde, l) => wde.Emit(OpCodes.Bne_Un, l),
                //不等于的比较
                _compareWriter = w => { w.Emit(OpCodes.Ceq); NotBoolean(w); },
                ArgumentCount = 2
            };

            SetOpposites(op1, op2);

            AreEqual = op1;
            AreNotEqual = op2;

            // 大于
            op1 = new LogicOperator
            {
                //大于
                _branchWriter = (wde, l) => wde.Emit(OpCodes.Bgt, l),
                _compareWriter = w => w.Emit(OpCodes.Cgt),
                ArgumentCount = 2
            };
            op2 = new LogicOperator
            {
                //小于等于
                _branchWriter = (wde, l) => wde.Emit(OpCodes.Ble, l),
                //小于
                _compareWriter = w => w.Emit(OpCodes.Clt),
                ArgumentCount = 2
            };

            SetOpposites(op1, op2);

            GreaterThan = op1;
            LessThanOrEqualTo = op2;

            op1 = new LogicOperator
            {
                //大于等于
                _branchWriter = (wde, l) => wde.Emit(OpCodes.Bge, l),
                //大于等于的比较
                _compareWriter = w => { w.Emit(OpCodes.Clt); NotBoolean(w); },
                ArgumentCount = 2
            };
            op2 = new LogicOperator
            {
                //小于的分支
                _branchWriter = (wde, l) => wde.Emit(OpCodes.Blt, l),
                //小于的比较
                _compareWriter = w => { w.Emit(OpCodes.Cgt); NotBoolean(w); },
                ArgumentCount = 2
            };

            SetOpposites(op1, op2);

            GreaterThanOrEqualTo = op1;
            LessThan = op2;
        }

        /// <summary>
        /// <para>转成bool类型对应的int值</para>
        /// <para>false:0;true:1</para>
        /// <para>0取反得到的数值是-1；1取反得到的数值是-2；因此，我们需要将-1转成1，把-2转成0，才能得到正确的bool对应的整数值</para>
        /// </summary>
        private static void CastBooleanInt(ICILWriter w)
        {
            w.Emit(OpCodes.Ldc_I4_1); w.Emit(OpCodes.And);
        }

        /// <summary>
        /// 对bool取反；注意：在计算堆栈中，bool是以int存放的true:1,false:0
        /// </summary>
        /// <param name="w"></param>
        private static void NotBoolean(ICILWriter w)
        {
            w.Emit(OpCodes.Not); CastBooleanInt(w);
        }

        /// <summary>
        /// <para>MSIL 分支命令写入</para>
        /// <para>接受ICilWriter、ILabel参数，你可以实现满足条件定位到ILabel的逻辑</para>
        /// </summary>
        private Action<ICILWriter, ILabel> _branchWriter;
        /// <summary>
        /// <para>MSIL比较命令的写入</para>
        /// </summary>
        private Action<ICILWriter> _compareWriter;
        /// <summary>
        /// 相反的IL操作
        /// </summary>
        private LogicOperator _opposite;

        private LogicOperator() { }

        /// <summary>
        /// 写入一个比较指令，该指令会将一个int32值推送到计算堆栈上
        /// </summary>
        /// <param name="writer">MSIL写入器</param>
        public void WriteCompare(ICILWriter writer)
        {
            _compareWriter(writer);
        }

        /// <summary>
        /// <para>写入条件转移指令</para>
        /// <para>结果为true时，将跳转到target</para>
        /// </summary>
        /// <param name="writer">写入器</param>
        /// <param name="target">需要跳转的语句标签</param>
        public void WriteBranch(ICILWriter writer, ILabel target)
        {
            _branchWriter(writer, target);
        }

        /// <summary>
        /// 执行操作，需要的参数个数
        /// </summary>
        public int ArgumentCount { get; private set; }

        /// <summary>
        /// 返回相反的操作。例如：如果此实例代表小于，那么<see cref="Negate"/>返回 大于或等于
        /// </summary>
        /// <returns>与此相反的操作，该操作将有完全相反的结果.</returns>
        public LogicOperator Negate()
        {
            return _opposite;
        }
    }
}
