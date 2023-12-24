namespace CodeArt.ServiceModel.Mock
{
    public interface IContractPackage
    {
        /// <summary>
        /// 根据服务请求获取契约
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        MockContract Find(ServiceRequest request);

        /// <summary>
        /// 根据契约编号得到契约
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        MockContract Find(string id);

        /// <summary>
        /// 重置包内所有契约，使其恢复到初始值
        /// </summary>
        void Reset();

    }
}
