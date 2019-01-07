using System;
using System.Collections.Generic;
using System.Text;

namespace WebGather.Video.Models
{
    /// <summary>
    /// 保存搜索结果
    /// </summary>
    public class GatherResult
    {
        /// <summary>
        /// 影片标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 影片简介
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 宣传图地址
        /// </summary>
        public string Pic { get; set; }
        /// <summary>
        /// 所属网站 youku tudou aiqiyi ...
        /// </summary>
        public string OwinPlat { get; set; }
        /// <summary>
        /// 爬虫接口
        /// </summary>
        public string OwinApi { get; set; }
        /// <summary>
        /// 影片剧集列表
        /// </summary>
        public IEnumerable<Drama> DramaList { get; set; }
    }
}
