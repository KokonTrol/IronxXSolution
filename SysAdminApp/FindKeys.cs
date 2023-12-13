using System.Threading.Tasks;

namespace SysAdminApp
{
    public static class FindKeys
    {
        public static Task<string> GetKeys(string text)
        {
            string idString = "";
            try
            {
                using (System.Diagnostics.Process p = new System.Diagnostics.Process())
                {
                    p.StartInfo = new System.Diagnostics.ProcessStartInfo("findKeys.exe", $"\"{text}\"");
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
                return Task.Run(() => idString);
            }
            catch
            {
                return Task.Run(() => idString);
            }
        }
    }
}
