using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SongSlackbot.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace SongSlackbot.Utils
{
    public class SongCrawller
    {
        public ResultModels<List<Charts>> SongCrawling()
        {
            Trace.WriteLine("실행됨");
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
                        if (chart.Singer == null || chart.Title == null) //크롤링 도중 가수명과 곡 제목을 수집하지 못했을 시
                        {
                            result = new ResultModels<List<Charts>>(false, list);
                            return result;
                        }
                        list.Add(chart);
                    }
                }
                if (list.Count() == 100) //100개의 행을 모두 성공적으로 파싱할 시
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
    }
}