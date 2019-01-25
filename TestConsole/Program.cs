using Newtonsoft.Json;
using System;
using WebGather.Video;
using WebGather.Video.Tension;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            TestVideoGather("寻龙诀");//寻龙诀

            //while (true)
            //{
            //    Console.Write("输入视频名称：");
            //    string input = Console.ReadLine();
            //    Console.WriteLine("==============================================");
            //}
            Console.ReadLine();
        }

        /// <summary>
        /// 测试视频链接爬虫
        /// </summary>
        public static void TestVideoGather(string content)
        {
            IVideoGather videoGather = new TensionHtmlDocHandle();
            var result = videoGather.GetSearchResult(content);//古剑奇谭之流月昭明
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
