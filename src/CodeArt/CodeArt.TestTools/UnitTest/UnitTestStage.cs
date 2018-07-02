using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.Util;
using CodeArt.AppSetting;
using CodeArt.Runtime;


namespace CodeArt.TestTools
{

    /// <summary>
    /// 测试舞台
    /// 每一个测试单元就是舞台中的一个场景，场景有多种事件方法可以使用
    /// </summary>
    public class UnitTestStage : UnitTest
    {
        #region 场景的事件方法

        /// <summary>
        /// 进入场景之前，也就是共生上下文开启之前触发的方法
        /// </summary>
        protected virtual void PreEnterScene() { }

        /// <summary>
        /// 进入场景之后，也就是共生上下文开启之后触发的方法
        /// </summary>
        protected virtual void EnteredScene() { }


        /// <summary>
        /// 离开场景之前，也就是共生上下文关闭之前触发的方法
        /// </summary>
        protected virtual void PreLeaveScene() { }

        /// <summary>
        /// 离开场景之后，也就是共生上下文关闭之后触发的方法
        /// </summary>
        protected virtual void LeftScene() { }

        #endregion

        protected override void EachTestInitialize()
        {
            try
            {
                PreEnterScene();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                Symbiosis.Open();
                EnteredScene();
            }
        }

        protected override void EachTestCleanup()
        {
            try
            {
                PreLeaveScene();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Symbiosis.Close();
                LeftScene();
            }
        }
    }
}
