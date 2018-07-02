using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Net;

using CodeArt.Util;
using CodeArt.DTO;

namespace CodeArt.Net.Anycast
{
    internal static class DataAnalyzer
    {
        public static Participant DeserializeParticipant(string orgin, byte[] data)
        {
            DTObject dto = DTObject.Create(data);

            var id = dto.GetValue<string>("id");
            var name = dto.GetValue<string>("name");
            var extensions = dto.GetObject("extensions");
            return new Participant(id, name, extensions)
            {
                Orgin = orgin
            };
        }

        private static DTObject CreateExtensions(byte[] data)
        {
            return DTObject.Create(data);
        }

        public static byte[] SerializeParticipant(Participant participant)
        {
            var dto = DTObject.Create();
            dto.SetValue("id", participant.Id);
            dto.SetValue("name", participant.Name);
            dto.SetObject("extensions", participant.Extensions);
            return dto.ToData();
        }

        private static byte[] GetExtensionsData(DTObject extensions)
        {
            return extensions.ToData();
        }

    }
}
