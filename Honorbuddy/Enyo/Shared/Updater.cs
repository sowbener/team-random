using Styx.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Enyo.Shared
{
    class Updater
    {
        private static readonly string TyraelSvnUrl = "https://subversion.assembla.com/svn/team-random/Honorbuddy/Enyo/";
        private static readonly Regex LinkPattern = new Regex(@"<li><a href="".+"">(?<ln>.+(?:..))</a></li>", RegexOptions.CultureInvariant);

        public static void CheckForUpdate()
        {
            CheckForUpdate(Utilities.AssemblyDirectory + "\\Bots\\Enyo\\", true);
        }

        static void CheckForUpdate(string path, bool checkallow)
        {
            try
            {
                Logger.PrintLog("\r\n------------------------------------------");
                Logger.PrintLog("Checking if the used revision is the latest, updates if it is not.");
                var remoterev = GetRevision();

                if (BotSettings.Instance.CurrentRevision != remoterev)
                {
                    var logwrt = BotSettings.Instance.AutoUpdate ? "Downloading Update - Please wait." : "Please update manually!";
                    Logger.PrintLog("A new version was found.\r\n" + logwrt);

                    if (!BotSettings.Instance.AutoUpdate && checkallow)
                        return;

                    DownloadFilesFromSvn(new WebClient(), TyraelSvnUrl, path);
                    BotSettings.Instance.CurrentRevision = remoterev;
                    BotSettings.Instance.Save();

                    Logger.PrintLog("A new version of Tyrael was installed. Please restart Honorbuddy.");
                    Logger.PrintLog("------------------------------------------");
                }
                else
                {
                    Logger.PrintLog("No updates found.");
                    Logger.PrintLog("------------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Logger.DiagnosticLog("CheckForUpdate Error: {0}.", ex);
            }
        }

        private static int GetRevision()
        {
            try
            {
                var wc = new WebClient();
                var webData = wc.DownloadString(TyraelSvnUrl + "version");

                Logger.DiagnosticLog("Current SVN version: {0}", int.Parse(webData));
                return int.Parse(webData);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private static void DownloadFilesFromSvn(WebClient client, string url, string path)
        {
            var html = client.DownloadString(url);
            MatchCollection results = LinkPattern.Matches(html);

            IEnumerable<Match> matches = from match in results.OfType<Match>()
                                         where match.Success && match.Groups["ln"].Success
                                         select match;
            foreach (Match match in matches)
            {
                var file = RemoveXmlEscapes(match.Groups["ln"].Value);
                var newUrl = url + file;
                if (newUrl[newUrl.Length - 1] == '/')
                {
                    DownloadFilesFromSvn(client, newUrl, path);
                }
                else
                {
                    string filePath, dirPath;
                    if (url.Length > TyraelSvnUrl.Length)
                    {
                        var relativePath = url.Substring(TyraelSvnUrl.Length);
                        dirPath = Path.Combine(path, relativePath);
                        filePath = Path.Combine(dirPath, file);
                    }
                    else
                    {
                        dirPath = Environment.CurrentDirectory;
                        filePath = Path.Combine(path, file);
                    }

                    Logger.DiagnosticLog("Downloading {0}.", file);

                    try
                    {
                        if (!Directory.Exists(dirPath))
                            Directory.CreateDirectory(dirPath);
                        client.DownloadFile(newUrl, filePath);
                        Logger.DiagnosticLog("Download {0} done.", file);
                    }
                    catch (Exception ex)
                    {
                        Logger.DiagnosticLog("DownloadFilesFromSvn Error: {0}.", ex);
                    }
                }
            }
        }

        private static string RemoveXmlEscapes(string xml)
        {
            return xml.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&apos;", "'");
        }
    }
}
