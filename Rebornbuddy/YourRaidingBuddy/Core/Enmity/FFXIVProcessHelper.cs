using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace YourRaidingBuddy.Helpers
{

    public static class FfxivProcessHelper
    {
        private static IList<Process> GetFfxivProcessList()
        {
            return (
                from x in Process.GetProcessesByName("ffxiv")
                where !x.HasExited && x.MainModule.ModuleName == "ffxiv.exe"
                select x).Union(
                    from x in Process.GetProcessesByName("ffxiv_dx11")
                    where !x.HasExited && x.MainModule.ModuleName == "ffxiv_dx11.exe"
                    select x).ToList();
        }

        public static Process GetFfxivProcess(int pid = 0)
        {
            Process result;
            try
            {
                var list = GetFfxivProcessList();
                if (pid == 0)
                {
                    if (list.Any())
                    {
                        result = (
                            from x in list
                            orderby x.Id
                            select x).FirstOrDefault();
                    }
                    else
                    {
                        result = null;
                    }
                }
                else
                {
                    result = list.FirstOrDefault(x => x.Id == pid);
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }
    }
}