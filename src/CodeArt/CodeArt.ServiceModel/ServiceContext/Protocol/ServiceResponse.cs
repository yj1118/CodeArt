using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Runtime.Serialization;

using CodeArt.DTO;

namespace CodeArt.ServiceModel
{
    public struct ServiceResponse
    {
        /// <summary>
        /// 调用状态
        /// </summary>
        public DTObject Status
        {
            get;
            private set;
        }

        /// <summary>
        /// 返回值
        /// </summary>
        public DTObject ReturnValue
        {
            get;
            private set;
        }

        public BinaryResponse Binary
        {
            get;
            private set;
        }

        public ServiceResponse(DTObject status, DTObject returnValue, BinaryResponse binary)
        {
            this.Status = status ?? DTObject.Empty;
            this.ReturnValue = returnValue ?? DTObject.Empty;
            this.Binary = binary;
        }

        public void TryCatch()
        {
            var status = this.Status;
            if (status.GetValue<string>("status", string.Empty) == "failed")
            {
                var msg = status.GetValue<string>("message");
                var userError = status.GetValueWithDefault<bool>("userError", true);
                if (userError)
                    throw new InvokeServiceUserException(msg);
                else
                    throw new InvokeServiceException(msg);
            }
        }

        public static readonly ServiceResponse Empty = new ServiceResponse(null, null, BinaryResponse.Empty);

        public bool IsEmpty()
        {
            return this.Status.IsEmpty() && this.ReturnValue.IsEmpty();
        }


        #region 静态成员

        /// <summary>
        /// 根据dto定义，得到ServiceResponse
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static ServiceResponse Create(DTObject dto, BinaryResponse binaryData)
        {
            var status = dto.GetObject("status", ServiceHostUtil.Success);
            var returnValue = dto.GetObject("returnValue", DTObject.Empty);

            if (!binaryData.IsEmpty()) binaryData.Info = returnValue;
            return new ServiceResponse(status, returnValue, binaryData);
        }

        #endregion

    }
}
