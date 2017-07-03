using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

using CodeArt.DomainDriven;
using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven.DataAccess
{
    public class SqlLike
    {
        private Position _position;


        /// <summary>
        /// 是否前置匹配
        /// </summary>
        public bool Before
        {
            get
            {
                return (_position & Position.Before) == Position.Before;
            }
            private set
            {
                _position &= ~Position.Before;
                if (value) _position |= Position.Before;
            }
        }

        /// <summary>
        /// 是否后置通配
        /// </summary>
        public bool After
        {
            get
            {
                return (_position & Position.After) == Position.After;
            }
            private set
            {
                _position &= ~Position.After;
                if (value) _position |= Position.After;
            }
        }

        /// <summary>
        /// 匹配的参数名称
        /// </summary>
        public string ParamName
        {
            get;
            private set;
        }

        internal SqlLike(string paramName, bool before,bool after)
        {
            this.ParamName = paramName;
            this.Before = before;
            this.After = after;
        }



        private enum Position : byte
        {
            /// <summary>
            /// 前置通配
            /// </summary>
            Before = 0x1,
            /// <summary>
            /// 后置通配
            /// </summary>
            After = 0x2
        }


    }


}