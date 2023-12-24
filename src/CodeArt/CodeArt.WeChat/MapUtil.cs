using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using CodeArt.AppSetting;
using CodeArt.Util;
using CodeArt.DTO;
using CodeArt.Caching.Redis;
using CodeArt.Web;
using System.Text;

namespace CodeArt.WeChat
{
    public static class MapUtil
    {
        public static DTObject GeoCoder(float latitude, float longitude)
        {
            var key = ConfigurationManager.AppSettings["wx-map-key"];

            var location = $"{latitude},{longitude}";
            var url = $"https://apis.map.qq.com/ws/geocoder/v1/?key={key}&location={location}";
            var bytes = WebUtil.SendGet(url);
            var data = DTObject.Create(Encoding.UTF8.GetString(bytes));

            var message = data.GetValue<string>("message", string.Empty);
            if (message != "query ok") return DTObject.Empty;

            return data;

            //{adcode,lat,lng,nation,nation_code,province,city,city_code,district,street,street_number,address}
        }

    }
}
