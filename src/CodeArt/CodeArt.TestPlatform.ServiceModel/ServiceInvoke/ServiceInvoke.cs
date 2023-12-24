using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DTO;

namespace CodeArt.TestPlatform
{
    /// <summary>
    /// 心愿
    /// </summary>
    [ObjectRepository(typeof(IServiceInvokeRepository))]
    [ObjectValidator(typeof(ServiceInvokeSpecification))]
    public class ServiceInvoke : AggregateRoot<ServiceInvoke, Guid>
    {
        [PropertyRepository()]
        [StringLength(1, 100)]
        [ASCIIString()]
        [NotEmpty()]
        private static readonly DomainProperty NameProperty = DomainProperty.Register<string, ServiceInvoke>("Name");

        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name
        {
            get
            {
                return GetValue<string>(NameProperty);
            }
            private set
            {
                SetValue(NameProperty, value);
            }
        }

        
        [PropertyRepository()]
        [StringLength(1, 2000)]
        [NotEmpty()]
        private static readonly DomainProperty InputCodeProperty = DomainProperty.Register<string, ServiceInvoke>("InputCode");

        /// <summary>
        /// 输入参数的码
        /// </summary>
        public string InputCode
        {
            get
            {
                return GetValue<string>(InputCodeProperty);
            }
            private set
            {
                SetValue(InputCodeProperty, value);
            }
        }

        [PropertyRepository()]
        [NotEmpty()]
        private static readonly DomainProperty OutputCodeProperty = DomainProperty.Register<string, ServiceInvoke>("OutputCode");

        /// <summary>
        /// 返回值的代码
        /// </summary>
        public string OutputCode
        {
            get
            {
                return GetValue<string>(OutputCodeProperty);
            }
            private set
            {
                SetValue(OutputCodeProperty, value);
            }
        }


        [PropertyRepository()]
        private static readonly DomainProperty TimeProperty = DomainProperty.Register<DateTime, ServiceInvoke>("Time");

        /// <summary>
        /// 数据存储时间
        /// </summary>
        public DateTime Time
        {
            get
            {
                return GetValue<DateTime>(TimeProperty);
            }
            private set
            {
                SetValue(TimeProperty, value);
            }
        }


        //[PropertyRepository()]
        //private static readonly DomainProperty InputTimeProperty = DomainProperty.Register<DateTime, ServiceInvoke>("InputTime");

        ///// <summary>
        ///// 获得输入的时间
        ///// </summary>
        //public DateTime InputTime
        //{
        //    get
        //    {
        //        return GetValue<DateTime>(InputTimeProperty);
        //    }
        //    private set
        //    {
        //        SetValue(InputTimeProperty, value);
        //    }
        //}

        //[PropertyRepository()]
        //private static readonly DomainProperty OutputTimeProperty = DomainProperty.Register<DateTime, ServiceInvoke>("OutputTime");

        ///// <summary>
        ///// 获得输出的时间
        ///// </summary>
        //public DateTime OutputTime
        //{
        //    get
        //    {
        //        return GetValue<DateTime>(OutputTimeProperty);
        //    }
        //    private set
        //    {
        //        SetValue(OutputTimeProperty, value);
        //    }
        //}

        public ServiceInvoke(Guid id, string name, string inputCode, string outputCode)
            : base(id)
        {
            this.Name = name;
            this.InputCode = inputCode;
            this.OutputCode = outputCode;
            this.Time = DateTime.Now;

            this.OnConstructed();
        }

        [ConstructorRepository]
        public ServiceInvoke(Guid id, string name, string inputCode, string outputCode, DateTime time)
            : base(id)
        {
            this.Name = name;
            this.InputCode = inputCode;
            this.OutputCode = outputCode;
            this.Time = time;

            this.OnConstructed();
        }

        #region 空对象

        private class ServiceInvokeEmpty : ServiceInvoke
        {
            public ServiceInvokeEmpty()
                : base(Guid.Empty, string.Empty, string.Empty, string.Empty, DateTime.Now)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly ServiceInvoke Empty = new ServiceInvokeEmpty();

        #endregion


    }
}
