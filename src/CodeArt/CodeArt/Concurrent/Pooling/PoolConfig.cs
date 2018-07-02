using System;
using System.Xml.Serialization;

namespace CodeArt.Concurrent
{
	/// <summary>
    /// 	<para>为Pool{T}配置设置环境</para>
	/// </summary>
	[XmlRoot("Pool", Namespace = "http://codeart.cn/PoolConfig.xsd")]
	public class PoolConfig
	{
		public PoolConfig()
		{
			FetchOrder = PoolFetchOrder.Lifo;
			LoanCapacity = 0; // 不限制
            PoolCapacity = 0; // 不限制
            MaxUses = 0; // 不限制
            MaxLifespan = 0; // 不限制
            MaxRemainTime = 0;
            MaxUses = 0;
		}

		/// <summary>
        /// 设置或获取调用池的方式。
        /// 该值可以是PoolFetchOrder.Fifo或PoolFetchOrder.Lifo
        /// PoolFetchOrder.Lifo项会导致大量的内存用于管理池，这是因为较低的代比交高的代更可能被抛弃
		/// </summary>
		[XmlElement("FetchOrder")]
		public PoolFetchOrder FetchOrder { get; set; }

		/// <summary>
        /// 设置或获取在同一个时间内，最大能够借出的项的数量。
        /// 如果线程池中借出的项数量达到该值，那么下次在借用项时，调用线程将被阻塞，直到有项被返回到线程池中。
        /// 如果该值小于或者等于0，那么项会被马上借给调用线程，默认值是0（无限）	
		/// </summary>
		[XmlElement("LoanCapacity")]
		public int LoanCapacity { get; set; }

		/// <summary>
        /// 获取或设置池中可容纳的最大项数量
        /// 当项被返回到池中时，如果池的容量已达到最大值，那么该项将被抛弃。
        /// 如果该值小于或等于0，代表无限制
        /// 借出数的限制LoanCapacity 会引起阻塞，但PoolCapacity限制是抛弃项，而不是阻塞
        /// 两者使用的场景不同
		/// </summary>
		[XmlElement("PoolCapacity")]
		public int PoolCapacity { get; set; }

		/// <summary>
        /// 获取或设置池中每一项的最大寿命（单位秒）
        /// 如果该值小于或者等于0，则代表允许池中的项无限制存在
		/// </summary>
		[XmlElement("MaxLifespan")]
		public int MaxLifespan { get; set; }


        /// <summary>
        /// 获取或设置池中每一项的停留时间（单位秒）
        /// 如果项在池中超过停留时间，那么抛弃
        /// 如果项被使用，那么停留时间会被重置计算
        /// 如果该值小于或者等于0，则代表允许池中的项无限制存在
        /// </summary>
        [XmlElement("MaxRemainTime")]
        public int MaxRemainTime { get; set; }

		/// <summary>
        /// 获取或设置池中项在被移除或销毁之前，能使用的最大次数
        /// 如果该值小于或等于0，那么可以使用无限次
		/// </summary>
		[XmlElement("MaxUses")]
		public int MaxUses { get; set; }
	}
}
