using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace Dota2CheckUpdate
{
    public static class CheckGameUpdate 
    {
        private static string _link = @"https://steamdb.info/api/PatchnotesRSS/?appid=570";
        public static string GameName() { return "Dota 2"; }
        public static DateTime Check()
        {
            Task<DateTime> task = Task.Run(() => LoadPage());
            task.Wait();
            
            return task.Result;
        }

        private async static Task<DateTime> LoadPage()
        {
            try
            {

                HttpClient _client;
                HttpClientHandler handler = new HttpClientHandler();
                _client = new HttpClient(handler);
                _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.99 Safari/537.36");
                _client.BaseAddress = new UriBuilder(_link).Uri;

                HttpRequestMessage request = new HttpRequestMessage();

                request = new HttpRequestMessage(HttpMethod.Get, _link);
                var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(await response.Content.ReadAsStringAsync());

                if (htmlDoc == null)
                    return new DateTime(0);


                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(htmlDoc.Text);

                var nod = xDoc.DocumentElement.SelectNodes("descendant::item");

               
                if (nod.Count>0)
                {
                    var itemUpdate = nod.Item(0).SelectNodes("descendant::pubDate").Item(0).InnerText;
                    return DateTime.Parse(itemUpdate);
                     
                }
                else
                {
                    return new DateTime(0);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DateTime(0);
            }

        }
    }
}
