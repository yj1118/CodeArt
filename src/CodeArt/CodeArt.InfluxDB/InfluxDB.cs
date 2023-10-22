using InfluxDB.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.DTO;
using System.Configuration;
using InfluxDB.Client.Writes;
using InfluxDB.Client.Api.Domain;
using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.InfluxDB
{
    public class InfluxDB : IDisposable
    {
        private string _url;
        private string _token;
        private string _bucket;
        private string _org;
        private InfluxDBClient _client;

        public InfluxDB(string url, string token, string bucket, string org)
        {
            _url = url;
            _token = token;
            _bucket = bucket;
            _org = org;
            _client = InfluxDBClientFactory.Create(_url, _token);
        }

        /// <summary>
        /// 写入数据,value需要明确标注类型例如float:1f;
        /// </summary>
        /// <param name="measurement"></param>
        /// <param name="tag"></param>
        /// <param name="field"></param>
        public void Write(string measurement, (string name, string value) tag, (string name, object value) field)
        {
            using (var writeApi = _client.GetWriteApi())
            {
                var point = PointData.Measurement(measurement)
                .Tag(tag.name, tag.value)
                .Field(field.name, field.value)
                .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

                writeApi.WritePoint(point, _bucket, _org);
            }
        }

        private string GetQueryString(DTObject arg)
        {
            string qs = string.Empty;
            using (var pool = StringPool.Borrow())
            {
                var query = pool.Item;
                query.AppendFormat("from(bucket: \"{0}\") |> range(start: {1}{2})", _bucket, arg.GetValue<string>("start"), (arg.Exist("stop") ? (", stop: " + arg.GetValue<string>("stop")) : ""));

  
                //过滤条件
                if (arg.Valid("measurement") || arg.Valid("field") || arg.Valid("fields") || arg.Valid("tag"))
                {
                    string temp = "";
                    if (arg.Valid("measurement"))
                    {
                        temp += string.Format("r._measurement == \"{0}\"", arg.GetValue<string>("measurement"));
                    }

                    if (arg.Valid("field"))
                    {
                        temp += string.Format("{0}r._field == \"{1}\"", (temp.Length > 0 ? " and " : ""), arg.GetValue<string>("field"));
                    }

                    if (arg.Valid("fields"))
                    {
                        string str = "";
                        string[] fields = arg.Dynamic.fields.OfType<string>();
                        //for 循环遍历 fields
                        for (int i = 0; i < fields.Count(); i++)
                        {
                            str += string.Format("{0}r._field == \"{1}\"", (i == 0 ? "" : " or "), fields[i]);
                        }

                        temp += string.Format("{0}({1})", (temp.Length > 0 ? " and " : ""), str);
                    }

                    if (arg.Valid("tag"))
                    {
                        var tag = arg.GetObject("tag");
                        temp += string.Format("{0}r.{1} == r.\"{2}\"", (temp.Length > 0 ? " and " : ""), tag.GetValue<string>("name"), tag.GetValue<string>("value"));
                    }

                    query.AppendFormat(" |> filter(fn: (r) => {0})", temp);
                }

                //聚合
                if (arg.Valid("aggregate"))
                {
                    var aggregate = arg.GetObject("aggregate");
                    query.AppendFormat(" |> aggregateWindow(every: {0}, fn: {1}, createEmpty: false)", aggregate.GetValue<string>("interval"), aggregate.GetValue<string>("type"));
                }

                //排序
                if (arg.Valid("sort"))
                {
                    var sort = arg.GetObject("sort");
                    var cs = arg.Exist("columns") ? arg.GetValue<string>("columns") : (arg.GetValue<bool>("time") ? "_time" : "_value");
                    query.AppendFormat(" |> sort(columns: [\"{0}\"], desc: {1})", cs, sort.GetValue<bool>("desc"));
                }

                if (arg.Valid("limit"))
                {
                    var limit = arg.GetObject("limit");
                    query.AppendFormat(" |> limit(n: {0}, offset: {1})", limit.GetValue<int>("count"), limit.GetValue<int>("offset"));
                }
                
                qs = query.ToString();
            }

            return qs;
        }

        public DTObject Query(DTObject arg)
        {
            var query = GetQueryString(arg);

            var list = new List<DTObject>();
            var tables = _client.GetQueryApi().QueryAsync(query, _org).Result;

            var data = DTObject.Create();
            foreach (var table in tables)
            {
                //获取数据
                var item = data.CreateAndPush("rows");
                table.Records.ForEach(record =>
                {
                    var time = record.GetTimeInDateTime().Value.ToLocalTime().ToString("u");
                    item["Time"] = time;
                    item["Measurement"] = record.GetMeasurement();
                    item["Field"] = record.GetField();
                    item["Value"] = record.GetValue();
                });
            }

            return data;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
