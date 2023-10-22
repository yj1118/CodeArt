using System;
using System.Diagnostics;
using System.IO;

using System.Collections.Generic;

namespace CodeArt.DTO
{
    public interface IDTOWriter
    {
        void Write(string name,DateTime value);

        void Write(string name,string value);
        void Write(string name,float value);
        void Write(string name,double value);
        void Write(string name,uint value);
        void Write(string name,ulong value);
        void Write(string name,ushort value);

        void Write(string name,sbyte value);

        void Write(string name,char value);
        void Write(string name,byte value);
        void Write(string name,bool value);

        void Write(string name,decimal value);
        void Write(string name,long value);
        void Write(string name,short value);
        void Write(string name,int value);
        void Write(string name,Guid value);

       

        void Write(string name, DateTime? value);

        void Write(string name, byte? value);

        void Write(string name, Guid? value);

        void Write(string name, float? value);

        void Write(string name, decimal? value);

        void WriteElement<T>(string name, bool elementIsPrimitive, T telement);

        void Write(string name, object value);

        /// <summary>
        /// 写入一个空数组
        /// </summary>
        /// <param name="name"></param>
        void WriteArray(string name);
    }
}
