using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.Util;
using System.Diagnostics;
using CodeArt.Log;

namespace CodeArtTest.Util
{
    /// <summary>
    /// UnitTest1 的摘要说明
    /// </summary>
    [TestClass]
    public class StringSegmentTest
    {
        [TestMethod]
        public void Common()
        {
            string source = "123456789";
            var seg123 = new StringSegment(source, 0, 3);
            Assert.AreEqual("123", seg123.ToString());

            var seg4567 = new StringSegment(source, 3, 4);
            Assert.AreEqual("4567", seg4567.ToString());

            var seg6789 = new StringSegment(source, 5, 4);
            Assert.AreEqual("6789", seg6789.ToString());

            var seg0 = new StringSegment(source, 0, 0);
            Assert.AreEqual("", seg0.ToString());
        }

        [TestMethod]
        public void TrimStart()
        {
            string source = "   12345   6789  ";
            var seg12 = new StringSegment(source, 0, 5).TrimStart();
            Assert.AreEqual("12", seg12.ToString());

            var seg34 = new StringSegment(source, 5, 2).TrimStart();
            Assert.AreEqual("34", seg34.ToString());

        }

        [TestMethod]
        public void TrimEnd()
        {
            string source = "   12345   6789  ";
            var seg89 = new StringSegment(source, 13, 4).TrimEnd();
            Assert.AreEqual("89", seg89.ToString());

            var seg_6789 = new StringSegment(source, 10, 7).TrimEnd();
            Assert.AreEqual(" 6789", seg_6789.ToString());
        }

        [TestMethod]
        public void Trim()
        {
            string source = "   12345   6789  ";

            var seg12345 = new StringSegment(source, 1, 9).Trim();
            Assert.AreEqual("12345", seg12345.ToString());

            seg12345 = new StringSegment(source, 3, 5).Trim();
            Assert.AreEqual("12345", seg12345.ToString());

            var seg0 = new StringSegment(source, 1, 2).Trim();
            Assert.AreEqual("", seg0.ToString());
        }

        [TestMethod]
        public void StartsWith()
        {
            string source = "abcde";
            var seg = new StringSegment(source);

            Assert.IsTrue(seg.StartsWith("abc"));

            Assert.IsTrue(seg.StartsWith('a'));

            source = "ABcde";
            seg = new StringSegment(source);

            Assert.IsFalse(seg.StartsWith("abc"));

            source = "ABcde";
            seg = new StringSegment(source);

            Assert.IsTrue(seg.StartsWith("abc", true));

            source = "ab";
            seg = new StringSegment(source);

            Assert.IsFalse(seg.StartsWith("abc"));

            source = "abcd";
            seg = new StringSegment(source);

            Assert.IsFalse(seg.StartsWith("bc"));

            source = "abcdefg";
            var segDEFG = new StringSegment(source, 3, 4);

            Assert.IsTrue(segDEFG.StartsWith("de"));
            Assert.IsTrue(segDEFG.StartsWith("d"));
            Assert.IsTrue(segDEFG.StartsWith('d'));
        }

        [TestMethod]
        public void EndsWith()
        {
            string source = "abcde";
            var seg = new StringSegment(source);

            Assert.IsTrue(seg.EndsWith("cde"));

            source = "abCDe";
            seg = new StringSegment(source);

            Assert.IsFalse(seg.EndsWith("cde"));

            source = "abCDe";
            seg = new StringSegment(source);

            Assert.IsTrue(seg.EndsWith("cde", true));

            source = "de";
            seg = new StringSegment(source);

            Assert.IsFalse(seg.EndsWith("cde"));

            source = "abcd";
            seg = new StringSegment(source);

            Assert.IsFalse(seg.EndsWith("bc"));

            source = "abcd";
            var segBCD = new StringSegment(source, 1, 3);

            Assert.IsTrue(segBCD.EndsWith("cd"));
            Assert.IsTrue(segBCD.EndsWith('d'));
        }


        [TestMethod]
        public void IndexOfChar()
        {
            string source = "abcde";
            var segBCD = new StringSegment(source, 1, 3);

            var pos = segBCD.IndexOf('a');
            Assert.AreEqual(-1, pos);

            pos = segBCD.IndexOf('b');
            Assert.AreEqual(0, pos);

            pos = segBCD.IndexOf('c');
            Assert.AreEqual(1, pos);

            pos = segBCD.IndexOf('d');
            Assert.AreEqual(2, pos);

            pos = segBCD.IndexOf('e');
            Assert.AreEqual(-1, pos);


            pos = segBCD.IndexOf('B');
            Assert.AreEqual(-1, pos);

            pos = segBCD.IndexOf('B', true);
            Assert.AreEqual(0, pos);

            pos = segBCD.IndexOf('C', true);
            Assert.AreEqual(1, pos);

            pos = segBCD.IndexOf('C');
            Assert.AreEqual(-1, pos);
        }


        [TestMethod]
        public void IndexOfString()
        {
            string source = "abcde";
            var segBCD = new StringSegment(source, 1, 3);

            var pos = segBCD.IndexOf("ab");
            Assert.AreEqual(-1, pos);

            pos = segBCD.IndexOf("b");
            Assert.AreEqual(0, pos);

            pos = segBCD.IndexOf("bc");
            Assert.AreEqual(0, pos);

            pos = segBCD.IndexOf("cd");
            Assert.AreEqual(1, pos);

            pos = segBCD.IndexOf("d");
            Assert.AreEqual(2, pos);

            pos = segBCD.IndexOf("de");
            Assert.AreEqual(-1, pos);

            pos = segBCD.IndexOf("e");
            Assert.AreEqual(-1, pos);



            pos = segBCD.IndexOf("B");
            Assert.AreEqual(-1, pos);

            pos = segBCD.IndexOf("bC");
            Assert.AreEqual(-1, pos);

            pos = segBCD.IndexOf("B", true);
            Assert.AreEqual(0, pos);

            pos = segBCD.IndexOf("BCD", true);
            Assert.AreEqual(0, pos);


            pos = segBCD.IndexOf("C", true);
            Assert.AreEqual(1, pos);

            pos = segBCD.IndexOf("CD", true);
            Assert.AreEqual(1, pos);


            pos = segBCD.IndexOf("C");
            Assert.AreEqual(-1, pos);


            pos = segBCD.IndexOf("CDE", true);
            Assert.AreEqual(-1, pos);


            string longSource = "abcdefghijklmnop";
            var ghijkl = new StringSegment(longSource, 6, 6);

            pos = ghijkl.IndexOf("ijkl");
            Assert.AreEqual(2, pos);

            pos = ghijkl.IndexOf("gh");
            Assert.AreEqual(0, pos);

            pos = ghijkl.IndexOf("kl");
            Assert.AreEqual(4, pos);

            pos = ghijkl.IndexOf("l");
            Assert.AreEqual(5, pos);

            pos = ghijkl.IndexOf("m");
            Assert.AreEqual(-1, pos);

            pos = ghijkl.IndexOf("f");
            Assert.AreEqual(-1, pos);

            pos = ghijkl.IndexOf("a");
            Assert.AreEqual(-1, pos);

            pos = ghijkl.IndexOf("p");
            Assert.AreEqual(-1, pos);

        }


        [TestMethod]
        public void Substring()
        {
            string code = "abcdefghijklmnop 哈哈哈  嘿嘿";
            var ghijkl = new StringSegment(code, 6, 6);

            var gh = ghijkl.Substring(0, 2);
            Assert.AreEqual("gh", gh.ToString());


            var jkl = ghijkl.Substring(3, 3);
            Assert.AreEqual("jkl", jkl.ToString());

            var l = ghijkl.Substring(5, 1);
            Assert.AreEqual("l", l.ToString());

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                var ll = ghijkl.Substring(5, 2);
                Assert.AreEqual("l", l.ToString());
            });

            var source = new StringSegment(code);

            var 哈哈哈 = source.Substring(16, 6).Trim();
            Assert.AreEqual("哈哈哈", 哈哈哈.ToString());
        }


        //[TestMethod]
        //public void TrimSpeed()
        //{
        //    string source = "   12345   6789  ";
        //    var segment = new StringSegment(source);

        //    Stopwatch temp = new Stopwatch();
        //    temp.Start();
        //    for (var i = 0; i < 10000000; i++)
        //    {
        //        var st = source.Trim();
        //    }
        //    var elapsed0 = temp.ElapsedMilliseconds;
        //    LogWrapper.Default.Debug("elapsed0:"+ elapsed0);
        //    temp.Restart();
        //    for (var i = 0; i < 10000000; i++)
        //    {
        //        var st = segment.Trim();
        //    }
        //    var elapsed1 = temp.ElapsedMilliseconds;
        //    LogWrapper.Default.Debug("elapsed1:" + elapsed1);
        //}

    }
}
