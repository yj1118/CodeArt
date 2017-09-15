using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.Concurrent;
using CodeArt.TestTools;
using CodeArt.IO;
using System.Text;

namespace CodeArtTest.IO
{
    [TestClass]
    public class SegmentReaderTest
    {
        [TestMethod]
        public void CommonRead()
        {
            ReadBy128BufferSize(50);
            ReadBy128BufferSize(200);
            ReadBy128BufferSize(300);
            ReadBy128BufferSize(500);
            ReadBy128BufferSize(2000);
        }


        private void ReadBy128BufferSize(int contentSize)
        {
            var content = GetBytes(contentSize);
            var blockSize = SegmentSize.Byte128.Value;
            var maxIndex = contentSize / blockSize;
            using (MemoryStream ms = new MemoryStream(content))
            {
                using (var temp = SegmentReader.Borrow(SegmentSize.Byte128))
                {
                    var reader = temp.Item;
                    int index = 0;
                    reader.Read(ms, (seg) =>
                    {
                        if (index == 0)
                        {
                            Assert.IsTrue(seg.IsFirst);
                        }

                        if (index == maxIndex)
                        {
                            Assert.IsTrue(seg.IsLast);
                        }

                        if (index > 0 && index < maxIndex)
                        {
                            Assert.IsFalse(seg.IsFirst || seg.IsLast);
                        }

                        //对比数据
                        byte[] source = null;
                        if (index == maxIndex)
                        {
                            var offset = blockSize * index;
                            var retain = contentSize - offset;
                            source = new byte[retain];
                            Buffer.BlockCopy(content, offset, source, 0, retain);
                        }
                        else
                        {
                            source = new byte[blockSize];
                            var offset = blockSize * index;
                            Buffer.BlockCopy(content, offset, source, 0, blockSize);
                        }

                        var target = seg.GetContent();
                        Assert.IsTrue(target.SequenceEqual(source));


                        index++;
                    });
                    Assert.AreEqual(index, maxIndex + 1);
                    Assert.AreEqual(ms.Position, ms.Length);
                }
            }
        }

        [TestMethod]
        public void CommonReadSlim()
        {
            ReadBy128BufferSizeSlim(50);
            ReadBy128BufferSizeSlim(200);
            ReadBy128BufferSizeSlim(300);
            ReadBy128BufferSizeSlim(500);
            ReadBy128BufferSizeSlim(2000);
        }

        private void ReadBy128BufferSizeSlim(int contentSize)
        {
            var content = GetBytes(contentSize);
            var blockSize = SegmentSize.Byte128.Value;
            var maxIndex = contentSize / blockSize;
            using (MemoryStream ms = new MemoryStream(content))
            {
                using (var temp = SegmentReaderSlim.Borrow(SegmentSize.Byte128))
                {
                    var reader = temp.Item;
                    int index = 0;
                    reader.Read(ms, (seg) =>
                    {
                        //对比数据
                        byte[] source = null;
                        if (index == maxIndex)
                        {
                            var offset = blockSize * index;
                            var retain = contentSize - offset;
                            source = new byte[retain];
                            Buffer.BlockCopy(content, offset, source, 0, retain);
                        }
                        else
                        {
                            source = new byte[blockSize];
                            var offset = blockSize * index;
                            Buffer.BlockCopy(content, offset, source, 0, blockSize);
                        }

                        var target = seg.GetContent();
                        Assert.IsTrue(target.SequenceEqual(source));

                        index++;
                    });
                    Assert.AreEqual(index, maxIndex + 1);
                    Assert.AreEqual(ms.Position, ms.Length);
                }
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
