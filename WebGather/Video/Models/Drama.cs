using System;
using System.Collections.Generic;
using System.Text;

namespace WebGather.Video.Models
{
    /// <summary>
    /// 保存剧集信息
    /// </summary>
    public class Drama
    {
        /// <summary>
        /// 剧集标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 缩略图
        /// </summary>
        public string Pic { get; set; }
        /// <summary>
        /// 剧集链接
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// 剧集简介
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 当前集数
        /// </summary>
        public int Order { get; set; }
    }
}
