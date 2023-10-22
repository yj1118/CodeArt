using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// ��ѯ����
    /// </summary>
    public sealed class QueryLevel
    {
        public int Code
        {
            get;
            private set;
        }

        private QueryLevel(int code)
        {
            Code = code;
        }

        internal const int NoneCode = 1;
        internal const int ShareCode = 2;
        internal const int SingleCode = 3;
        internal const int HoldSingleCode = 4;
        internal const int MirroringCode = 5;

        /// <summary>
        /// ����
        /// </summary>
        public static readonly QueryLevel None = new QueryLevel(NoneCode);

        /// <summary>
        /// ֻ��һ���߳̿��Է��ʲ�ѯ�Ľ�����������߳̽��ȴ�
        /// </summary>
        public static readonly QueryLevel Single = new QueryLevel(SingleCode);

        /// <summary>
        /// ֻ��һ���߳̿��Է��ʲ�ѯ�Ľ�������������ѯ�����Ĳ����ڵ����ݣ������߳̽��ȴ�
        /// Ҳ����˵��HoldSingle�����Ա�֤�����ѯ�������������ݺͼ�����������ݶ�����ס
        /// ������������HoldSingle
        /// ���Է�ֹ�ڲ�ѯ�б���̲߳�������
        /// </summary>
        public static readonly QueryLevel HoldSingle = new QueryLevel(HoldSingleCode);

        /// <summary>
        /// ����������ǰ�߳̿������̺߳󣬲���Ӱ�������̻߳�ȡShare��None
        /// ���������̲߳���������ȡSingle��HoldSingle������Ҫ�ȴ�ֻ���̲߳�����ɺ���У�
        /// ���⣬�������߳�����Single����HoldSingle,��ǰ�߳�Ҳ�޷����ֻ��������Ҫ�ȴ�
        /// </summary>
        public static readonly QueryLevel Share = new QueryLevel(ShareCode);

        /// <summary>
        /// �Ծ������ʽ���ض��󣬸�ģʽ�²���ӻ������л�ȡ�������ֱ����������ģʽ����ȫ�µĶ���
        /// </summary>
        public static readonly QueryLevel Mirroring = new QueryLevel(MirroringCode);


        public static implicit operator int(QueryLevel level)
        {
            return level.Code;
        }

        public static bool operator ==(QueryLevel level0, QueryLevel level1)
        {
            if ((object)level0 == null && (object)level1 == null) return true;
            if ((object)level0 == null || (object)level1 == null) return false;
            return level0.Code == level1.Code;
        }

        public static bool operator !=(QueryLevel level0, QueryLevel level1)
        {
            return !(level0 == level1);
        }

        public override bool Equals(object obj)
        {
            QueryLevel target = obj as QueryLevel;
            if (target == null) return false;
            return this.Code == target.Code;
        }

        public override int GetHashCode()
        {
            return this.Code;
        }


        public string GetMSSqlLockCode()
        {
            switch (this.Code)
            {
                case QueryLevel.ShareCode: return string.Empty;
                case QueryLevel.SingleCode: return " with(xlock,rowlock) ";
                case QueryLevel.HoldSingleCode: return " with(xlock,holdlock) ";
                default:
                    return " with(nolock) "; //None��Mirror ��������ģʽ
            }
        }
    }
}