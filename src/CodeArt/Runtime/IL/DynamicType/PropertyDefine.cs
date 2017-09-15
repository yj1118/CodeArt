using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using CodeArt.Util;

namespace CodeArt.Runtime.IL
{
    public class PropertyDefine
    {
        private PropertyBuilder _property;
        private MethodBuilder _getAccessor;
        private MethodBuilder _setAccessor;


        internal PropertyDefine(PropertyBuilder property, MethodBuilder getAccessor, MethodBuilder setAccessor)
        {
            _property = property;
            _getAccessor = getAccessor;
            _setAccessor = setAccessor;
        }

        private MethodGenerator getMethodGenerator = null;
        public MethodGenerator GetMethod
        {
            get
            {
                if (getMethodGenerator == null)
                {
                    _property.SetGetMethod(_getAccessor);
                    getMethodGenerator = new MethodGenerator(_getAccessor, _getAccessor.GetILGenerator());
                }
                return getMethodGenerator;
            }
        }

        private MethodGenerator setMethodGenerator = null;
        public MethodGenerator SetMethod
        {
            get
            {
                if (setMethodGenerator == null)
                {
                    _property.SetSetMethod(_setAccessor);
                    setMethodGenerator = new MethodGenerator(_setAccessor, _setAccessor.GetILGenerator());
                }
                return setMethodGenerator;
            }
        }
    }
}
