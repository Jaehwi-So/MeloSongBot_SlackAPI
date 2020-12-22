using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace SongSlackbot.Slackbot
{
    public class SlackEventHandler
    {
        SlackBot slackBot = new SlackBot();
        public void InitChartsScheduleTask()
        {   /* 최신 차트 크롤링 실행, 목록 보여줌..

            */
            slackBot.PostMessage(title: "YYYY-MM-DD 의 신곡",
                     username: "MeloSongBot",
                     fallback: "오늘의 신곡입니다.",
                     text: "안녕",
                     channel: "#test",
                     iconurl: "icon_url");

            HttpRuntime.Cache.Add(
                "ScheduledTask",
                1, //Cache할 값
                null, Cache.NoAbsoluteExpiration,
                TimeSpan.FromSeconds(5),
                CacheItemPriority.NotRemovable,
                new CacheItemRemovedCallback(callBackInitChartsSchedule));
        }

        //캐시 만료시 설정
        public void callBackInitChartsSchedule(string key, Object value, CacheItemRemovedReason reason)
        {
            //캐쉬 재실행
            InitChartsScheduleTask();
        }
    }
}