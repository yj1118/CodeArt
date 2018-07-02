using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.DTO;

namespace CodeArt.ServiceModel.Mock
{
    /// <summary>
    /// mock契约，契约是可以被更改的
    /// 每个契约都与一个服务有关，契约执行后，会触发契约事件
    /// </summary>
    public sealed class MockContract
    {
        /// <summary>
        /// 契约编号，可以为空
        /// </summary>
        public string Id
        {
            get;
            private set;
        }

        /// <summary>
        /// 契约的描述
        /// </summary>
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// 与契约有关的服务名称
        /// </summary>
        public string ServiceName
        {
            get
            {
                return this.Request.Name;
            }
        }

        /// <summary>
        /// 契约的请求数据
        /// </summary>
        public ServiceRequest Request
        {
            get;
            private set;
        }

        /// <summary>
        /// 契约的响应数据
        /// </summary>
        public ServiceResponse Response
        {
            get;
            set;
        }

        /// <summary>
        /// 契约执行后会引发的事件
        /// </summary>
        private IContractEvent[] _contractEvents;

        /// <summary>
        /// 契约所在的包
        /// </summary>
        public IContractPackage Package
        {
            get;
            private set;
        }

        public MockContract(string id, 
                            string description, 
                            ServiceRequest request, 
                            ServiceResponse defaultResponse, 
                            IContractEvent[] contractEvents,
                            IContractPackage package)
        {
            this.Id = id;
            this.Description = description;
            this.Request = request;
            this.Response = defaultResponse; //默认的响应
            _contractEvents = contractEvents;
            this.Package = package;
        }

        /// <summary>
        /// 执行契约
        /// </summary>
        public ServiceResponse Invoke()
        {
            RaiseEvents();
            return this.Response;
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        private void RaiseEvents()
        {
            foreach (var ce in _contractEvents)
            {
                ce.RaiseEvent(this);
            }
        }


        #region 静态成员

        public static MockContract Create(DTObject dto, IContractPackage package)
        {
            var id = dto.GetValue<string>("id", string.Empty);
            var description = dto.GetValue<string>("description", string.Empty);

            //request
            var requestDTO = dto.GetObject("request", DTObject.Empty);
            var request = ServiceRequest.Create(requestDTO);

            //response
            var responseDTO = dto.GetObject("response", DTObject.Empty);
            var response = ServiceResponse.Create(responseDTO);

            var events = dto.GetList("events", false) ?? DTObjects.Empty;
            List<IContractEvent> ces = new List<IContractEvent>(events.Count);
            foreach (var e in events)
            {
                ces.Add(ContractEventFactory.CreateCE(e));
            }

            return new MockContract(id, description, request, response, ces.ToArray(), package);
        }

     

        #endregion

    }
}
