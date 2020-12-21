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

namespace SongSlackbot.Controllers
{
    public class ChartController : ApiController
    {
        SongBotEntities db = new SongBotEntities();

        //크롤링 메서드
        public ResultModels<List<Charts>> SongCrawling()
        {
            List<Charts> list = new List<Charts>();
            ResultModels<List<Charts>> result;
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            var options = new ChromeOptions(); 
            options.AddArgument("--window-position=-32000,-32000"); 
            options.AddArgument("headless"); //윈도우창 위치값을 화면밖으로 조정
            try
            {
                using (IWebDriver driver = new ChromeDriver(service, options))
                {
                    driver.Url = "https://www.melon.com/chart/index.htm";
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
                    var root = driver.FindElement(By.XPath("//*[@id='frm']/div/table/tbody"));
                    var trs = root.FindElements(By.CssSelector("tr"));
                    for (int i = 1; i <= trs.Count; i++)
                    {
                        Charts chart = new Charts();
                        chart.Title = root.FindElement(By.XPath("//tr[" + i + "]/td[4]/div/div/div[1]/span/a")).Text;
                        chart.Rank = i;

                        var singer_root = root.FindElement(By.XPath("//tr[" + i + "]/td[4]/div/div/div[2]"));
                        var singer_a_tags = singer_root.FindElements(By.XPath("./a"));

                        string singers = "";
                        int singer_cnt = singer_a_tags.Count;
                        for (int j = 1; j <= singer_cnt; j++)
                        {
                            string singer = singer_root.FindElement(By.XPath("./a[" + j + "]")).Text;
                            Trace.WriteLine(singer);
                            if (j == singer_cnt)
                            {
                                singers += singer;
                                break;
                            }
                            singers += singer + ", ";
                        }

                        Trace.WriteLine(i + " " + singer_cnt);
                        chart.Singer = singers;
                        chart.Regdate = DateTime.Today;
                        chart.Status = 1;
                        if(chart.Singer == null || chart.Title == null) //크롤링 도중 가수명과 곡 제목을 수집하지 못했을 시
                        {
                            result = new ResultModels<List<Charts>>(false, list);
                            return result;
                        }
                        list.Add(chart);
                    }
                }
                if(list.Count() == 100) //100개의 행을 모두 성공적으로 파싱할 시
                {
                    result = new ResultModels<List<Charts>>(true, list);
                    return result;
                }
                else //모든 행을 파싱하는 데 실패할 시
                {
                    result = new ResultModels<List<Charts>>(false, list);
                    return result;
                }
            }
            catch (Exception e)  //예외가 발생할 시
            {
                Trace.WriteLine(e);
                result = new ResultModels<List<Charts>>(false, list);
                return result;
            }
        }

        // GET api/<controller> //일단 테스트용으로 GET메서드. 크롤링해서 곡 db에 저장함.
        public IHttpActionResult Get()
        {
            ResultModels<List<Charts>> result = SongCrawling();
            Trace.WriteLine("" + result.Success + result.ResultList.Count);
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
                    return Json("success");
                }
                catch
                {
                    return Json("fail");
                }
            }
            return Json("fail");
        }


        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}