using HtmlAgilityPack;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FortniteCheckUpdate
{
    public static class CheckGameUpdate
    {
        private static string _link = @"https://fortnitenews.com";

        public static string GameName() { return "Fortnite"; } 
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
                DateTime date = new DateTime(0);
                HtmlWeb web = new HtmlWeb();

                var htmlDoc = web.Load(_link);

                if (htmlDoc == null)
                    return new DateTime(0);

                var nodes = htmlDoc.DocumentNode
                    .SelectNodes("//div[@class='sidebar__section']/article[@class='sidebar__story']/h6[@class='sidebar__story-title']/a");
                
                if (nodes != null)
                {
                    var href = nodes.FirstOrDefault().Attributes["href"].Value;
                    htmlDoc = web.Load(_link+ href);
                    if (htmlDoc == null)
                        return new DateTime(0);
                    nodes = htmlDoc.DocumentNode
                    .SelectNodes("//div[@class='post-header__info']/time[@class='post-header__date']");
                    if (nodes != null)
                    {
                        return DateTime.ParseExact(nodes.First().Attributes["datetime"].Value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    return new DateTime(0);

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