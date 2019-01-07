using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using WebGather.Video.Models;

namespace WebGather.Video
{
    /// <summary>
    /// html页面解析处理抽象类
    /// </summary>
    public abstract class VideoCollectionHandleAbstract : IDisposable
    {
        protected static readonly HttpClient HttpClient = new HttpClient();
        public virtual void Dispose() { }

        /// <summary>
        /// 定义html节点类型的规则，是否拥有剧集列表
        /// </summary>
        /// <param name="htmlNode"></param>
        /// <returns></returns>
        protected abstract bool CheckList(HtmlNode htmlNode);

        /// <summary>
        /// 定义html节点类型的规则，是否拥有多个平台
        /// </summary>
        /// <param name="htmlNode"></param>
        /// <returns></returns>
        protected abstract bool CheckPlats(HtmlNode htmlNode);

        /// <summary>
        /// 检测结果是一个列表单个平台时处理
        /// </summary>
        /// <param name="htmlNode">可供解析的节点</param>
        /// <returns>json api地址</returns>
        protected abstract GatherResult OnOnePlatAndList(HtmlNode htmlNode);

        /// <summary>
        /// 检测结果是一个立即播放按钮单个平台时处理
        /// </summary>
        /// <param name="htmlNode">可供解析的节点</param>
        /// <returns>爬取列表</returns>
        protected abstract IEnumerable<GatherResult> OnOnePlatAndButtons(HtmlNode htmlNode);

        /// <summary>
        /// 检测结果是一个列表多个平台时处理
        /// </summary>
        /// <param name="htmlNode">可供解析的节点</param>
        /// <returns>爬取列表</returns>
        protected abstract IEnumerable<GatherResult> OnPlatsAndList(HtmlNode htmlNode);

        /// <summary>
        /// 检测结果是一个立即播放按钮多个平台时处理
        /// </summary>
        /// <param name="htmlNode">可供解析的节点</param>
        /// <returns>爬取列表</returns>
        protected abstract IEnumerable<GatherResult> OnPlatsAndButton(HtmlNode htmlNode);

        /// <summary>
        /// 从html节点中获取最终解析结果
        /// </summary>
        /// <param name="htmlNode">单个html节点，包含需要的视频信息</param>
        /// <returns>爬取列表</returns>
        public IEnumerable<GatherResult> Query(HtmlNode htmlNode)
        {
            var HasList = CheckList(htmlNode);//是否拥有剧集列表
            var HasPlats = CheckPlats(htmlNode);//是否拥有多个视频平台
            var result = new List<GatherResult>();
            if (HasList)
            {
                if (HasPlats)
                {
                    result.AddRange(this.OnPlatsAndList(htmlNode));
                }
                else
                {
                    result.Add(this.OnOnePlatAndList(htmlNode));
                }
            }
            else
            {
                if (HasPlats)
                {
                    result.AddRange(this.OnPlatsAndButton(htmlNode));
                }
                else
                {
                    result.AddRange(this.OnOnePlatAndButtons(htmlNode));
                }
            }

            return result;
        }

    }
}
