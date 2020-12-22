using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using SongSlackbot.Models;
using System.Data.Entity;
using OpenQA.Selenium;
using System.Diagnostics;
using OpenQA.Selenium.Chrome;
using System.Threading.Tasks;
using SongSlackbot.Utils;

namespace SongSlackbot.Controllers
{


    public class ChartController : ApiController
    {
        SongBotEntities db = new SongBotEntities();
        SongCrawller crawller = new SongCrawller();

        /*
        public string method_test()
        {
            return "hello";
        }
        */

        // GET api/<controller>
        public ResultModels<List<Charts>> Get()
        {
            Trace.WriteLine("[Request Mapping : GET : api/chart]");
            string type = Request.GetQueryNameValuePairs().Where(v => v.Key == "type").Select(v => v.Value).FirstOrDefault();
            List<Charts> list = null;
            ResultModels<List<Charts>> result;
            try
            {
                if (type == "new")
                {
                    list = db.Charts.Where(x => x.Status == 2).ToList<Charts>();
                }
                else
                {
                    list = db.Charts.Where(x => x.Status == 1 || x.Status == 2).ToList<Charts>();
                }
                result = new ResultModels<List<Charts>>(true, list);
            }
            catch(Exception e)
            {
                result = new ResultModels<List<Charts>>(false, list);
                return result;
            }
            return result;
        }

        // POST api/<controller> : 최신 차트 목록을 크롤링해서 곡 DB에 저장함. 이전 차트 목록은 만료 상태로 변경, 만료 상태의 차트 목록은 삭제
        public async Task<Dictionary<string, string>> Post()
        {
            Trace.WriteLine("[Request Mapping : POST : api/chart/:idx]");
            var task = Task.Run(() => crawller.SongCrawling());
            ResultModels<List<Charts>> result = await task;
            Trace.WriteLine("" + result.Success + result.ResultList.Count);
            Dictionary<string, string> resultMap = new Dictionary<string, string>();
            if (result.Success == true)
            {
                try
                {
                    // Status == 0 
                    List<Charts> delete_list = db.Charts.Where(x => x.Status == 0).ToList<Charts>();
                    db.Charts.RemoveRange(delete_list); //이전 차트 삭제

                    // Status == 1 or 2 
                    List<Charts> before_list = db.Charts.Where(x => x.Status == 1 || x.Status == 2).Take(100).OrderBy(x => x.Rank).ToList<Charts>();
                    foreach(Charts chart in before_list)    //추출 후 사용안함 상태로 변경
                    {
                        chart.Status = 0;
                    }

                    // NEW
                    foreach (Charts chart in result.ResultList)
                    {
                        bool exist = before_list.Any(x => x.Title == chart.Title);
                        if (!exist) //새로 Chart in
                        {
                            chart.Status = 2;   //Status == 2 : 신곡 
                        }
                        db.Charts.Add(chart);
                    }
                    db.SaveChanges();   //100개 전부 성공적으로 insert시 commit
                    resultMap.Add("result", "success");
                    return resultMap;
                }
                catch
                {
                    resultMap.Add("result", "fail");
                    return resultMap;
                }
            }
            resultMap.Add("result", "fail");
            return resultMap;
        }
        
        //test
        public Dictionary<string, string> Post(int id)
        {
            Dictionary<string, string> resultMap = new Dictionary<string, string>();
            resultMap.Add("result", "fail");
            return resultMap;
        }
    }
}