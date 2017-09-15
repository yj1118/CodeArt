using System;
using System.Linq;
using System.Xml.Serialization;
using System.Configuration;


namespace CodeArt.DTO
{
    [XmlRoot("SerializationConfig", Namespace = "http://codeart.cn/SerializationConfig.xsd")]
	public class SerializationConfig
	{
        /// <summary>
        /// 序列化框架版本号
        /// </summary>
		[XmlAttribute("version")]
		public int Version { get; set; }

        public static SerializationConfig Current;

        static SerializationConfig()
        {
            Current = ConfigurationManager.GetSection("codeArt.DTO.serialization") as SerializationConfig;
            if (Current == null) Current = new SerializationConfig { Version = 1 };
        }


	}
}
