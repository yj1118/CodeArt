using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using CodeArt.DTO;
using CodeArt.Runtime;

namespace CodeArt.TestPlatform
{
    public class TestScene : ITestScene
    {
        public TestSceneAttribute Tip
        {
            get;
            private set;
        }

        public Fixture Fixture
        {
            get;
            private set;
        }

        public void Start()
        {
            try
            {
                InitTip();
                StartTip();

                if(this.Tip.AutoCleanupScene)
                    ServiceUtil.ClearData();

                this.Fixture = new Fixture();
                SceneInitialize();
                var cases = this.GetCaseMethods();
                foreach (var @case in cases)
                {
                    RunTestCase(@case);
                }
            }
            catch(Exception ex)
            {
                TestEvents.RaiseTestError(this, new TestEvents.TestErrorArgs(ex));
            }
            finally
            {
                SceneCleanup();
                this.Fixture.Clear();

                if (this.Tip.AutoCleanupScene)
                    ServiceUtil.ClearData();

                EndTip();
            }
        }

        /// <summary>
        /// 整个场景被执行之前进行的初始化操作
        /// </summary>
        protected virtual void SceneInitialize()
        {

        }

        /// <summary>
        /// 整个场景被执行完后的清理操作
        /// </summary>
        protected virtual void SceneCleanup()
        {
        }


        /// <summary>
        /// 运行测试用例
        /// </summary>
        /// <param name="case"></param>
        private void RunTestCase(MethodInfo @case)
        {
            var tip = Attribute.GetCustomAttribute(@case, typeof(TestCaseAttribute)) as TestCaseAttribute;
            if (tip == null) throw new TestException(@case.Name + "没有标记" + typeof(TestCaseAttribute));

            var ctx = TestContext.Current = new TestContext();
            try
            {
                TestEvents.RaiseTestCaseStart(this, new TestEvents.TestCaseArgs(@case.Name, tip, ctx));

                if (this.Tip.AutoCleanupCase)
                    ServiceUtil.ClearData();

                EachCaseInitialize();
                ctx.Start();
                @case.Invoke(this, null);
                ctx.Success = true;
            }
            catch (Exception ex)
            {
                ex = ex.InnerException ?? ex; //过滤Invoke引起的包装异常类
                TestEvents.RaiseTestError(this, new TestEvents.TestErrorArgs(ex));
            }
            finally
            {
                ctx.Stop();
                TestEvents.RaiseTestCaseEnd(this, new TestEvents.TestCaseArgs(@case.Name, tip, ctx));
                EachCaseCleanup();
                TestContext.Current = null;

                if (this.Tip.AutoCleanupCase)
                    ServiceUtil.ClearData();
            }
        }

        /// <summary>
        /// 每个测试用例被执行之前的初始化操作
        /// </summary>
        protected virtual void EachCaseInitialize()
        {

        }

        /// <summary>
        /// 每个测试用例被执行之后的清理操作
        /// </summary>
        protected virtual void EachCaseCleanup()
        {
        }



        private void InitTip()
        {
            var attr = Attribute.GetCustomAttribute(this.GetType(), typeof(TestSceneAttribute)) as TestSceneAttribute;
            if (attr == null) throw new TestException("没有标记" + typeof(TestSceneAttribute));
            this.Tip = attr;
        }

        /// <summary>
        /// 提示开始测试了
        /// </summary>
        private void StartTip()
        {
            TestEvents.RaiseTestStart(this, new TestEvents.TestSceneArgs(this.Tip));
        }

        /// <summary>
        /// 提示结束测试了
        /// </summary>
        private void EndTip()
        {
            TestEvents.RaiseTestEnd(this, new TestEvents.TestSceneArgs(this.Tip));
        }

        private IEnumerable<MethodInfo> GetCaseMethods()
        {
            return AttributeUtil.GetMethods<TestCaseAttribute>(this.GetType());
        }

    }
}