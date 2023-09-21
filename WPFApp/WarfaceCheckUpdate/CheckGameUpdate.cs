using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WarfaceCheckUpdate
{
    public static class CheckGameUpdate
    {
        private static Dictionary<string, int> _month = new Dictionary<string, int>()
                                                        {
                                                            { "Янв", 1 },
                                                            { "Фев", 2 },
                                                            { "Мар", 3 },
                                                            { "Апр", 4 },
                                                            { "Май", 5 },
                                                            { "Июн", 6 },
                                                            { "Июл", 7 },
                                                            { "Авг", 8 },
                                                            { "Сен", 9 },
                                                            { "Окт", 10 },
                                                            { "Ноя", 11 },
                                                            { "Дек", 12 },
                                                        };
        private static string _link = @"https://ru.warface.com/news/update";
        public static string GameName() { return "Warface"; }
        public static DateTime Check()
        {
            Task<DateTime> task = Task.Run(() => LoadPage());
            task.Wait();

            return task.Result;
        }

        private static DateTime LoadPage()
        {
            try
            {
                HtmlWeb web = new HtmlWeb();

                var htmlDoc = web.Load(_link);

                if (htmlDoc == null)
                    return new DateTime(0);

                var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='item-list']//li").Where(n => n.Attributes["class"].Value.Contains("views-row")).ToList();
                Regex regex = new Regex(@"[\t\r\n\s]*(\d+)[\t\r\n\s]+(\w+)[\t\r\n\s]+(\d+)[\t\r\n\s.]*", RegexOptions.Multiline);

                if (nodes.Count() >= 1)
                {
                    MatchCollection matches;
                    matches = regex.Matches(nodes[0].InnerText);
                    DateTime date1 = new DateTime(Convert.ToInt32(matches[0].Groups[3].Value),
                                                        _month[matches[0].Groups[2].Value],
                                                        Convert.ToInt32(matches[0].Groups[1].Value));
                    if (nodes.Count> 1)
                    {
                        matches = regex.Matches(nodes[1].InnerText);
                        DateTime date2 = new DateTime(Convert.ToInt32(matches[0].Groups[3].Value),
                                                            _month[matches[0].Groups[2].Value],
                                                            Convert.ToInt32(matches[0].Groups[1].Value));
                        return date1 > date2 ? date1 : date2;
                    }
                    else
                    {
                        return date1;
                    }

                }
                else
                {
                    return new DateTime(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DateTime(0);
            }

        }
    }
}
