using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebGather.Video.Models;

namespace WebGather.Video.Tension
{
    public class TensionHtmlDocHandle : VideoCollectionHandleAbstract, IVideoGather
    {
        public IEnumerable<GatherResult> GetSearchResult(string content)
        {
            string url = $"https://v.qq.com/x/search/?q={content}";
            HtmlDocument htmlDocument = new HtmlDocument();
            string html = HttpClient.GetAsync(url).Result.Content.ReadAsStringAsync().Result.Replace("\r", null).Replace("\n", null).Replace("\t", null);
            htmlDocument.LoadHtml(html);
            var nodes = this.GetHtmlNodes(htmlDocument)?.Select(node => this.Query(node));
            return nodes.Count() >= 2 ? nodes?.Aggregate((a, b) => a.Concat(b)) : nodes.FirstOrDefault();
        }

        public override void Dispose()
        {
            HttpClient.Dispose();
        }

        protected override bool CheckPlats(HtmlNode htmlNode)
        {
            return htmlNode.ChildNodes.Count(x => x.HasClass("mod_play_source")) > 0;
        }



        protected override bool CheckList(HtmlNode htmlNode)
        {
            return htmlNode.ChildNodes.Count(x => x.HasClass("result_episode_list") || x.HasClass("_playlist")) > 0;
        }

        protected override IEnumerable<GatherResult> OnOnePlatAndButtons(HtmlNode htmlNode)
        {
            var oldStat = htmlNode.Attributes["_oldstat"].Value.Split("&");
            string sval8 = oldStat.First(s => s.Contains("sval8")).Split("=").Last();
            string id = htmlNode.Attributes["data-id"].Value;
            string plname = string.Join("", htmlNode.ChildNodes.FirstOrDefault(child => child.HasClass("result_source"))?.Attributes["r-props"]?.Value?.SkipWhile(s => s != '\'').Skip(1).TakeWhile(s => s != '\'') ?? "");
            string plat = sval8.Split("%2526").First(s => s.Contains("plat")).Split("%253D").Last();
            string type = "5";//单按钮(立即播放)写死 5
            string data_type = sval8.Split("%26").First(s => s.Contains("data%5Ftype")).Split("%3D").Last();
            string video_type = sval8.Split("%26").First(s => s.Contains("video%5Ftype")).Split("%3D").Last();
            string otype = "json";
            string uid = oldStat.First(s => s.Contains("guid")).Split("=").Last().Replace("%2D", "-");
            string callback = "";
            string _t = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

            List<GatherResult> result = new List<GatherResult>();
            var link = $"https://s.video.qq.com/get_playsource?id={id}&plat={plat}&type={type}&data_type={data_type}&video_type={video_type}&plname={plname}&otype={otype}&callback={callback}";
            var gatherResult = GetGatherResult(link, htmlNode);
            var tagA = htmlNode.ChildNodes.FirstOrDefault(node => node.HasClass("result_btn_line"))?.Descendants("a");
            if (tagA != null && tagA.Count() > 1)
            {
                gatherResult.DramaList = gatherResult.DramaList.Concat(tagA.Skip(1).Select(a => new Drama()
                {
                    Title = a.Descendants("span").First().InnerText,
                    Pic = gatherResult.DramaList.FirstOrDefault()?.Pic,
                    Link = a.Attributes["href"].Value
                }).SkipLast(1));
            }
            result.Add(gatherResult);
            return result;
        }

        protected override GatherResult OnOnePlatAndList(HtmlNode htmlNode)
        {
            var oldStat = htmlNode.Attributes["_oldstat"].Value.Split("&");
            string sval8 = oldStat.First(s => s.Contains("sval8")).Split("=").Last();
            string id = htmlNode.Attributes["data-id"].Value;
            string plname = string.Join("", htmlNode.ChildNodes.FirstOrDefault(child => child.HasClass("result_source"))?.Attributes["r-props"].Value.SkipWhile(s => s != '\'').Skip(1).TakeWhile(s => s != '\'') ?? "");
            string plat = sval8.Split("%2526").First(s => s.Contains("plat")).Split("%253D").Last();
            string type = "4";
            string range = this.GetRange(htmlNode);

            string data_type = sval8.Split("%26").First(s => s.Contains("data%5Ftype")).Split("%3D").Last();
            string video_type = sval8.Split("%26").First(s => s.Contains("video%5Ftype")).Split("%3D").Last();
            string otype = "json";
            string uid = oldStat.First(s => s.Contains("guid")).Split("=").Last().Replace("%2D", "-");
            string callback = "";
            string _t = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            string link = $"https://s.video.qq.com/get_playsource?id={id}&plname={plname}&range={range}&plat={plat}&type={type}&data_type={data_type}&video_type={video_type}&otype={otype}&uid={uid}&callback={callback}&_t={_t}";
            return GetGatherResult(link, htmlNode);
        }

        protected override IEnumerable<GatherResult> OnPlatsAndButton(HtmlNode htmlNode)
        {
            var oldStat = htmlNode.Attributes["_oldstat"].Value.Split("&");
            return htmlNode.ChildNodes.First(d => d.HasClass("play_source_list")).Descendants("li").Select(d =>
            {
                string defaultStr = string.Empty;
                var @params = JsonConvert.DeserializeObject<ReqParamModel>(d.Attributes["data-params"].Value.Replace("&quot;", "\""));

                if (@params != null)
                {
                    var id = @params.id;
                    var plname = @params.plname;
                    var plat = @params.plat;
                    var type = @params.type;
                    var data_type = @params.data_type;
                    var video_type = @params.video_type;
                    var otype = "json";
                    var uid = oldStat.First(s => s.Contains("guid")).Split("=").Last().Replace("%2D", "-");
                    var callback = string.Empty;
                    string _t = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                    defaultStr = $" https://s.video.qq.com/get_playsource?id={id}&plname={plname}&plat={plat}&type={type}&data_type={data_type}&video_type={video_type}&otype={otype}&uid={uid}&callback={callback}&_={_t}";
                }
                else
                {
                    string id = htmlNode.Attributes["data-id"].Value;
                    string sval8 = oldStat.First(s => s.Contains("sval8")).Split("=").Last();
                    string plname = string.Join("", htmlNode.ChildNodes.FirstOrDefault(child => child.HasClass("result_source"))?.Attributes["r-props"].Value.SkipWhile(s => s != '\'').Skip(1).TakeWhile(s => s != '\'') ?? "");
                    string plat = sval8.Split("%2526").First(s => s.Contains("plat")).Split("%253D").Last();
                    string type = "4";
                    string range = this.GetRange(htmlNode);
                    string data_type = sval8.Split("%26").First(s => s.Contains("data%5Ftype")).Split("%3D").Last();
                    string video_type = sval8.Split("%26").First(s => s.Contains("video%5Ftype")).Split("%3D").Last();
                    string otype = "json";
                    string uid = oldStat.First(s => s.Contains("guid")).Split("=").Last().Replace("%2D", "-");
                    string callback = string.Empty;
                    string _t = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                    defaultStr = $"https://s.video.qq.com/get_playsource?id={id}&plname={plname}&plat={plat}&type={type}&range={range}&data_type={data_type}&video_type={video_type}&otype={otype}&uid={uid}&callback={callback}&_={_t}";
                }
                return defaultStr;
            }).Select(link => GetGatherResult(link, htmlNode));
        }

        protected override IEnumerable<GatherResult> OnPlatsAndList(HtmlNode htmlNode)
        {
            var oldStat = htmlNode.Attributes["_oldstat"].Value.Split("&");
            var lis = htmlNode.ChildNodes.First(d => d.HasClass("play_source_list")).Descendants("li");

            return lis.Select(d =>
            {
                string deafault = string.Empty;
                var @params = JsonConvert.DeserializeObject<ReqParamModel>(d.Attributes["data-params"].Value.Replace("&quot;", "\""));
                if (@params != null)
                {
                    var id = @params.id;
                    var plname = @params.plname;
                    var plat = @params.plat;
                    var type = 4;//@params.type
                    var data_type = @params.data_type;
                    string range = this.GetRange(htmlNode);

                    var video_type = @params.video_type;
                    var otype = "json";
                    var uid = oldStat.First(s => s.Contains("guid")).Split("=").Last().Replace("%2D", "-");
                    var callback = string.Empty;
                    string _t = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                    deafault = $"https://s.video.qq.com/get_playsource?id={id}&plname={plname}&plat={plat}&type={type}&range={range}&data_type={data_type}&video_type={video_type}&otype={otype}&uid={uid}&callback={callback}&_={_t}";
                }
                else
                {
                    string sval8 = oldStat.First(s => s.Contains("sval8")).Split("=").Last();
                    string id = htmlNode.Attributes["data-id"].Value;
                    string plname = string.Join("", htmlNode.ChildNodes.FirstOrDefault(child => child.HasClass("result_source"))?.Attributes["r-props"].Value.SkipWhile(s => s != '\'').Skip(1).TakeWhile(s => s != '\'') ?? "");
                    string plat = sval8.Split("%2526").First(s => s.Contains("plat")).Split("%253D").Last();
                    string type = "4";
                    string range = this.GetRange(htmlNode);

                    string data_type = sval8.Split("%26").First(s => s.Contains("data%5Ftype")).Split("%3D").Last();
                    string video_type = sval8.Split("%26").First(s => s.Contains("video%5Ftype")).Split("%3D").Last();
                    string otype = "json";
                    string uid = oldStat.First(s => s.Contains("guid")).Split("=").Last().Replace("%2D", "-");
                    string callback = string.Empty;
                    string _t = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                    deafault = $"https://s.video.qq.com/get_playsource?id={id}&plname={plname}&plat={plat}&type={type}&range={range}&data_type={data_type}&video_type={video_type}&otype={otype}&uid={uid}&callback={callback}&_={_t}";
                }
                return deafault;
            }).Select(link => GetGatherResult(link, htmlNode));
        }

        /// <summary>
        /// 根据api链接和html节点 获取完整的解析结果
        /// </summary>
        /// <param name="link"></param>
        /// <param name="htmlNode"></param>
        /// <returns></returns>
        private GatherResult GetGatherResult(string link, HtmlNode htmlNode)
        {
            var res = HttpClient.GetAsync(link).Result;
            string html = res.Content.ReadAsStringAsync().Result;
            string json = string.Join(null, html.SkipWhile(c => c != '{').TakeWhile(c => c != ';'));
            TensionJsonResult tensionJsonResult = JsonConvert.DeserializeObject<TensionJsonResult>(json);

            var result = new GatherResult
            {
                Title = $"{htmlNode.Descendants("h2").FirstOrDefault(x => x.HasClass("result_title"))?.InnerText}",
                Description = $"{htmlNode.Descendants("span").FirstOrDefault(x => x.HasClass("desc_text"))?.InnerText}",
                Pic = $"https:{htmlNode.Descendants("a").FirstOrDefault(x => x.HasClass("result_figure")).Descendants("img").First().Attributes["src"].Value}",
                OwinPlat = $"{tensionJsonResult?.PlaylistItem?.name}",
                OwinApi = link,
                DramaList = tensionJsonResult?.PlaylistItem?.videoPlayList.Select(x => new Drama
                {
                    Title = x.title,
                    Pic = x.pic,
                    Link = x.playUrl
                })
            };
            return result;
        }

        /// <summary>
        /// 从完整的网页中筛选出可供解析的html节点列表
        /// </summary>
        /// <param name="htmlDocument">搜索结果返回的静态html页面</param>
        /// <returns>通过制定查找规则返回可供解析的html节点</returns>
        private IEnumerable<HtmlNode> GetHtmlNodes(HtmlDocument htmlDocument)
        {
            return htmlDocument
                .DocumentNode
                .SelectNodes("/html/body/div[2]/div[2]/div[1]/div")
                .Where(node => node.GetAttributeValue("data-id", string.Empty) != string.Empty);
        }

        /// <summary>
        /// 获取Range参数
        /// </summary>
        /// <param name="htmlNode"></param>
        /// <returns></returns>
        private string GetRange(HtmlNode htmlNode)
        {
            JObject propsJObject = JObject.Parse(htmlNode.ChildNodes.FirstOrDefault(node => node.HasClass("_playlist")).FirstChild.Attributes["r-props"].Value.Replace(";", ","));
            if (propsJObject.TryGetValue("activeRange", out JToken jToken))
            {
                return jToken.ToString();
            }
            else if (propsJObject.TryGetValue("activeId", out JToken jToken2))
            {
                return jToken2.ToString();
            }
            return DateTime.Now.Year.ToString();
        }

    }
}
