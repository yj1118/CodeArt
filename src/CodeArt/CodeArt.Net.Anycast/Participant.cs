using CodeArt.DTO;
using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Net;


namespace CodeArt.Net.Anycast
{
    public class Participant
    {
        public string Id
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        internal DTObject Extensions
        {
            get;
            private set;
        }

        /// <summary>
        /// 该成员的来源地址
        /// </summary>
        public string Orgin
        {
            get;
            internal set;
        }


        /// <summary>
        /// 获取数据的扩展信息
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public string GetExtension(string exp)
        {
            return Extensions.GetValue<string>(exp, string.Empty);
        }

        public T GetExtension<T>(string exp,T defaultValue)
        {
            return Extensions.GetValue<T>(exp, defaultValue);
        }

        public bool ContainsExtension(string exp)
        {
            return Extensions.GetValue(exp, false) != null;
        }


        public void SetName(string name)
        {
            this.Name = name;
            this.Data = DataAnalyzer.SerializeParticipant(this);
        }

        public void WriteExtensions(Action<DTObject> action)
        {
            action(this.Extensions);
            this.Data = DataAnalyzer.SerializeParticipant(this);
        }


        public Participant(string id, string name)
            : this(id, name, DTObject.Create())
        {
        }

        internal Participant(string id, string name, DTObject extensions)
        {
            this.Id = id;
            this.Name = name;
            this.Extensions = extensions;
            this.Data = DataAnalyzer.SerializeParticipant(this);
        }

        public Participant Clone()
        {
            return new Participant(this.Id, this.Name, this.Extensions.Clone());
        }

        public byte[] Data
        {
            get;
            private set;
        }

        public override bool Equals(object obj)
        {
            var target = obj as Participant;
            if (target == null) return false;
            return target.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static Participant Empty = new Participant(string.Empty,string.Empty, DTObject.Empty);

    }
}