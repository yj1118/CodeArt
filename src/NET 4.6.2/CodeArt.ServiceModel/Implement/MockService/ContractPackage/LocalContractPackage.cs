using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;
using System.IO;

using CodeArt;
using System.Text;
using CodeArt.Util;
using CodeArt.IO;
using CodeArt.DTO;

namespace CodeArt.ServiceModel.Mock
{
    internal sealed class LocalContractPackage: IContractPackage
    {
        /// <summary>
        /// 服务名称->该服务下的契约
        /// </summary>
        private MultiDictionary<string, MockContract> _contracts;

        private Dictionary<string, MockContract> _keyValues;

        private string _folder;

        public LocalContractPackage()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder">存放mock数据的目录</param>
        public void Load(string folder)
        {
            _folder = folder;
            _contracts = new MultiDictionary<string, MockContract>(false);
            _keyValues = new Dictionary<string, MockContract>();

            if (string.IsNullOrEmpty(folder)) return;

            var files = IOUtil.GetAllFiles(folder);
            foreach (var file in files)
            {
                var code = File.ReadAllText(file);
                ParseJSON(code);
            }
        }

        private void ParseJSON(string code)
        {
            var dto = DTObject.CreateReusable(code);
            var items = dto.GetList();
            foreach(var item in items)
            {
                var contract = MockContract.Create(item, this);
                _contracts.Add(contract.ServiceName, contract);

                if(!string.IsNullOrEmpty(contract.Id))
                {
                    _keyValues.Add(contract.Id, contract);
                }
            }
        }


        /// <summary>
        /// 根据服务请求获取契约
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public MockContract Find(ServiceRequest request)
        {
            var contracts = _contracts.GetValues(request.Name);
            if (contracts == null) throw new ServiceException("没有找到服务" + request.Name + "的mock契约");
            MockContract target = BestMatch(contracts, request);
            if (target != null) return target;

            target = ArgumentMatch(contracts, request);
            if (target != null) return target;

            target = GetDefaultContract(contracts);
            if (target != null) return target;

            if (target == null) throw new ServiceException("没有找到服务" 
                                                            + request.Name + "下调用约定为" 
                                                            + request.Argument.GetCode() + "、身份信息为"
                                                            + request.Identity.GetCode() + "的mock数据");
            return target;
        }

        /// <summary>
        /// 最佳匹配，参数和身份都匹配
        /// </summary>
        /// <param name="contracts"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private MockContract BestMatch(IEnumerable<MockContract> contracts, ServiceRequest request)
        {
            foreach (var contract in contracts)
            {
                if (request.Identity == contract.Request.Identity && request.Argument == contract.Request.Argument)
                    return contract;
            }
            return null;
        }

        /// <summary>
        /// 根据参数匹配
        /// </summary>
        /// <param name="contracts"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private MockContract ArgumentMatch(IEnumerable<MockContract> contracts, ServiceRequest request)
        {
            foreach (var contract in contracts)
            {
                if (request.Argument == contract.Request.Argument)
                    return contract;
            }
            return null;
        }

        /// <summary>
        /// 默认匹配
        /// </summary>
        /// <param name="contracts"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private MockContract GetDefaultContract(IEnumerable<MockContract> contracts)
        {
            foreach (var contract in contracts)
            {
                //没有定义参数和身份的契约，就是默认契约
                if (contract.Request.Argument.IsEmpty() && contract.Request.Identity.IsEmpty())
                    return contract;
            }
            return null;
        }

        /// <summary>
        /// 根据契约编号得到契约
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MockContract Find(string id)
        {
            MockContract contract = null;
            if (_keyValues.TryGetValue(id, out contract)) return contract;
            return null;
        }

        public void Reset()
        {
            this.Load(_folder);
        }

    }
}
