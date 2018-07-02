using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Dynamic;

using CodeArt.Util;
using CodeArt.DTO;


namespace CodeArt.DomainDriven
{
    internal interface IDynamicObject
    {
        TypeDefine Define
        {
            get;
        }

        /// <summary>
        /// 从dto中加载数据
        /// </summary>
        /// <param name="data"></param>
        void Load(DTObject data);


        /// <summary>
        /// 得到动态对象内部所有的根类型的成员对象（不包括当前对象自身，也不包括空的根对象）
        /// </summary>
        /// <returns></returns>
        IEnumerable<DynamicRoot> GetRoots();

    }
}