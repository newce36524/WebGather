using System;
using System.Collections.Generic;
using System.Text;
using WebGather.Video.Models;

namespace WebGather.Video
{
    /// <summary>
    /// 视频链接采集接口,各大网站的具体采集类,都必须实现该接口
    /// </summary>
    public interface IVideoGather : IDisposable
    {
        /// <summary>
        /// 根据文本内容获取查询结果
        /// </summary>
        /// <param name="url">搜索内容</param>
        /// <returns>搜索结果</returns>
        IEnumerable<GatherResult> GetSearchResult(string content);

    }

}
