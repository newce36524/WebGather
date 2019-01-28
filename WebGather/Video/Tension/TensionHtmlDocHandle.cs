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
            var paramModel = new ParamModel
            {
                Id = htmlNode.Attributes["data-id"].Value,
                Plname = this.GetPlname(htmlNode),
                Plat = sval8.Split("%2526").First(s => s.Contains("plat")).Split("%253D").Last(),
                Type = "5",
                Data_type = sval8.Split("%26").First(s => s.Contains("data%5Ftype")).Split("%3D").Last(),
                Video_type = sval8.Split("%26").First(s => s.Contains("video%5Ftype")).Split("%3D").Last(),
                Otype = "json",
                Uid = oldStat.First(s => s.Contains("guid")).Split("=").Last().Replace("%2D", "-"),
                Callback = "",
                Range = this.GetRange(htmlNode),
                CurrentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(),
            };
            var link = $"https://s.video.qq.com/get_playsource?{paramModel}";
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
            return new List<GatherResult>() { gatherResult };
        }

        protected override GatherResult OnOnePlatAndList(HtmlNode htmlNode)
        {
            var oldStat = htmlNode.Attributes["_oldstat"].Value.Split("&");
            string sval8 = oldStat.First(s => s.Contains("sval8")).Split("=").Last();
            var paramModel = new ParamModel
            {
                Id = htmlNode.Attributes["data-id"].Value,
                Plname = this.GetPlname(htmlNode),
                Plat = sval8.Split("%2526").First(s => s.Contains("plat")).Split("%253D").Last(),
                Type = "4",
                Data_type = sval8.Split("%26").First(s => s.Contains("data%5Ftype")).Split("%3D").Last(),
                Video_type = sval8.Split("%26").First(s => s.Contains("video%5Ftype")).Split("%3D").Last(),
                Otype = "json",
                Uid = oldStat.First(s => s.Contains("guid")).Split("=").Last().Replace("%2D", "-"),
                Callback = "",
                Range = this.GetRange(htmlNode),
                CurrentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(),
            };
            var result = this.Retry(param => GetGatherResult($"https://s.video.qq.com/get_playsource?{param}", htmlNode), paramModel, 4);
            return result;
        }

        protected override IEnumerable<GatherResult> OnPlatsAndButton(HtmlNode htmlNode)
        {
            var oldStat = htmlNode.Attributes["_oldstat"].Value.Split("&");
            return htmlNode.ChildNodes.First(d => d.HasClass("play_source_list")).Descendants("li").Select(d =>
            {
                string sval8 = oldStat.First(s => s.Contains("sval8")).Split("=").Last();
                var @params = JsonConvert.DeserializeObject<ReqParamModel>(d.Attributes["data-params"].Value.Replace("&quot;", "\""));
                var paramModel = new ParamModel
                {
                    Id = @params != null ? @params.id : htmlNode.Attributes["data-id"].Value,
                    Plname = @params != null ? @params.plname : this.GetPlname(htmlNode),
                    Plat = @params != null ? @params.plat : sval8.Split("%2526").First(s => s.Contains("plat")).Split("%253D").Last(),
                    Type = @params != null ? @params.type : "4",
                    Data_type = @params != null ? @params.data_type.ToString() : sval8.Split("%26").First(s => s.Contains("data%5Ftype")).Split("%3D").Last(),
                    Video_type = @params != null ? @params.video_type.ToString() : sval8.Split("%26").First(s => s.Contains("video%5Ftype")).Split("%3D").Last(),
                    Otype = "json",
                    Uid = oldStat.First(s => s.Contains("guid")).Split("=").Last().Replace("%2D", "-"),
                    Callback = string.Empty,
                    Range = this.GetRange(htmlNode),
                    CurrentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(),
                };
                return paramModel;
            }).Select(paramModel =>
            {
                var result = this.Retry(param => GetGatherResult($"https://s.video.qq.com/get_playsource?{param}", htmlNode), paramModel, 4);
                return result;
            });
        }

        protected override IEnumerable<GatherResult> OnPlatsAndList(HtmlNode htmlNode)
        {
            var oldStat = htmlNode.Attributes["_oldstat"].Value.Split("&");
            var lis = htmlNode.ChildNodes.First(d => d.HasClass("play_source_list")).Descendants("li");

            return lis.Select(d =>
            {
                var @params = JsonConvert.DeserializeObject<ReqParamModel>(d.Attributes["data-params"].Value.Replace("&quot;", "\""));
                string sval8 = oldStat.First(s => s.Contains("sval8")).Split("=").Last();
                var paramModel = new ParamModel
                {
                    Id = @params != null ? @params.id : htmlNode.Attributes["data-id"].Value,
                    Plname = @params != null ? @params.plname : this.GetPlname(htmlNode),
                    Plat = @params != null ? @params.plat : sval8.Split("%2526").First(s => s.Contains("plat")).Split("%253D").Last(),
                    Type = "4",
                    Range = this.GetRange(htmlNode),
                    Data_type = @params != null ? @params.data_type.ToString() : sval8.Split("%26").First(s => s.Contains("data%5Ftype")).Split("%3D").Last(),
                    Video_type = @params != null ? @params.video_type.ToString() : sval8.Split("%26").First(s => s.Contains("video%5Ftype")).Split("%3D").Last(),
                    Otype = "json",
                    Uid = oldStat.First(s => s.Contains("guid")).Split("=").Last().Replace("%2D", "-"),
                    Callback = string.Empty,
                    CurrentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(),
                };
                return $"https://s.video.qq.com/get_playsource?{paramModel}";
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
            var targetNode = htmlNode.ChildNodes.FirstOrDefault(node => node.HasClass("_playlist"))?.FirstChild;
            if (targetNode != null)
            {
                JObject propsJObject = JObject.Parse(targetNode.Attributes["r-props"].Value.Replace(";", ","));
                if (propsJObject.TryGetValue("activeRange", out JToken jToken))//activeRange
                {
                    return jToken.ToString();
                }
                else if (propsJObject.TryGetValue("activeId", out JToken jToken2))
                {
                    if (jToken2.ToString().Length > 6)//针对爱奇艺 类似数据:6886344511270958589,无法查询
                    {
                        propsJObject = JObject.Parse(targetNode.FirstChild.FirstChild.Attributes["r-props"].Value.Replace(";", ","));
                        if (propsJObject.TryGetValue("range", out JToken jToken3))
                        {
                            return jToken3.ToString();
                        }
                    }
                    return jToken2.ToString();
                }
            }
            return DateTime.Now.Year.ToString();
        }

        private string GetPlname(HtmlNode htmlNode)
        {
            var targetNode = htmlNode.ChildNodes.FirstOrDefault(node => node.HasClass("_playlist"))?.FirstChild;
            if (targetNode != null)
            {
                JObject propsJObject = JObject.Parse(targetNode.Attributes["r-props"].Value.Replace(";", ","));
                if (propsJObject.TryGetValue("playSrc", out JToken jToken))
                {
                    return jToken.ToString();
                }
                else if (propsJObject.TryGetValue("playSrcName", out JToken jToken2))
                {
                    return jToken2.ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// range 重试，解决range字段找不到，但又必须提供的问题
        /// </summary>
        /// <param name="func"></param>
        /// <param name="paramModel"></param>
        /// <param name="retryNum"></param>
        /// <returns></returns>
        private GatherResult Retry(Func<ParamModel, GatherResult> func, ParamModel paramModel, int retryNum)
        {
            GatherResult result = default(GatherResult);
            try
            {
                result = func(paramModel);
                return result;
            }
            catch (Exception)
            {
                for (int i = 1; i <= retryNum; i++)
                {
                    try
                    {
                        paramModel.Range = $"{DateTime.Now.Year - i}";
                        result = func(paramModel);
                        return result;
                    }
                    catch (Exception)
                    {
                    }
                }
                return result;
            }
        }

    }
}
