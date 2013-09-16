using Styx.Common;
using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace Tyrael.Shared
{
    public class TyraelHelper
    {
        public enum LockState
        {
            True,
            False
        }

        internal static void StatCounter()
        {
            try
            {
                var statcounterDate = DateTime.Now.DayOfYear.ToString(CultureInfo.InvariantCulture);
                if (!statcounterDate.Equals(TyraelSettings.Instance.LastStatCounted))
                {
                    Parallel.Invoke(
                        () => new WebClient().DownloadData("http://c.statcounter.com/9219924/0/e3fed179/1/"),
                        () => Logging.WriteDiagnostic("FU: StatCounter has been updated!"));
                    TyraelSettings.Instance.LastStatCounted = statcounterDate;
                    TyraelSettings.Instance.Save();
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { /* Catch all errors */ }
        }
    }
}
