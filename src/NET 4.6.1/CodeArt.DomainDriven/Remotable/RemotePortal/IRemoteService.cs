using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.DomainDriven
{
    public interface IRemoteService
    {
        /// <summary>
        /// 根据类型契约，获取远程对象的数据
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        DTObject GetObject(RemoteType remoteType, object id);

        /// <summary>
        /// 通知数据已变更
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        void NotifyUpdated(RemoteType remoteType, object id, IEnumerable<string> addresses);

        /// <summary>
        /// 通知数据已被删除
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        void NotifyDeleted(RemoteType remoteType, object id, IEnumerable<string> addresses);
    }
}
