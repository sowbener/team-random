#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

#endregion

namespace YourRaidingBuddy.Helpers
{
    internal class Updater
    {
        private const string SvnUrl = "http://85.214.202.147:8553/svn/yrebornbuddy/";
        //     private const string ChangeLogUrl = "http://code.google.com/p/YourRaidingBuddy/source/detail?r=";

        private static readonly Regex LinkPattern = new Regex(@".*<(file|dir).*href=""(.+)"" />");

        public static string ChangeLog;
        public static string NewString;

        private static readonly Regex ChangelogPattern =
            new Regex(
                "<h4 style=\"margin-top:0\">Log message</h4>\r?\n?<pre class=\"wrap\" style=\"margin-left:1em\">(?<log>.+\r?\n?.+\r?\n?.+\r?\n?.+\r?\n?.+\r?\n?.+\r?\n?.+\r?\n?.+\r?\n?.+\r?\n?.+\r?\n?.+\r?\n?.+\r?\n?)</pre>",
                RegexOptions.CultureInvariant);

        public static int CcRevision
        {
            get
            {
                int revision = 0;

                try
                {
                    string path = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
                        @"Routines\YourRaidingBuddy\MiscStuff.xml");

                    var reader = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    var xmlDocument = new XmlDocument();
                    xmlDocument.Load(reader);
                    XmlNodeList nodeList = xmlDocument.GetElementsByTagName("MiscInformation");
                    revision = Convert.ToInt16(nodeList[0].FirstChild.ChildNodes[0].InnerText);
                }

                catch
                {
                }

                return revision;
            }

            set
            {
                string path = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
                    @"Routines\YourRaidingBuddy\MiscStuff.xml");

                var reader = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(reader);

                XmlNodeList nodeList = xmlDocument.GetElementsByTagName("MiscInformation");
                nodeList[0].FirstChild.ChildNodes[0].InnerText = value.ToString(CultureInfo.InvariantCulture);

                var writer = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
                xmlDocument.Save(writer);
            }
        }

        public static void DeleteAll()
        {
            Logger.Write("Preparing update process.");
            Logger.Write("Delete old files in YourRaidingBuddy folder.");
            string basePath = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
                @"Routines\YourRaidingBuddy\");
            var downloadedMessageInfo = new DirectoryInfo(basePath);

            foreach (FileInfo file in downloadedMessageInfo.GetFiles().Where(file => file.Name != "MiscStuff.xml"))
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in downloadedMessageInfo.GetDirectories().Where(dir => dir.Name != ".svn" || dir.Name != "$tf" || dir.Name != ".vs" || dir.Name != "obj" || dir.Name != "bin"))
            {
                dir.Delete(true);
            }
            Logger.Write("Delete old files in Config folder.");
            Logger.Write("Cleanup completed. You need to reconfigure YourRaidingBuddy!");
        }

        public static void CheckForUpdate()
        {
            try
            {
                int revision = CcRevision;
                int onlineRevision = GetOnlineRevision();

                Logger.Write("Checking online for new revision of YourRaidingBuddy");
                if (revision < onlineRevision)
                {
                    DeleteAll();
                    //string changeLog = GetChangeLog(onlineRevision);
                    Logger.Write("Revision {0} is available for download, you are currently using rev {1}.",
                        onlineRevision, revision);
                    Logger.Write("This will now download in the background, you will be informed when its complete.");

                    DownloadFilesFromSvn(new WebClient(), SvnUrl);
                    Logger.Write(" ");
                    Logger.Write("Download of revision " + onlineRevision +
                                  " is complete. You must close and restart RB for the changes to be applied.");
                    Logger.Write(" ");
                    CcRevision = onlineRevision;
                 //   ChangeLog = GetChangeLog(onlineRevision);
                 //   NewString = ChangeLog.Replace("//", Environment.NewLine);
                //    if (YourRaidingBuddySettings.Instance.Changelog)
                //        new ChangelogForm().ShowDialog();
                }
                else
                {
                    Logger.Write("No updates have been found. Revision " + revision + " is the latest build.");
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.ToString(), "Wtf something went wrong?");
            }
        }

        private static int GetOnlineRevision()
        {
            var client = new WebClient();
            client.Credentials = new NetworkCredential("yreborndll", "lolitsux!YES");
            string html = client.DownloadString(SvnUrl);
            var pattern = new Regex(@"(.*)index rev=\""(\d+)\""(.*)");
            Match match = pattern.Match(html);
            if (match.Success) return int.Parse(match.Groups[2].Value);
            throw new Exception("Unable to retreive revision! The sky is falling!");
        }

        private static void DownloadFilesFromSvn(WebClient client, string url)
        {
            try
            {
                string basePath = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
                    @"Routines\YourRaidingBuddy\");
                client.Credentials = new NetworkCredential("yreborndll", "lolitsux!YES");
                string html = client.DownloadString(url);
                //    Console.WriteLine("Getting Our response: " + html.Substring(0, 1400));
                MatchCollection results = LinkPattern.Matches(html);
                IEnumerable<Match> matches = from match in results.OfType<Match>()
                                             where match.Success && match.Groups[1].Success
                                             select match;
                foreach (Match match in matches)
                {
                    string file = RemoveXmlEscapes(match.Groups[2].Value);
                    string newUrl = url + file;
                    if (newUrl[newUrl.Length - 1] == '/') // it's a directory...
                    {
                        DownloadFilesFromSvn(client, newUrl);
                    }
                    else // its a file.
                    {
                        string filePath, dirPath;
                        if (url.Length > SvnUrl.Length)
                        {
                            string relativePath = url.Substring(SvnUrl.Length);
                            dirPath = Path.Combine(basePath, relativePath);
                            filePath = Path.Combine(dirPath, file);
                        }
                        else
                        {
                            dirPath = Environment.CurrentDirectory;
                            filePath = Path.Combine(basePath, file);
                            Console.WriteLine(filePath);
                        }
                        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                        client.DownloadFile(newUrl, filePath);
                    }
                }
            }
            catch (WebException e)
            {

                Console.WriteLine(e.Message);
            }
        }

        private static string RemoveXmlEscapes(string xml)
        {
            return
                xml.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace(
                    "&apos;", "'");
        }

        private static string GetChangeLog(int revision)
        {
            var client = new WebClient();
         //   string html = client.DownloadString(ChangeLogUrl + revision);
        //    Match match = ChangelogPattern.Match(html);
        //    if (match.Success && match.Groups["log"].Success) return RemoveXmlEscapes(match.Groups["log"].Value);
            return null;
        }
    }
}