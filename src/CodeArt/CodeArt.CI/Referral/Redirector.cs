using CodeArt.Concurrent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.CI.Referral
{
    public interface IRedirector
    {
        void Redirect();
    }

    public abstract class Redirector : IRedirector
    {
        public string FileName
        {
            get;
            private set;
        }

        private IEnumerable<string> _folderSegments; //sln所在目录的片段数组


        public Redirector(string fileName)
        {
            this.FileName = fileName;
            _folderSegments = Util.GetFolderSegments(FileName);
        }

        /// <summary>
        /// 根据workspace的工程配置，重定向路径
        /// </summary>
        public abstract void Redirect();

        /// <summary>
        /// 根据当前解决方案或项目所在的路径，计算出绝对路径<paramref name="absolutePath"/>的相对路径
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <returns></returns>
        protected string GetRelativePath(string absolutePath)
        {
            var absoluteSegments = absolutePath.Split('\\');
            var slnSegments = _folderSegments.ToArray();
            var startCount = 0;
            for (var i = 0; i < slnSegments.Length; i++)
            {
                if (absoluteSegments[i] == slnSegments[i])
                {
                    startCount++;
                }
                else
                    break;
            }

            int prevCount = slnSegments.Length - startCount;

            var end = absoluteSegments.Skip(startCount);
            using (var temp = StringPool.Borrow())
            {
                var path = temp.Item;
                while (prevCount > 0)
                {
                    path.Append(@"..\");
                    prevCount--;
                }

                foreach (var segment in end)
                {
                    path.AppendFormat(@"{0}\", segment);
                }
                path.Length--;
                return path.ToString();
            }
        }

        /// <summary>
        /// 根据前解决方案或项目所在的路径，计算出相对路径<paramref name="relativePath"/>的绝对路径
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        protected string GetAbsolutePath(string relativePath)
        {
            var relativeSegments = new Stack<string>(relativePath.Split('\\').Reverse());
            int prevCount = 0;
            while (true)
            {
                var segment = relativeSegments.Peek();
                if (segment == "..")
                {
                    prevCount++;
                    relativeSegments.Pop();
                }
                else
                    break;
            }


            var slnSegments = new Stack<string>(_folderSegments);
            while (prevCount > 0)
            {
                slnSegments.Pop();
                prevCount--;
            }

            var start = slnSegments.ToArray().Reverse();
            var end = relativeSegments.ToArray();

            List<string> segments = new List<string>();
            segments.AddRange(start);
            segments.AddRange(end);

            return string.Join(@"\", segments);
        }

        protected abstract string GetCode();

        public void Save(string fileName)
        {
            string content = this.GetCode();
            File.WriteAllText(fileName, content);
        }

        public void Save()
        {
            this.Save(FileName);
        }
    }
}
