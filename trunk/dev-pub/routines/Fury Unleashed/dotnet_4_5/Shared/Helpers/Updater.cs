// Thanks to Highvoltz for the autoupdater!

using FuryUnleashed.Interfaces.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Styx.Common;

namespace FuryUnleashed.Shared.Helpers
{
    internal static class Updater
    {
        private static readonly string FuSvnUrl = InternalSettings.Instance.General.SvnUrl == Enum.SvnUrl.Release
            ? "https://subversion.assembla.com/svn/team-random/trunk/release/routines/Fury Unleashed/dotnet_4_5/"
            : "https://subversion.assembla.com/svn/team-random/trunk/dev-pub/routines/Fury Unleashed/dotnet_4_5/";
        private static readonly Regex LinkPattern = new Regex(@"<li><a href="".+"">(?<ln>.+(?:..))</a></li>", RegexOptions.CultureInvariant);

        public static void CheckForUpdate()
        {
            CheckForUpdate(Styx.Common.Utilities.AssemblyDirectory + "\\Routines\\Fury Unleashed\\", true);
        }

        public static void CheckForUpdate(string path, bool checkallow)
        {
            try
            {
                Logger.InitLogO("\r\nChecking if the used revision is the latest, updates if not - Can be disabled in the GUI.");
                var remoteRev = GetRevision();

                if (InternalSettings.Instance.General.CurrentRevision != remoteRev)
                {
                    var logwrt = InternalSettings.Instance.General.CheckAutoUpdate ? "Downloading Update - Please wait." : "Please update manually!";
                    Logger.InitLogO("A new version was found. " + logwrt);
                    if (!InternalSettings.Instance.General.CheckAutoUpdate && checkallow) return;

                    DownloadFilesFromSvn(new WebClient(), FuSvnUrl, path);
                    InternalSettings.Instance.General.CurrentRevision = remoteRev;
                    InternalSettings.Instance.General.Save();

                    Logger.InitLogO("A new version of Fury Unleashed was installed. Please restart Honorbuddy.");
                }
                else
                {
                    Logger.InitLogO("No updates found.");
                }
            }
            catch (Exception ex)
            {
                Logger.DiagLogW("{0}.", ex);
            }
        }

        private static int GetRevision()
        {
            var client = new WebClient();
            var html = client.DownloadString(FuSvnUrl);
            var pattern = new Regex(@" - Revision (?<rev>\d+):", RegexOptions.CultureInvariant);
            Match match = pattern.Match(html);
            if (match.Success && match.Groups["rev"].Success)
                return int.Parse(match.Groups["rev"].Value);
            throw new Exception("FU: Unable to retrieve revision.");
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
                    if (url.Length > FuSvnUrl.Length)
                    {
                        var relativePath = url.Substring(FuSvnUrl.Length);
                        dirPath = Path.Combine(path, relativePath);
                        filePath = Path.Combine(dirPath, file);
                    }
                    else
                    {
                        dirPath = Environment.CurrentDirectory;
                        filePath = Path.Combine(path, file);
                    }

                    Logger.DiagLogW("FU: Downloading {0}.", file);

                    try
                    {
                        if (!Directory.Exists(dirPath))
                            Directory.CreateDirectory(dirPath);
                        client.DownloadFile(newUrl, filePath);
                        Logger.DiagLogW("FU: Download {0} done.", file);
                    }
                    catch (Exception ex)
                    {
                        Logger.DiagLogW("{0}.", ex);
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