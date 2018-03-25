using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Text;

using CodeArt.Util;

namespace CodeArt.DTO
{
    internal static class DeserializerUtil
    {
        public static FindingInfo Find(StringSegment code, int startIndex, char key)
        {
            int pointer = startIndex, lastIndex = code.Length - 1;
            if (pointer > lastIndex) return FindingInfo.Empty;

            int length = 0;
            //StringBuilder pass = new StringBuilder();

            int level = 0;
            bool isInString = false;
            char startChar = char.MinValue;
            while (pointer < code.Length)
            {
                char current = code.GetChar(pointer);
                bool isStart = pointer == 0;
                bool isEnd = pointer == lastIndex;
                char prevWord = isStart ? char.MinValue : code.GetChar(pointer - 1);

                pointer++;

                if ((current == '"' || current == '\'') && (isStart || prevWord != '\\'))
                {
                    if (startChar == char.MinValue || startChar == current) //需要一一对应
                    {
                        isInString = !isInString;
                        //pass.Append(word);
                        length++;
                        startChar = isInString ? current : char.MinValue;
                        continue;
                    }
                }

                if (isInString)
                {
                    //pass.Append(word);
                    length++;
                }
                else
                {
                    if (current == '{' || current == '[') level++;
                    else if (current == '}' || current == ']') level--;

                    bool isEndWord = current == key;

                    if ((isEndWord || isEnd) && level == 0)
                    {
                        if (!isEndWord)
                        {
                            //pass.Append(word);
                            length++;
                        }
                        var pass = new StringSegment(code.Source, code.Offset + startIndex, length);
                        return new FindingInfo(pointer - 1, pass, isEndWord);
                    }

                    //pass.Append(word);
                    length++;
                }
            }

            {
                var pass = new StringSegment(code.Source, code.Offset + startIndex, length);
                return new FindingInfo(pointer - 1, pass, false);
            }
        }


        public struct FindingInfo
        {
            public int KeyPosition
            {
                get;
                private set;
            }

            /// <summary>
            /// 查找该关键词路过的文本
            /// </summary>
            public StringSegment Pass
            {
                get;
                private set;
            }

            /// <summary>
            /// 是否找到了目标字符
            /// </summary>
            public bool Finded
            {
                get;
                private set;
            }

            public FindingInfo(int keyPosition, StringSegment pass, bool finded)
            {
                this.KeyPosition = keyPosition;
                this.Pass = pass.Trim();//Trim很重要，可以把{xx:name, xx:name ,xxx:name } 这种格式的无效空格都移除掉
                this.Finded = finded;
            }

            public static readonly FindingInfo Empty = new FindingInfo(-1, StringSegment.Empty, false);

            public bool IsEmpty()
            {
                return this.KeyPosition < 0;
            }
        }
    }
}