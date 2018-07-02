using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.Concurrent;
using CodeArt.TestTools;
using CodeArt.IO;
using System.Text;

namespace CodeArtTest.IO
{
    [TestClass]
    public class ByteBufferTest
    {
        [TestMethod]
        public void WriteBytes()
        {
            var bytes100 = GetBytes(100);
            using (var temp = ByteBuffer.Borrow(bytes100.Length))
            {
                var bytes = temp.Item;
                bytes.Write(bytes100);

                Assert.AreEqual(bytes.Length, bytes100.Length);
                Assert.AreEqual(bytes.SegmentSize, SegmentSize.Byte128);
                Assert.AreEqual(bytes.AllocatedSegmentCount, 1);

                var buffer = bytes.ReadBytes(bytes.Length);
                Assert.AreEqual(buffer.Length, bytes100.Length);
            }

            var bytes200 = GetBytes(200);
            using (var temp = ByteBuffer.Borrow(SegmentSize.Byte128))
            {
                var bytes = temp.Item;
                bytes.Write(bytes200);

                Assert.AreEqual(bytes.Length, bytes200.Length);
                Assert.AreEqual(bytes.SegmentSize, SegmentSize.Byte128);
                Assert.AreEqual(bytes.AllocatedSegmentCount, 2);

                var buffer = bytes.ReadBytes(bytes.Length);
                Assert.AreEqual(buffer.Length, bytes200.Length);

            }
        }






        private byte[] GetBytes(int count)
        {
            var buffer = new byte[count];
            new Random().NextBytes(buffer);
            return buffer;
        }


    }
}
