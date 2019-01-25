using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using WebGather.Video;
using WebGather.Video.Tension;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestVideoGather("寻龙诀");//寻龙诀
            

            HttpClient httpClient = new HttpClient();
            string url = "https://movie.douban.com/j/search_subjects?type=movie&tag=%E5%8A%A8%E4%BD%9C&sort=time&page_limit=400&page_start=400";
            var json = httpClient.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            foreach (var item in JToken.Parse(json)["subjects"])
            {
                //Console.WriteLine(item["title"]);
                TestVideoGather(item["title"].ToString());
            }


            while (true)
            {
                Console.Write("输入视频名称：");
                string input = Console.ReadLine();
                Console.WriteLine("==============================================");
            }
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
