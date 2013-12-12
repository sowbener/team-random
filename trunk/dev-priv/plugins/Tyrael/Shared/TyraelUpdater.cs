// Thanks to Highvoltz for the autoupdater!

using Styx.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace Tyrael.Shared
{
    internal static class Updater
    {
        private static readonly string TyraelSvnUrl = TyraelSettings.Instance.SvnUrl == TyraelUtilities.SvnUrl.Release
            ? "https://subversion.assembla.com/svn/team-random/trunk/release/plugins/Tyrael/"
            : "https://subversion.assembla.com/svn/team-random/trunk/dev-pub/plugins/Tyrael/";
        private static readonly Regex LinkPattern = new Regex(@"<li><a href="".+"">(?<ln>.+(?:..))</a></li>", RegexOptions.CultureInvariant);

        public static void CheckForUpdate()
        {
            CheckForUpdate(Utilities.AssemblyDirectory + "\\Bots\\Tyrael\\", true);
        }

        public static void CheckForUpdate(string path, bool checkallow)
        {
            try
            {
                Logging.Write(Colors.DodgerBlue, "\r\n[Tyrael] Checking if the used revision is the latest, updates if it is not.");
                var remoteRev = GetRevision();

                if (TyraelSettings.Instance.CurrentRevision != remoteRev)
                {
                    var logwrt = TyraelSettings.Instance.CheckAutoUpdate ? "[Tyrael] Downloading Update - Please wait." : "[Tyrael] Please update manually!";
                    Logging.Write(Colors.DodgerBlue, "[Tyrael] A new version was found. " + logwrt);
                    if (!TyraelSettings.Instance.CheckAutoUpdate && checkallow) return;

                    DownloadFilesFromSvn(new WebClient(), TyraelSvnUrl, path);
                    TyraelSettings.Instance.CurrentRevision = remoteRev;
                    TyraelSettings.Instance.Save();

                    Logging.Write(Colors.DodgerBlue, "[Tyrael] A new version of Fury Unleashed was installed. Please restart Honorbuddy.");
                }
                else
                {
                    Logging.Write(Colors.DodgerBlue, "[Tyrael] No updates found.");
                }
            }
            catch (Exception ex)
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "{0}.", ex);
            }
        }

        private static int GetRevision()
        {
            var wc = new WebClient();
            var webData = wc.DownloadString(TyraelSvnUrl + "version");
            Logging.WriteDiagnostic(Colors.MediumPurple, "[Tyrael] Current SVN version: {0}", int.Parse(webData));
            return int.Parse(webData);
            //throw new Exception("[Tyrael] Unable to retrieve revision");
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

                    Logging.WriteDiagnostic(Colors.MediumPurple, "[Tyrael] Downloading {0}.", file);

                    try
                    {
                        if (!Directory.Exists(dirPath))
                            Directory.CreateDirectory(dirPath);
                        client.DownloadFile(newUrl, filePath);
                        Logging.WriteDiagnostic(Colors.MediumPurple, "[Tyrael] Download {0} done.", file);
                    }
                    catch (Exception ex)
                    {
                        Logging.WriteDiagnostic(Colors.MediumPurple, "{0}.", ex);
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