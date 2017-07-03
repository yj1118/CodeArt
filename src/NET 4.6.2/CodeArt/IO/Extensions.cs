using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;

namespace CodeArt.IO
{
    /// <summary>
    /// 基元数据转字节
    /// </summary>
    public static class Extensions
    {
        #region 字符串转字节

        public static byte[] GetBytes(this string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        public static int GetBytes(this string str, Encoding encoding,byte[] buffer,int offset)
        {
            return encoding.GetBytes(str, 0, str.Length, buffer, offset);
        }

        public static byte[] GetBytes(this string str)
        {
            return GetBytes(str, Encoding.UTF8);
        }

        public static int GetBytes(this string str, byte[] buffer, int offset)
        {
            return GetBytes(str, Encoding.UTF8, buffer, offset);
        }

        public static string GetString(this byte[] buffer, int index, int count, Encoding encoding)
        {
            return encoding.GetString(buffer, index, count);
        }

        public static string GetString(this byte[] buffer, Encoding encoding)
        {
            return GetString(buffer, 0, buffer.Length, encoding);
        }

        public static string GetString(this byte[] bytes, int index, int count)
        {
            return GetString(bytes, index, count, Encoding.UTF8);
        }

        public static string GetString(this byte[] buffer)
        {
            return GetString(buffer, Encoding.UTF8);
        }

        #endregion

        #region 整型

        public static byte[] GetBytes(this int value)
        {
            //return BitConverter.GetBytes(value);
            var bytes = new byte[4];
            bytes[0] = (byte)(value & 0xFF);
            bytes[1] = (byte)((value & 0xFF00) >> 8);
            bytes[2] = (byte)((value & 0xFF0000) >> 16);
            bytes[3] = (byte)((value >> 24) & 0xFF);
            return bytes;
        }

        /// <summary>
        /// 将value的字节形式写入到buffer中
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        /// <param name="offset">从该偏移量开始写入int的4字节</param>
        public static void GetBytes(this int value, byte[] buffer, int offset)
        {
            //return BitConverter.GetBytes(value);
            buffer[offset] = (byte)(value & 0xFF);
            buffer[offset + 1] = (byte)((value & 0xFF00) >> 8);
            buffer[offset + 2] = (byte)((value & 0xFF0000) >> 16);
            buffer[offset + 3] = (byte)((value >> 24) & 0xFF);
        }

        public static int ToInt(this byte[] buffer, int startIndex)
        {
            return BitConverter.ToInt32(buffer, startIndex);
        }

        public static int ToInt(this byte[] buffer)
        {
            return BitConverter.ToInt32(buffer, 0);
        }

        #endregion

        /// <summary>
        /// 读取流中的字节数，非常安全，就算流不支持length读写也能得到字节数
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this Stream stream, int bufferSize = 512)
        {
            var fs = stream as FileStream;
            if (fs != null)
            {
                //暂时不知道为什么FileStream的 stream.Read(buffer, offset, count); 方法会返回0
                var buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                return buffer;
            }

            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int retval = stream.ReadPro(buffer, 0, bufferSize);
                while (retval == bufferSize)
                {
                    ms.Write(buffer, 0, bufferSize);
                    retval = stream.ReadPro(buffer, 0, bufferSize);
                }
                // 写入剩余的字节。
                ms.Write(buffer, 0, retval);
                data = ms.ToArray();
            }
            return data;
        }

        /// <summary>
        /// 这是对 Stream.Read(byte[] buffer, int offset, int count)方法的强化版
        /// <para>因为在某些情况下，比如http流等特别的流对象下，就算数据有count个，一次读取也未必可以读取出来</para>
        /// <para>需要多次读取，所以我们提供了ReadPro版本来解决这个问题</para>
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int ReadPro(this Stream stream, byte[] buffer, int offset, int count)
        {
            int actualLength = 0;
            do
            {
                int readCount = stream.Read(buffer, offset, count);
                if (readCount == 0)
                {
                    break;
                }
                offset += readCount;
                actualLength += readCount;
                count -= readCount;
            }
            while (count > 0);
            return actualLength;
        }


        #region 数值


        private const int _gb = 1024 * 1024 * 1024;
        private const int _mb = 1024 * 1024;
        private const int _kb = 1024;

        /// <summary>
        /// 将字节转为G
        /// </summary>
        /// <param name="value"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static decimal ToGB(this long value)
        {
            return Math.Round((decimal)((decimal)value / _gb), 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 将字节转为M
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal ToMB(this long value)
        {
            return Math.Round((decimal)((decimal)value / _mb), 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 将字节转为K
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal ToKB(this long value)
        {
            return Math.Round((decimal)((decimal)value / _kb), 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 获得友好的体积大小
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSize(this long value)
        {
            if (value >= _gb) return string.Format("{0:0.##}G", value.ToGB());
            if (value >= _mb) return string.Format("{0:0.##}M", value.ToMB());
            if (value >= _kb) return string.Format("{0:0.##}K", value.ToKB());
            return string.Format("{0:0.##}B", value);
        }

        /// <summary>
        /// 计算出value GB对应的字节数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long GBToByte(this int value)
        {
            return (long)value * _gb;
        }

        public static long GBToByte(this float value)
        {
            return (long)value * _gb;
        }


        #endregion

    }
}
