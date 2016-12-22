using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace subjectnerdagreement.psdexport
{

    public class Word
    {
        public Dictionary<string , string > TypeAndParams = new Dictionary<string, string>();
        public string Context;

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendLine(Context);
            if (TypeAndParams != null)
            {
                foreach (string key in TypeAndParams.Keys)
                {
                    buf.AppendFormat("{0}:{1}\n", key, TypeAndParams[key]);
                }
            }
            return buf.ToString();
        }
    }

    /// <summary>
    /// 字符解析
    /// </summary>
    public class WordParser
    {
        public static Dictionary<string , List<Word>> parseMap = new Dictionary<string, List<Word>>();
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="context"></param>
        public static void BindingParse(GameObject context)
        {
            List<Word> words = ParseLayerName(context.name);

            //解析绑定组件和参数
            foreach (Word word in words)
            {
                if(word.TypeAndParams == null)  continue;

                context.name = words[0].Context;
                foreach (string key in word.TypeAndParams.Keys)
                {
                    Action<string, GameObject> import = null;

                    if (key != "component")
                        import = LayerWordBinder.GetParser(key);
                    else
                        import = LayerWordBinder.GetParser(word.TypeAndParams[key]);

                    if (import == null)
                    {
                        Debug.LogWarning("Cant parse context ! key :" + key + " , value:" + word.TypeAndParams[key]);
                        continue;
                    }

                    import.Invoke(word.TypeAndParams[key] , context);
                }
            }
            
        }


        public static List<Word> ParseLayerName(string layerName)
        {
            if (parseMap.ContainsKey(layerName)) return parseMap[layerName];

            char[] charArr = layerName.ToCharArray();
            int index = 0;
            StringBuilder buf = new StringBuilder();
            List<Word> words = new List<Word>();
            while (index < charArr.Length)
            {
                buf.Append(charArr[index]);
                if (checkEnd(charArr, index + 1) && buf.Length > 1)
                {
                    Word newWord = new Word();
                    newWord.Context = buf.ToString();
                    words.Add(newWord);

                    buf.Length = 0;
                }
                index++;
            }

            //解析组件和参数
            foreach (Word word in words)
            {
                if (word.Context.StartsWith("["))
                {
                    splitSquareBrackets(word);
                }
                else if (word.Context.StartsWith("@"))
                {
                    splitWidgetComponent(word);
                }
            }

            parseMap[layerName] = words;

            return words;
        }


        public static string[] GetTextureExportPath(string layerName)
        {
            List<Word> words = ParseLayerName(layerName);
            foreach (Word word in words)
            {
                if(!word.TypeAndParams.ContainsKey("img"))   continue;

                string paramStr = word.TypeAndParams["img"];
                string[] imgInfo = paramStr.Split('_'); //asset type

                return new []
                {
                    paramStr.Substring(imgInfo[0].Length + 1 , paramStr.Length - imgInfo[0].Length - 1),
                    PsdSetting.Instance.GetAssetFolder(imgInfo[0])
                };
            }

            return new []
            {
                layerName,
                PsdSetting.Instance.DefaultImportPath
            };
        }

        private static bool checkEnd(char[] charArr, int nextIndex)
        {
            if (nextIndex >= charArr.Length)
                return true;

            if (charArr[nextIndex] == '[' || charArr[nextIndex] == '@')
                return true;

            return false;
        }

        /// <summary>
        /// 切分[]括号
        /// 参数形式： [anchor:lr|img:common_pic001]
        /// </summary>
        /// <returns>返回字符总共的长度，包括括号本身</returns>
        private static void splitSquareBrackets(Word word)
        {
            string subStr = word.Context.Substring(1, word.Context.Length - 2);
            string[] args = subStr.Split('|');

            foreach (string param in args)
            {
                string[] paramInfo = param.Split(':');
                word.TypeAndParams[paramInfo[0]] = paramInfo[1];
            }
        }

        /// <summary>
        /// 切分组件名称
        /// </summary>
        /// <param name="word"></param>
        private static void splitWidgetComponent(Word word)
        {
            int index = word.Context.IndexOf("_");
            string subStr = word.Context.Substring(1, index > 0 ? index : word.Context.Length - 1);
            string[] componentAndParams = subStr.Split(':');
            word.TypeAndParams = new Dictionary<string, string>();
            word.TypeAndParams["component"] = componentAndParams[0];
        }
    }



}
