using Library.Functions;
using Library.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IronXHelper
{
    public static class SearchingHelperInfo
    {
        public static Task<List<HelperInfo>> Searching(string searchLine)
        {
            var data = JsonDocument.Parse(BaseFunctions.GetStringFileFromResources("DBConfig.json"));
            string dbPath = Path.Combine(BaseFunctions.GetDocumentFolder(), data.RootElement.GetProperty("DBName").ToString());

            string idString = "";
            using (System.Diagnostics.Process p = new System.Diagnostics.Process())
            {
                p.StartInfo = new System.Diagnostics.ProcessStartInfo("searchingModule.exe", $"-DBpath=\"{dbPath}\" -query=\"{searchLine}\"");
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.ErrorDialog = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                p.Start();

                p.WaitForExit();
                idString = p.StandardOutput.ReadToEnd();
                p.Kill();
            }
            List<HelperInfo> infos = new List<HelperInfo>();

            idString = Regex.Replace(idString, "[^\\d ]+", "");
            if(idString.Length > 1)
            {
                IronContext context = new IronContext();

                var ids = idString.Split(" ");
                foreach (string id in ids)
                {
                    infos.Add(context.HelperInfo.Where(x => x.Id.ToString() == id).First());
                }
                return Task.Run(() => infos);
            }
            else
            {
                return Task.Run(() =>  infos );
            }
        }
    }
}
