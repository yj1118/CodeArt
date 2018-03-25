using System;

namespace CodeArt.AOP
{
    public sealed class UnityResolveArgument
    {
        private string _name;
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        private string _value;
        /// <summary>
        /// 参数的值
        /// </summary>
        public string Value
        {
            get { return _value; }
        }

        public UnityResolveArgument(string name, string value)
        {
            _name = name;
            _value = value;
        }
    }
}
