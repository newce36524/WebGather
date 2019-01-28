using System;
using System.Collections.Generic;
using System.Text;

namespace WebGather.Video.Tension
{
    public class ParamModel
    {
        public string Id { get; set; }
        public string Plname { get; set; }
        public string Plat { get; set; }
        public string Type { get; set; }
        public string Range { get; set; }
        public string Data_type { get; set; }
        public string Video_type { get; set; }
        public string Otype { get; set; }
        public string Uid { get; set; }
        public string Callback { get; set; }
        public string CurrentTime { get; set; }

        public override string ToString()
        {
            return $"id={this.Id}&plname={this.Plname}&range={this.Range}&plat={this.Plat}&type={this.Type}&data_type={this.Data_type}&video_type={this.Video_type}&otype={this.Otype}&uid={this.Uid}&callback={this.Callback}&_t={this.CurrentTime}";
        }
    }

    public static class ParamModelExtension
    {
        public static string GetOnOnePlatAndButtonsLink(this ParamModel paramModel)
        {
            string link = $@"https://s.video.qq.com/get_playsource?{paramModel}";
            return link;
        }


        public static string GetOnOnePlatAndListLink(this ParamModel paramModel)
        {
            string link = $@"https://s.video.qq.com/get_playsource?id={paramModel.Id}&plname={paramModel.Plname}&range={paramModel.Range}&plat={paramModel.Plat}&type={paramModel.Type}&data_type={paramModel.Data_type}&video_type={paramModel.Video_type}&otype={paramModel.Otype}&uid={paramModel.Uid}&callback={paramModel.Callback}&_t={paramModel.CurrentTime}";
            return link;
        }

        public static string GetOnPlatsAndButtonLink(this ParamModel paramModel, bool flag)
        {
            string link;
            if (flag)
            {
                link = $@" https://s.video.qq.com/get_playsource?id={paramModel.Id}&plname={paramModel.Plname}&range={paramModel.Range}&plat={paramModel.Plat}&type={paramModel.Type}&data_type={paramModel.Data_type}&video_type={paramModel.Video_type}&otype={paramModel.Otype}&uid={paramModel.Uid}&callback={paramModel.Callback}&_t={paramModel.CurrentTime}";
            }
            else
            {
                link = $@"https://s.video.qq.com/get_playsource?id={paramModel.Id}&plname={paramModel.Plname}&range={paramModel.Range}&plat={paramModel.Plat}&type={paramModel.Type}&data_type={paramModel.Data_type}&video_type={paramModel.Video_type}&otype={paramModel.Otype}&uid={paramModel.Uid}&callback={paramModel.Callback}&_t={paramModel.CurrentTime}";
            }
            return link;
        }

        public static string GetOnPlatsAndListLink(this ParamModel paramModel, bool flag)
        {
            string link;
            if (flag)
            {
                link = $@"https://s.video.qq.com/get_playsource?id={paramModel.Id}&plname={paramModel.Plname}&range={paramModel.Range}&plat={paramModel.Plat}&type={paramModel.Type}&data_type={paramModel.Data_type}&video_type={paramModel.Video_type}&otype={paramModel.Otype}&uid={paramModel.Uid}&callback={paramModel.Callback}&_t={paramModel.CurrentTime}";
            }
            else
            {
                link = $@"https://s.video.qq.com/get_playsource?id={paramModel.Id}&plname={paramModel.Plname}&range={paramModel.Range}&plat={paramModel.Plat}&type={paramModel.Type}&data_type={paramModel.Data_type}&video_type={paramModel.Video_type}&otype={paramModel.Otype}&uid={paramModel.Uid}&callback={paramModel.Callback}&_t={paramModel.CurrentTime}";
            }
            return link;
        }


    }

}
