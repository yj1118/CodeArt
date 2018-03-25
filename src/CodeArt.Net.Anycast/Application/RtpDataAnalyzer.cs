using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Net;

using CodeArt.Util;
using CodeArt.DTO;
using CodeArt.IO;

namespace CodeArt.Net.Anycast
{
    internal static class RtpDataAnalyzer
    {
        public static RtpData Deserialize(string orgin, byte[] data)
        {
            using (var temp = ByteBuffer.Borrow(data.Length))
            {
                var array = temp.Item;
                array.Write(data);

                var participantDataLength = array.ReadInt32();
                var participantData = array.ReadBytes(participantDataLength);

                var extensionsDataLength = array.ReadInt32();
                var extensionsData = array.ReadBytes(extensionsDataLength);

                var bodyLength = array.ReadInt32();
                var body = array.ReadBytes(bodyLength);

                return new RtpData(DataAnalyzer.DeserializeParticipant(orgin, participantData), CreateExtensions(extensionsData), body);
            }
        }

        private static DTObject CreateExtensions(byte[] data)
        {
            return DTObject.Create(data);
        }


        public static byte[] Serialize(RtpData data)
        {
            using (var temp = ByteBuffer.Borrow(data.Body.Length))
            {
                var array = temp.Item;

                var participantData = DataAnalyzer.SerializeParticipant(data.Participant);
                var extensionssData = GetExtensionsData(data.Header);

                array.Write(participantData.Length);
                array.Write(participantData);

                array.Write(extensionssData.Length);
                array.Write(extensionssData);

                array.Write(data.Body.Length);
                array.Write(data.Body);

                return array.ToArray();
            }
        }

        private static byte[] GetExtensionsData(DTObject extensions)
        {
            return extensions.ToData();
        }

    }
}
