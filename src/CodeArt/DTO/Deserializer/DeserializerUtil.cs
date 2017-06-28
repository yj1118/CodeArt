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
                char word = code.GetChar(pointer);
                bool isStart = pointer == 0;
                bool isEnd = pointer == lastIndex;
                char prevWord = isStart ? char.MinValue : code.GetChar(pointer - 1);

                pointer++;

                if ((word == '"' || word == '\'') && (isStart || prevWord != '\\'))
                {
                    if (startChar == char.MinValue || startChar == word) //需要一一对应
                    {
                        isInString = !isInString;
                        //pass.Append(word);
                        length++;
                        startChar = isInString ? word : char.MinValue;
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
                    if (word == '{' || word == '[') level++;
                    else if (word == '}' || word == ']') level--;

                    bool isEndWord = word == key;

                    if ((isEndWord || isEnd) && level == 0)
                    {
                        if (!isEndWord)
                        {
                            //pass.Append(word);
                            length++;
                        }
                        var pass = new StringSegment(code.Source, code.Offset + startIndex, length);
                        return new FindingInfo(pointer - 1, pass);
                    }

                    //pass.Append(word);
                    length++;
                }
            }

            {
                var pass = new StringSegment(code.Source, code.Offset + startIndex, length);
                return new FindingInfo(pointer - 1, pass);
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

            public FindingInfo(int keyPosition, StringSegment pass)
            {
                this.KeyPosition = keyPosition;
                this.Pass = pass.Trim();//Trim很重要，可以把{xx:name, xx:name ,xxx:name } 这种格式的无效空格都移除掉
            }

            public static readonly FindingInfo Empty = new FindingInfo(-1, StringSegment.Empty);

            public bool IsEmpty()
            {
                return this.KeyPosition < 0;
            }
        }
    }
}